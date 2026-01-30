using card_library.Core.Application.Models;
using card_library.Core.Application.Models.DTO.Patches;

namespace card_library.Core.Application.Repository.Contracts
{
    public interface ICardRepository
    {
        Task AddAsync(Card card, CancellationToken ct = default);
        Task<Card?> GetById(Guid id, CancellationToken ct = default);
        Task<List<Card>> GetListByName(string name, CancellationToken ct = default);
        Task<List<Card>> GetListByIds(List<Guid> cardIds, CancellationToken ct = default);
        Task<bool> UpdateCard(CardPatch cardPatch, CancellationToken ct = default);
        Task<CardSection> UpdateCardSection();
        Task<CardSection> DeleteCardSection();
    }
}
