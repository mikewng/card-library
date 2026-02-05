namespace card_library.Core.Application.Models
{
    public class AwsS3Settings
    {
        public const string SectionName = "AwsS3";
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
        public string? CloudFrontUrl { get; set; }
        public bool MakeFilesPublic { get; set; } = true;
    }
}
