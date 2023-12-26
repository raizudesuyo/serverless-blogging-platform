using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using ServerlessBloggingPlatform.Entities;
using ServerlessBloggingPlatform.Pagination;

namespace ServerlessBloggingPlatform.Services {

    public class CommentService : ICommentService
    {
        private readonly IAmazonDynamoDB _dynamoDB;
        private readonly string _commentTableName;
        public CommentService(
            IAmazonDynamoDB dynamoDB,
            IConfiguration configuration
        )
        {
            _dynamoDB = dynamoDB;
            
            var commentTableName = configuration.GetValue<string>("DynamoDB:CommentTable");
            if(commentTableName == null) throw new ArgumentNullException(nameof(commentTableName));

            _commentTableName = commentTableName;
        }

        public async Task<CommentDTOPaged> GetComments(Guid postId, PaginationDTO pagination)
        {
            // Get comments for a post from dynamodb based on postId
            var lastKey = pagination.LastKey;
            var isFirstPage = pagination.FirstPage;
            var startKey = lastKey != null || !isFirstPage ? new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = lastKey.ToString() } }
            } : null;

            var request = new ScanRequest
            {
                TableName = _commentTableName,
                Limit = pagination.PageSize,
                ExclusiveStartKey = startKey,
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    { ":postId", new AttributeValue { S = postId.ToString() } }
                },
                FilterExpression = "PostId = :postId"
            };

            var response = await _dynamoDB.ScanAsync(request);

            return new CommentDTOPaged {
                Comments = response.Items.Select(x => new CommentDTO
                {
                    Id = Guid.Parse(x["Id"].S),
                    PostId = Guid.Parse(x["PostId"].S),
                    UserId = Guid.Parse(x["UserId"].S),
                    ParentId = Guid.Parse(x["ParentId"].S),
                    Content = x["Content"].S
                }).ToList(),
                LastKey = response.LastEvaluatedKey != null ? Guid.Parse(response.LastEvaluatedKey["Id"].S) : null,
                FirstPage = isFirstPage
            };
        }   

        public async Task<CommentDTO> CreateComment(Guid postId, CreateCommentDTO comment)
        {
            // Create a comment in dynamodb
            var id = Guid.NewGuid();
            var request = new PutItemRequest
            {
                TableName = _commentTableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { S = id.ToString() } },
                    { "PostId", new AttributeValue { S = postId.ToString() } },
                    { "UserId", new AttributeValue { S = comment.UserId.ToString() } },
                    { "ParentId", new AttributeValue { S = comment.ParentId.ToString() } },
                    { "Content", new AttributeValue { S = comment.Content } }
                }
            };

            await _dynamoDB.PutItemAsync(request);

            return new CommentDTO
            {
                Id = id,
                PostId = postId,
                UserId = comment.UserId,
                ParentId = comment.ParentId,
                Content = comment.Content
            };
        }
    }

    public interface ICommentService
    {
        Task<CommentDTOPaged> GetComments(Guid postId, PaginationDTO pagination);
        Task<CommentDTO> CreateComment(Guid postId, CreateCommentDTO comment);
    }
}