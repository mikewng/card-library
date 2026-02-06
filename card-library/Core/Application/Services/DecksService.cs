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
        private readonly IFileStorageService _fileStorageService;

        public DecksService(IDeckRepository deckRepository, IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
        {
            _deck = deckRepository;
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
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
                PublicImgUrl = await ResolvePresignedUrl(deckResult.ImageRefUrl)
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

        public async Task<Result<string>> UploadImage(Guid deckId, string s3Key, CancellationToken ct)
        {
            var deck = await _deck.GetById(deckId, ct);
            if (deck == null)
            {
                return Result<string>.Fail("Deck not found.");
            }

            deck.ImageRefUrl = s3Key;
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<string>.Ok(s3Key);
        }

        public async Task<Result<string>> DeleteImage(Guid deckId, CancellationToken ct)
        {
            var deck = await _deck.GetById(deckId, ct);
            if (deck == null)
            {
                return Result<string>.Fail("Deck not found.");
            }

            if (string.IsNullOrEmpty(deck.ImageRefUrl))
            {
                return Result<string>.Fail("Deck has no image to delete.");
            }

            var s3Key = deck.ImageRefUrl;
            deck.ImageRefUrl = string.Empty;
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<string>.Ok(s3Key);
        }

        private async Task<string> ResolvePresignedUrl(string imageRefUrl, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(imageRefUrl))
                return string.Empty;

            return await _fileStorageService.GetPresignedUrlAsync(imageRefUrl, ct: ct);
        }
    }
}
