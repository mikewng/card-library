using card_library.Core.Application.Models;

namespace card_library.Core.Application.Repository.Contracts
{
    public interface IGameRepository
    {
        Task AddAsync(Game game, CancellationToken ct = default);
    }
}
