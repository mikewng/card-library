using card_library.Core.Application.Models.DTO.Request;
using card_library.Core.Application.Models.DTO.Response;
using card_library.Core.Infrastructure.Utils;

namespace card_library.Core.Application.Services.Contracts
{
    public interface IGamesService
    {
        Task<Result<GameResponse>> GetGameById(Guid game_id);
        Task<Result<List<GameResponse>>> GetGamesByName(string game_name);
        Task<Result<NewGameResponse>> CreateGameById(NewGameRequest newGameRequest);
    }
}
