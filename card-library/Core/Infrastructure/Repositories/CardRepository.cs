using card_library.Core.Application.Models;
using card_library.Core.Application.Models.DTO.Patches;
using card_library.Core.Application.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace card_library.Core.Infrastructure.Repositories
{
    public class CardRepository: Repository<Card>, ICardRepository
    {
        public CardRepository(AppDbContext context) : base(context) { }
        public async Task AddAsync(Card card, CancellationToken ct = default)
        {
            await DbSet.AddAsync(card, ct);
        }

        public async Task<Card?> GetById(Guid id, CancellationToken ct = default)
        {
            return await DbSet.FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task<List<Card>> GetListByName(string name, CancellationToken ct = default)
        {
            return await DbSet.Where(c => c.Name.ToLower() == name.ToLower()).ToListAsync(ct);
        }

        public async Task<List<Card>> GetListByIds(List<Guid> cardIds, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateCard(CardPatch cardPatch, CancellationToken ct = default)
        {
            var existing = await DbSet.FirstOrDefaultAsync(g => g.Id == cardPatch.Id, ct);
            if (existing is null) return false;

            return true;
        }

        public async Task<CardSection> UpdateCardSection()
        {
            return new CardSection();
        }

        public async Task<CardSection> DeleteCardSection()
        {
            return new CardSection();
        }

    }
}
