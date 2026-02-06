using card_library.Core.Application.Models;
using card_library.Core.Application.Models.DTO.Request;
using card_library.Core.Application.Models.DTO.Response;
using card_library.Core.Application.Repository.Contracts;
using card_library.Core.Application.Services.Contracts;
using card_library.Core.Utils;

namespace card_library.Core.Application.Services
{
    public class CardsService : ICardsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICardRepository _card;
        private readonly IFileStorageService _fileStorageService;

        public CardsService(ICardRepository cardRepository, IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
        {
            _card = cardRepository;
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
        }
        public Task<Result<NewCardResponse>> CreateCard(NewCardRequest newCardRequest, CancellationToken ct)
        {
            Card newCard;
            if (newCardRequest.IsRawImageOnly)
            {
                newCard = new Card
                {
                    Name = newCardRequest.CardTitle,
                    IsRawCardImage = true,
                    CardTags = newCardRequest.Tags
                };
            } else
            {
                newCard = new Card
                {
                    Name = newCardRequest.CardTitle,
                    HexCardColor = newCardRequest.HexCardColor,
                    IsRawCardImage = false,
                    CardSections = newCardRequest.Sections,
                    CardTags = newCardRequest.Tags
                };
            }

            _card.AddAsync(newCard, ct);
            _unitOfWork.SaveChangesAsync(ct);
                
            throw new NotImplementedException();
        }

        public Task<Result<CardResponse>> GetCardById(Guid card_id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<CardResponse>>> GetCardsByName(string card_name, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<CardResponse>>> GetCardsByDeckId(Guid deck_id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<CardResponse>>> GetCardsByGameId(Guid game_id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<CardResponse>>> GetCardsByTags(List<CardTag> tag_list, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<string>> UploadImage(Guid cardId, string s3Key, CancellationToken ct)
        {
            var card = await _card.GetById(cardId, ct);
            if (card == null)
            {
                return Result<string>.Fail("Card not found.");
            }

            card.ImageRefUrl = s3Key;
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<string>.Ok(s3Key);
        }

        public async Task<Result<string>> DeleteImage(Guid cardId, CancellationToken ct)
        {
            var card = await _card.GetById(cardId, ct);
            if (card == null)
            {
                return Result<string>.Fail("Card not found.");
            }

            if (string.IsNullOrEmpty(card.ImageRefUrl))
            {
                return Result<string>.Fail("Card has no image to delete.");
            }

            var s3Key = card.ImageRefUrl;
            card.ImageRefUrl = string.Empty;
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<string>.Ok(s3Key);
        }

        private async Task<string> ResolvePresignedUrl(string imageRefUrl, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(imageRefUrl))
                return string.Empty;

            return await _fileStorageService.GetPresignedUrlAsync(imageRefUrl, ct: ct);
        }
    }
}
