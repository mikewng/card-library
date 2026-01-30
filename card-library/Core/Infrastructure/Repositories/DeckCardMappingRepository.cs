using card_library.Core.Application.Models;
using card_library.Core.Application.Repository.Contracts;

namespace card_library.Core.Infrastructure.Repositories
{
    public class DeckCardMappingRepository: Repository<DeckCardMapping>, IDeckCardMappingRepository
    {
        public DeckCardMappingRepository(AppDbContext context) : base(context) { }

        public Task AddAsync(DeckCardMapping deckCardMapping, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<DeckCardMapping?> GetById(Guid id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<DeckCardMapping>> GetListByDeckId(Guid deckId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
