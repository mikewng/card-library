using card_library.Core.Application.Models;
using card_library.Core.Application.Models.DTO.Request;
using card_library.Core.Application.Models.DTO.Response;
using card_library.Core.Application.Repository.Contracts;
using card_library.Core.Application.Services.Contracts;
using card_library.Core.Utils;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml.Linq;

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

        public async Task<Result<CardResponse>> GetCardById(Guid card_id, CancellationToken ct)
        {
            var cardResult = await _card.GetById(card_id, ct);
            if (cardResult == null)
            {
                return Result<CardResponse>.Fail("Failed to get card with associated ID.");
            }
            CardResponse cardResponse = new CardResponse
            {
                CardId = cardResult.Id,
                CardName = cardResult.Name,
                HexColor = cardResult.HexCardColor,
                PublicImgUrl = await ResolvePresignedUrl(cardResult.ImageRefUrl, ct),
                CardSections = cardResult.CardSections,
                CardTags = cardResult.CardTags
            };

            return Result<CardResponse>.Ok(cardResponse);
        }

        public async Task<Result<List<CardResponse>>> GetCardsByName(string card_name, CancellationToken ct)
        {
            var cardListResult = await _card.GetListByName(card_name, ct);
            if (cardListResult == null)
            {
                return Result<List<CardResponse>>.Fail("Failed to get card(s) with associated name.");
            }

            List<CardResponse> listCardResponses = new List<CardResponse>();
            foreach (Card cardResult in cardListResult)
            {
                if (cardResult != null)
                {
                    CardResponse cardResponse = new CardResponse
                    {
                        CardId = cardResult.Id,
                        CardName = cardResult.Name,
                        HexColor = cardResult.HexCardColor,
                        PublicImgUrl = await ResolvePresignedUrl(cardResult.ImageRefUrl, ct),
                        CardSections = cardResult.CardSections,
                        CardTags = cardResult.CardTags
                    };
                    listCardResponses.Add(cardResponse);
                }
            }

            return Result<List<CardResponse>>.Ok(listCardResponses);
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
