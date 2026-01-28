using card_library.Core.Application.Models.DTO.Request;
using card_library.Core.Application.Models.DTO.Response;
using card_library.Core.Utils;


namespace card_library.Core.Application.Services.Contracts
{
    public interface IDecksService
    {
        Task<Result<DeckResponse>> GetDeckById(Guid deck_id);
        Task<Result<List<DeckResponse>>> GetDecksByName(string deckName);
        Task<Result<NewDeckResponse>> CreateDeck(NewDeckRequest newDeckRequest);
        Task<Result<NewDeckResponse>> UpdateDeckById(ExistingDeckRequest existingDeckRequest);

    }
}
