namespace card_library.Core.Application.Services.Contracts
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(Stream stream,string fileName,string contentType,string? folder = null,CancellationToken ct = default);
        Task<bool> DeleteFileAsync(string fileUrl, CancellationToken ct = default);
        Task<string> GetPresignedUrlAsync(string fileKey,int expirationMinutes = 60,CancellationToken ct = default);
        Task<bool> FileExistsAsync(string fileKey, CancellationToken ct = default);
    }
}
