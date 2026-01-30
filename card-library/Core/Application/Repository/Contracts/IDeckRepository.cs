using card_library.Core.Application.Models;
using card_library.Core.Application.Models.DTO.Patches;

namespace card_library.Core.Application.Repository.Contracts
{
    public interface IDeckRepository
    {
        Task AddAsync(Deck deck, CancellationToken ct = default);
        Task<Deck?> GetById(Guid id, CancellationToken ct = default);
        Task<List<Deck>> GetListByName(string name, CancellationToken ct = default);
        Task<bool> UpdateDeck(DeckPatch deckPatch, CancellationToken ct = default);
    }
}
