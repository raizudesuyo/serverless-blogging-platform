using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using ServerlessBloggingPlatform.Entities;

namespace ServerlessBloggingPlatform.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly string _dynamoDbTableName;
        public AssetRepository(
            IAmazonDynamoDB dynamoDbClient,
            IConfiguration configuration
        )
        {
            _dynamoDbClient = dynamoDbClient;

            var tableName = configuration.GetValue<string>("DynamoDB:AssetTable");

            if(tableName == null) throw new ArgumentNullException(nameof(tableName));

            _dynamoDbTableName = tableName.ToString();
        }

        public async Task AddAsset(Asset asset)
        {
            // Write assetId and URL to DynamoDB
            var putItemRequest = new PutItemRequest
            {
                TableName = _dynamoDbTableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "AssetId", new AttributeValue { S = asset.Id.ToString() } },
                    { "Name", new AttributeValue { S = asset.Name } },
                    { "UserId", new AttributeValue { S = asset.UserId.ToString() } },
                    { "Url", new AttributeValue { S = asset.Url } }
                }
            };

            await _dynamoDbClient.PutItemAsync(putItemRequest);
        }

        public async Task<Asset?> GetAsset(Guid id)
        {
            // Reads from dynamodb table asset based on id
            var getItemRequest = new GetItemRequest
            {
                TableName = _dynamoDbTableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { "AssetId", new AttributeValue { S = id.ToString() } }
                }
            };

            var response = await _dynamoDbClient.GetItemAsync(getItemRequest);

            if (response.Item == null)
            {
                return null;
            }

            return new Asset
            {
                Id = id,
                Name = response.Item["Name"].S,
                Url = response.Item["Url"].S,
                UserId = Guid.Parse(response.Item["UserId"].S)
            };
        }
    }

    public interface IAssetRepository
    {
        Task AddAsset(Asset asset);
        Task<Asset?> GetAsset(Guid id);
    }
}
