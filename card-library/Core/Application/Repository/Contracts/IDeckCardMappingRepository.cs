using card_library.Core.Application.Models;
using card_library.Core.Application.Models.DTO.Patches;

namespace card_library.Core.Application.Repository.Contracts
{
    public interface IDeckCardMappingRepository
    {
        Task AddAsync(DeckCardMapping deckCardMapping, CancellationToken ct = default);
        Task<DeckCardMapping?> GetById(Guid id, CancellationToken ct = default);
        Task<List<DeckCardMapping>> GetListByDeckId(Guid deckId, CancellationToken ct = default);

    }
}
