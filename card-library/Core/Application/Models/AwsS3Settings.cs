namespace card_library.Core.Application.Models
{
    /// <summary>
    /// Configuration settings for AWS S3
    /// Bind this from appsettings.json
    /// </summary>
    public class AwsS3Settings
    {
        public const string SectionName = "AwsS3";

        /// <summary>
        /// AWS Access Key ID
        /// </summary>
        public string AccessKey { get; set; } = string.Empty;

        /// <summary>
        /// AWS Secret Access Key
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// AWS Region (e.g., "us-east-1", "eu-west-1")
        /// </summary>
        public string Region { get; set; } = string.Empty;

        /// <summary>
        /// S3 Bucket name where files will be stored
        /// </summary>
        public string BucketName { get; set; } = string.Empty;

        /// <summary>
        /// Optional: CloudFront distribution URL for CDN delivery
        /// If provided, will use this instead of S3 direct URL
        /// </summary>
        public string? CloudFrontUrl { get; set; }

        /// <summary>
        /// Whether uploaded files should be publicly accessible
        /// Default: true (for game images)
        /// </summary>
        public bool MakeFilesPublic { get; set; } = true;
    }
}
