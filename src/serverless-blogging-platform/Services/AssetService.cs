using Amazon.S3;
using Amazon.S3.Model;
using ServerlessBloggingPlatform.Repositories;

namespace ServerlessBloggingPlatform.Services
{
    public class AssetService : IAssetService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _configuration;
        private readonly IAssetRepository _assetRepository;

        public AssetService(
            IAmazonS3 s3Client,
            IConfiguration configuration,
            IAssetRepository assetRepository
        )
        {
            _s3Client = s3Client;
            _configuration = configuration;
            _assetRepository = assetRepository;
        }

        public async Task<Guid> UploadAsset(IFormFile file)
        {
            // Generate a unique identifier for the asset
            Guid assetId = Guid.NewGuid();

            // Specify the S3 bucket and key for the uploaded file
            string? bucketName = _configuration.GetValue<string>("UploadBucketName");
            string key = $"assets/{assetId}/{file.FileName}";

            // Upload s3 object as publicly accessible
            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                InputStream = file.OpenReadStream(),
                ContentType = file.ContentType,
                CannedACL = S3CannedACL.PublicRead
            };

            // Upload using putRequest
            await _s3Client.PutObjectAsync(putRequest);

            // Get the url of the uploaded file            
            var url = $"https://{bucketName}.s3.amazonaws.com/{key}";
            
            await _assetRepository.AddAsset(new Entities.Asset
            {
                Id = assetId,
                Name = file.FileName,
                UserId = Guid.Parse("00000000-0000-0000-0000-000000000000"), //TODO: This
                Url = url
            });

            return assetId;
        }

        public async Task<string?> GetAssetUrl(Guid assetId)
        {
            var result = await _assetRepository.GetAsset(assetId);
            return result?.Url;
        }
    }

    /// <summary>
    /// Represents a service for managing assets.
    /// </summary>
    public interface IAssetService
    {
        /// <summary>
        /// Uploads an asset.
        /// </summary>
        /// <returns>The unique identifier of the uploaded asset.</returns>
        Task<Guid> UploadAsset(IFormFile file);

        /// <summary>
        /// Retrieves an asset by its identifier.
        /// </summary>
        /// <param name="assetId">The unique identifier of the asset.</param>
        /// <returns>Asset URL</returns>
        Task<string?> GetAssetUrl(Guid assetId);
    }
}