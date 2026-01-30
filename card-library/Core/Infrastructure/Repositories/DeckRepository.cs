using card_library.Core.Application.Models;
using card_library.Core.Application.Models.DTO.Patches;
using card_library.Core.Application.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace card_library.Core.Infrastructure.Repositories
{
    public class DeckRepository: Repository<Deck>, IDeckRepository
    {
        public DeckRepository(AppDbContext context) : base(context) { }

        public async Task AddAsync(Deck deck, CancellationToken ct = default)
        {
            await DbSet.AddAsync(deck, ct);
        }

        public async Task<Deck?> GetById(Guid id, CancellationToken ct = default)
        {
            return await DbSet.FirstOrDefaultAsync(d => d.Id == id, ct);
        }

        public async Task<List<Deck>> GetListByName(string name, CancellationToken ct = default)
        {
            return await DbSet.Where(d => d.Name.ToLower() == name.ToLower()).ToListAsync(ct);
        }

        public async Task<bool> UpdateDeck(DeckPatch deckPatch, CancellationToken ct = default)
        {
            var existing = await DbSet.FirstOrDefaultAsync(g => g.Id == deckPatch.Id, ct);
            if (existing is null) return false;

            // Update:
            // - Name
            // - Description


            throw new NotImplementedException();
        }
    }
}
