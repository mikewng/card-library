using card_library.Core.Application.Repository.Contracts;
using card_library.Core.Utils;

namespace card_library.Core.Infrastructure.Repositories
{
    public class S3FileStorageRepository : IFileRepository
    {
        public Task<Result<string>> GenerateLinkToFile()
        {
            throw new NotImplementedException();
        }

        public Task<Result<string>> SaveToFileStorage()
        {
            throw new NotImplementedException();
        }
    }
}
