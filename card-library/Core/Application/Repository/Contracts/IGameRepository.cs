using card_library.Core.Application.Models;
using card_library.Core.Application.Models.DTO.Patches;

namespace card_library.Core.Application.Repository.Contracts
{
    public interface IGameRepository
    {
        Task AddAsync(Game game, CancellationToken ct = default);
        Task<Game?> GetById(Guid id, CancellationToken ct = default);
        Task<List<Game>> GetListByName(string name, CancellationToken ct = default);
        Task<bool> UpdateGame(GamePatch gamePatch, CancellationToken ct = default);
    }
}
