using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Mvc;
using ServerlessBloggingPlatform.Entities;
using ServerlessBloggingPlatform.Pagination;
using ServerlessBloggingPlatform.Services;

namespace ServerlessBloggingPlatform.Services;

public class PostService : IPostService
{
    private readonly IAmazonDynamoDB _dynamoDB;
    private readonly string _postTableName;

    public PostService(
        IAmazonDynamoDB dynamoDB,
        IConfiguration configuration
    )
    {
        _dynamoDB = dynamoDB;

        var postTable = configuration.GetValue<string>("DynamoDB:PostTable");
        if(postTable == null) throw new ArgumentNullException(nameof(postTable));
        
        _postTableName = postTable;
    }

    public async Task<PostDTOPaged> GetPosts(PaginationDTO pagination)
    {
        // If startkey is null then we are on the first page
        var isFirstPage = pagination.FirstPage;
        var lastKey = pagination.LastKey;
        var startKey = lastKey != null || !isFirstPage ? new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = lastKey.ToString() } }
        } : null;

        // Get posts from DynamoDB
        var request = new ScanRequest
        {
            TableName = _postTableName,
            Limit = pagination.PageSize,
            ExclusiveStartKey = startKey
        };

        var response = await _dynamoDB.ScanAsync(request);
        
        return new PostDTOPaged {
            Posts = response.Items.Select(x => new PostDTO
            {
                Id = Guid.Parse(x["Id"].S),
                Title = x["Title"].S,
                Content = x["Content"].S,
                UserId = Guid.Parse(x["UserId"].S)
            }).ToList(),
            LastKey = response.LastEvaluatedKey != null ? Guid.Parse(response.LastEvaluatedKey["Id"].S) : null,
            FirstPage = isFirstPage
        };
    }

    public async Task<PostDTO?> GetPost(Guid id)
    {
        // Get post with specific id from DynamoDB
        var request = new GetItemRequest
        {
            TableName = _postTableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = id.ToString() } }
            }
        };

        var response = await _dynamoDB.GetItemAsync(request);

        if(response.Item == null) return null;

        return new PostDTO
        {
            Id = Guid.Parse(response.Item["Id"].S),
            Title = response.Item["Title"].S,
            Content = response.Item["Content"].S,
            UserId = Guid.Parse(response.Item["UserId"].S)
        };
    }

    public async Task<PostDTOPaged> GetPostsByUser(Guid userId, PaginationDTO pagination)
    {
        // Get Post with specific user id from DynamoDB
        var isFirstPage = pagination.FirstPage;
        var lastKey = pagination.LastKey;
        var startKey = lastKey != null || !isFirstPage ? new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = lastKey.ToString() } }
        } : null;

        var request = new ScanRequest
        {
            TableName = _postTableName,
            Limit = pagination.PageSize,
            ExclusiveStartKey = startKey,
            FilterExpression = "UserId = :userId",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":userId", new AttributeValue { S = userId.ToString() } }
            }
        };

        var response = await _dynamoDB.ScanAsync(request);

        return new PostDTOPaged {
            Posts = response.Items.Select(x => new PostDTO
            {
                Id = Guid.Parse(x["Id"].S),
                Title = x["Title"].S,
                Content = x["Content"].S,
                UserId = Guid.Parse(x["UserId"].S)
            }).ToList(),
            LastKey = response.LastEvaluatedKey != null ? Guid.Parse(response.LastEvaluatedKey["Id"].S) : null,
            FirstPage = isFirstPage
        };
    }

    public async Task<Post> CreatePost(CreatePostDTO post)
    {
        // Create post in DynamoDB
        var request = new PutItemRequest
        {
            TableName = _postTableName,
            Item = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = Guid.NewGuid().ToString() } },
                { "UserId", new AttributeValue { S = post.UserId.ToString() } },
                { "Title", new AttributeValue { S = post.Title } },
                { "Content", new AttributeValue { S = post.Content } }
            }
        };

        await _dynamoDB.PutItemAsync(request);

        return new Post
        {
            Id = Guid.Parse(request.Item["Id"].S),
            UserId = Guid.Parse(request.Item["UserId"].S),
            Title = request.Item["Title"].S,
            Content = request.Item["Content"].S
        };
    }

    public async Task<PostDTO> UpdatePost(Guid id, CreatePostDTO post)
    {
        // Update post in DynamoDB
        var request = new UpdateItemRequest
        {
            TableName = _postTableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = id.ToString() } }
            },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":title", new AttributeValue { S = post.Title } },
                { ":content", new AttributeValue { S = post.Content } }
            },
            UpdateExpression = "SET Title = :title, Content = :content",
            ReturnValues = "ALL_NEW"
        };

        var response = await _dynamoDB.UpdateItemAsync(request);

        return new PostDTO
        {
            Id = Guid.Parse(response.Attributes["Id"].S),
            Title = response.Attributes["Title"].S,
            Content = response.Attributes["Content"].S,
            UserId = Guid.Parse(response.Attributes["UserId"].S)
        };
    }
}

public interface IPostService
{
    Task<PostDTOPaged> GetPosts(PaginationDTO pagination);
    Task<PostDTO?> GetPost(Guid id);
    Task<PostDTOPaged> GetPostsByUser(Guid userId, PaginationDTO pagination);
    Task<Post> CreatePost(CreatePostDTO post);
    Task<PostDTO> UpdatePost(Guid id, CreatePostDTO post);
}

