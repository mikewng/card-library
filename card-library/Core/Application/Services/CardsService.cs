using card_library.Core.Application.Models;
using card_library.Core.Application.Models.DTO.Request;
using card_library.Core.Application.Models.DTO.Response;
using card_library.Core.Application.Services.Contracts;
using card_library.Core.Utils;

namespace card_library.Core.Application.Services
{
    public class CardsService : ICardsService
    {
        public Task<Result<NewCardResponse>> CreateCard(NewCardRequest newCardRequest)
        {
            throw new NotImplementedException();
        }

        public Task<Result<CardResponse>> GetCardById(Guid card_id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<CardResponse>>> GetCardsByName(string card_name)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<CardResponse>>> GetCardsByDeckId(Guid deck_id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<CardResponse>>> GetCardsByGameId(Guid game_id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<CardResponse>>> GetCardsByTags(List<CardTag> tag_list)
        {
            throw new NotImplementedException();
        }
    }
}
