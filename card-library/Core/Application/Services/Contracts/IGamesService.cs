using card_library.Core.Application.Models.DTO.Request;
using card_library.Core.Application.Models.DTO.Response;
using card_library.Core.Utils;

namespace card_library.Core.Application.Services.Contracts
{
    public interface IGamesService
    {
        Task<Result<GameResponse>> GetGameById(Guid game_id, CancellationToken ct);
        Task<Result<List<GameResponse>>> GetGamesByName(string game_name, CancellationToken ct);
        Task<Result<NewGameResponse>> CreateGameById(NewGameRequest newGameRequest, CancellationToken ct);
        Task<Result<NewGameResponse>> EditGameContent(NewGameRequest newGameResponse, CancellationToken ct);
        Task<Result<string>> UploadImage(Guid gameId, string s3Key, CancellationToken ct);
        Task<Result<string>> DeleteImage(Guid gameId, CancellationToken ct);
    }
}
