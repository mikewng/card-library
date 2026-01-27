using card_library.Core.Application.Models.DTO.Request;
using card_library.Core.Application.Models.DTO.Response;
using card_library.Core.Application.Services.Contracts;
using card_library.Core.Utils;

namespace card_library.Core.Application.Services
{
    public class DecksService : IDecksService
    {
        public Task<Result<NewDeckResponse>> CreateDeck(NewDeckRequest newDeckRequest)
        {
            throw new NotImplementedException();
        }

        public Task<Result<DeckResponse>> GetDeckById(Guid deck_id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<NewDeckResponse>> UpdateDeckById(ExistingDeckRequest existingDeckRequest)
        {
            throw new NotImplementedException();
        }
    }
}
