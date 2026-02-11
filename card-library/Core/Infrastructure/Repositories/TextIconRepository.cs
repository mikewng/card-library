using card_library.Core.Application.Models;
using card_library.Core.Application.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace card_library.Core.Infrastructure.Repositories
{
    public class TextIconRepository : Repository<TextIcon>, ITextIconRepository
    {
        public TextIconRepository(AppDbContext context) : base(context) { }

        public async Task AddAsync(TextIcon textIcon, CancellationToken ct = default)
        {
            await DbSet.AddAsync(textIcon, ct);
        }

        public async Task<TextIcon?> GetById(Guid id, CancellationToken ct = default)
        {
            return await DbSet.FirstOrDefaultAsync(t => t.Id == id, ct);
        }

        public async Task<List<TextIcon>> GetAll(CancellationToken ct = default)
        {
            return await DbSet.ToListAsync(ct);
        }

        public async Task<TextIcon?> GetByName(string name, CancellationToken ct = default)
        {
            return await DbSet.FirstOrDefaultAsync(t => t.Name == name, ct);
        }

        public void Delete(TextIcon textIcon)
        {
            DbSet.Remove(textIcon);
        }
    }
}
