using card_library.Core.Utils;

namespace card_library.Core.Application.Repository.Contracts
{
    public interface IFileRepository
    {
        Task<Result<string>> SaveToFileStorage();
        Task<Result<string>> GenerateLinkToFile(string secureToken);
    }
}
