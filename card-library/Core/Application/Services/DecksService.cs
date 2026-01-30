using card_library.Core.Application.Models.DTO.Request;
using card_library.Core.Application.Models.DTO.Response;
using card_library.Core.Application.Repository.Contracts;
using card_library.Core.Application.Services.Contracts;
using card_library.Core.Utils;

namespace card_library.Core.Application.Services
{
    public class DecksService : IDecksService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDeckRepository _deck;

        public DecksService(IDeckRepository deckRepository, IUnitOfWork unitOfWork)
        {
            _deck = deckRepository;
            _unitOfWork = unitOfWork;
        }

        public Task<Result<NewDeckResponse>> CreateDeck(NewDeckRequest newDeckRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<DeckResponse>> GetDeckById(Guid deck_id)
        {
            var deckResult = await _deck.GetById(deck_id);
            if (deckResult == null)
            {
                return Result<DeckResponse>.Fail("Failed to get deck of associated ID.");
            }

            DeckResponse deckResponse = new DeckResponse
            {
                DeckId = deck_id,
                DeckName = deckResult.Name,
                DeckDescription = deckResult.Description,
                PublicImgUrl = deckResult.ImageRefUrl
            };

            return Result<DeckResponse>.Ok(deckResponse);

        }

        public Task<Result<List<DeckResponse>>> GetDecksByName(string deckName)
        {
            throw new NotImplementedException();
        }

        public Task<Result<NewDeckResponse>> UpdateDeckById(ExistingDeckRequest existingDeckRequest)
        {
            throw new NotImplementedException();
        }
    }
}
