using Amazon.S3;
using Amazon.S3.Model;
using card_library.Core.Application.Models;
using card_library.Core.Application.Services.Contracts;
using Microsoft.Extensions.Options;

namespace card_library.Core.Application.Services
{
    public class S3FileStorageService : IFileStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly AwsS3Settings _settings;

        public S3FileStorageService(IAmazonS3 s3Client, IOptions<AwsS3Settings> settings)
        {
            _s3Client = s3Client;
            _settings = settings.Value;
        }

        public async Task<string> UploadFileAsync(
            Stream stream,
            string fileName,
            string contentType,
            string? folder = null,
            CancellationToken ct = default)
        {
            // Generate unique file name to avoid collisions
            var fileExtension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

            // Construct the full S3 key (path)
            var key = string.IsNullOrWhiteSpace(folder)
                ? uniqueFileName
                : $"{folder.TrimEnd('/')}/{uniqueFileName}";

            var putRequest = new PutObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = key,
                InputStream = stream,
                ContentType = contentType
            };

            try
            {
                var response = await _s3Client.PutObjectAsync(putRequest, ct);

                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception($"Failed to upload file to S3. Status: {response.HttpStatusCode}");
                }

                // Return the S3 key - callers should use GetPresignedUrlAsync for access URLs
                return key;
            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception($"S3 Error uploading file: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteFileAsync(string fileUrl, CancellationToken ct = default)
        {
            try
            {
                // Extract the S3 key from the URL
                var key = ExtractKeyFromUrl(fileUrl);

                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = _settings.BucketName,
                    Key = key
                };

                var response = await _s3Client.DeleteObjectAsync(deleteRequest, ct);
                return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent ||
                       response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (AmazonS3Exception)
            {
                return false;
            }
        }

        public async Task<string> GetPresignedUrlAsync(
            string fileKey,
            int expirationMinutes = 60,
            CancellationToken ct = default)
        {
            try
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _settings.BucketName,
                    Key = fileKey,
                    Expires = DateTime.UtcNow.AddMinutes(expirationMinutes)
                };

                // GetPreSignedURL is synchronous in AWS SDK
                var url = await Task.Run(() => _s3Client.GetPreSignedURL(request), ct);
                return url;
            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception($"S3 Error generating presigned URL: {ex.Message}", ex);
            }
        }

        public async Task<bool> FileExistsAsync(string fileKey, CancellationToken ct = default)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = _settings.BucketName,
                    Key = fileKey
                };

                await _s3Client.GetObjectMetadataAsync(request, ct);
                return true;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        /// <summary>
        /// Constructs the public URL for a file in S3
        /// Uses CloudFront URL if configured, otherwise S3 direct URL
        /// </summary>
        private string GetFileUrl(string key)
        {
            if (!string.IsNullOrWhiteSpace(_settings.CloudFrontUrl))
            {
                return $"{_settings.CloudFrontUrl.TrimEnd('/')}/{key}";
            }

            return $"https://{_settings.BucketName}.s3.{_settings.Region}.amazonaws.com/{key}";
        }

        /// <summary>
        /// Extracts the S3 key from a full URL
        /// Handles S3 direct URLs, CloudFront URLs, and presigned URLs
        /// </summary>
        private string ExtractKeyFromUrl(string fileUrl)
        {
            // Strip query parameters (for presigned URLs)
            var urlWithoutQuery = fileUrl.Split('?')[0];

            // Handle CloudFront URL
            if (!string.IsNullOrWhiteSpace(_settings.CloudFrontUrl) &&
                urlWithoutQuery.StartsWith(_settings.CloudFrontUrl, StringComparison.OrdinalIgnoreCase))
            {
                return urlWithoutQuery.Substring(_settings.CloudFrontUrl.TrimEnd('/').Length + 1);
            }

            // Handle S3 direct URL
            var s3BaseUrl = $"https://{_settings.BucketName}.s3.{_settings.Region}.amazonaws.com/";
            if (urlWithoutQuery.StartsWith(s3BaseUrl, StringComparison.OrdinalIgnoreCase))
            {
                return urlWithoutQuery.Substring(s3BaseUrl.Length);
            }

            // If it's already just a key, return it
            return urlWithoutQuery;
        }
    }
}
