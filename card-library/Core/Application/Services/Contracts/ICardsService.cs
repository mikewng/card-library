using card_library.Core.Application.Models;
using card_library.Core.Application.Models.DTO.Request;
using card_library.Core.Application.Models.DTO.Response;
using card_library.Core.Utils;

namespace card_library.Core.Application.Services.Contracts
{
    public interface ICardsService
    {
        Task<Result<CardResponse>> GetCardById(Guid card_id, CancellationToken ct);
        Task<Result<List<CardResponse>>> GetCardsByName(string card_name, CancellationToken ct);
        Task<Result<List<CardResponse>>> GetCardsByDeckId(Guid deck_id, CancellationToken ct);
        Task<Result<List<CardResponse>>> GetCardsByTags(List<CardTag> tag_list, CancellationToken ct);
        Task<Result<List<CardResponse>>> GetCardsByGameId(Guid game_id, CancellationToken ct);
        Task<Result<NewCardResponse>> CreateCard(NewCardRequest newCardRequest, CancellationToken ct);
        Task<Result<string>> UploadImage(Guid cardId, string s3Key, CancellationToken ct);
        Task<Result<string>> DeleteImage(Guid cardId, CancellationToken ct);
    }
}
