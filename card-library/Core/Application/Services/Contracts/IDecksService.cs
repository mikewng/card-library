using card_library.Core.Application.Models.DTO.Request;
using card_library.Core.Application.Models.DTO.Response;
using card_library.Core.Infrastructure.Utils;


namespace card_library.Core.Application.Services.Contracts
{
    public interface IDecksService
    {
        Task<Result<DeckResponse>> GetDeckById(Guid deck_id);
        Task<Result<NewDeckResponse>> CreateDeckById(NewDeckRequest newDeckRequest);
        Task<Result<NewDeckResponse>> UpdateDeckById(ExistingDeckRequest existingDeckRequest);

    }
}
