using card_library.Core.Application.Models;
using card_library.Core.Application.Repository.Contracts;

namespace card_library.Core.Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        public Task AddAsync(Game game, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
