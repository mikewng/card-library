using card_library.Core.Application.Models;
using card_library.Core.Application.Models.DTO.Patches;
using card_library.Core.Application.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace card_library.Core.Infrastructure.Repositories
{
    public class GameRepository: Repository<Game>, IGameRepository
    {

        public GameRepository(AppDbContext context) : base(context) { }

        public async Task AddAsync(Game game, CancellationToken ct = default)
        {
            await DbSet.AddAsync(game, ct);
        }

        public async Task<Game?> GetById(Guid id, CancellationToken ct = default)
        {
            return await DbSet.FirstOrDefaultAsync(g => g.Id == id, ct);
        }

        public async Task<List<Game>> GetListByName(string name, CancellationToken ct = default)
        {
            return await DbSet.Where(g => g.GameName.ToLower() == name.ToLower()).ToListAsync(ct);
        }

        public async Task<bool> UpdateGame(GamePatch gamePatch, CancellationToken ct = default)
        {
            var existing = await DbSet.FirstOrDefaultAsync(g => g.Id == gamePatch.Id, ct);
            if (existing is null) return false;

            existing.GameName = gamePatch.GameName;
            existing.Description = gamePatch.Description;
            existing.ImageRefUrl = gamePatch.ImageRefUrl;

            return true;
        }
    }
}
