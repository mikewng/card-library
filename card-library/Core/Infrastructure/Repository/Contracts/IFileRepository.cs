using card_library.Core.Infrastructure.Utils;

namespace card_library.Core.Infrastructure.Repository.Contracts
{
    public interface IFileRepository
    {
        Task<Result<string>> SaveToFileStorage();
        Task<Result<string>> GenerateLinkToFile();
    }
}
