using card_library.Core.Application.Models;
using card_library.Core.Application.Models.DTO.Request;
using card_library.Core.Application.Models.DTO.Response;
using card_library.Core.Infrastructure.Utils;

namespace card_library.Core.Application.Services.Contracts
{
    public interface ICardsService
    {
        Task<Result<CardResponse>> GetCardById(Guid card_id);
        Task<Result<CardResponse>> GetCardByName(string card_name);
        Task<Result<List<CardResponse>>> GetCardsByDeckId(Guid deck_id);
        Task<Result<List<CardResponse>>> GetCardsByTags(List<CardTag> tag_list);
        Task<Result<List<CardResponse>>> GetCardsByGameId(Guid game_id);
        Task<Result<NewCardResponse>> CreateCard(NewCardRequest newCardRequest);
    }
}
