using card_library.Core.Application.Models;
using card_library.Core.Application.Models.DTO.Request;
using card_library.Core.Application.Models.DTO.Response;
using card_library.Core.Application.Services;
using card_library.Core.Application.Services.Contracts;
using card_library.Core.Utils;
using Microsoft.AspNetCore.Mvc;

namespace card_library.Core.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CardsController : Controller
    {
        private readonly ILogger<CardsController> _logger;
        private readonly ICardsService _cardsService;
        private readonly IFileStorageService _fileStorageService;

        public CardsController(ILogger<CardsController> logger, ICardsService cardsService, IFileStorageService fileStorageService)
        {
            _logger = logger;
            _cardsService = cardsService;
            _fileStorageService = fileStorageService;
        }

        [HttpGet("id/{cardId:guid}", Name = "GetCardById")]
        public async Task<ActionResult<Result<CardResponse>>> GetById(Guid cardId, CancellationToken ct)
        {
            var card = await _cardsService.GetCardById(cardId, ct);
            if (!card.Success || card.Value == null)
            {
                return Result<CardResponse>.Fail("Could not find card by given id.");
            }

            return Result<CardResponse>.Ok(card.Value);
        }

        [HttpGet("name/{cardName}", Name = "GetCardsByName")]
        public async Task<ActionResult<Result<List<CardResponse>>>> GetByName(string cardName, CancellationToken ct)
        {
            var cardList = await _cardsService.GetCardsByName(cardName, ct);
            if (!cardList.Success || cardList.Value == null)
            {
                return Result<List<CardResponse>>.Fail("Could not find any cards by given name.");
            }

            return Result<List<CardResponse>>.Ok(cardList.Value);
        }

        [HttpGet("deckId/{deckId:guid}", Name = "GetCardsByDeckId")]
        public async Task<Result<List<CardResponse>>> GetByDeckId(Guid deckId, CancellationToken ct)
        {
            var cardList = await _cardsService.GetCardsByDeckId(deckId, ct);
            if (!cardList.Success || cardList.Value == null)
            {
                return Result<List<CardResponse>>.Fail("Could not find any cards of associated deck.");
            }

            return Result<List<CardResponse>>.Ok(cardList.Value);
        }

        [HttpGet("deckTags", Name = "GetCardsByTags")]
        public async Task<Result<List<CardResponse>>> GetByTags([FromBody] TagRequest request, CancellationToken ct)
        {
            var cardList = await _cardsService.GetCardsByTags(request.CardTags, ct);
            if (!cardList.Success || cardList.Value == null)
            {
                return Result<List<CardResponse>>.Fail("Could not find any cards of associated tag(s).");
            }

            return Result<List<CardResponse>>.Ok(cardList.Value);

        }

        [HttpGet("gameId/{gameId:guid}", Name = "GetCardsByGameId")]
        public async Task<Result<List<CardResponse>>> GetByGameId(Guid gameId, CancellationToken ct)
        {
            // Eventually add pagination
            var cardList = await _cardsService.GetCardsByGameId(gameId, ct);
            if (!cardList.Success || cardList.Value == null)
            {
                return Result<List<CardResponse>>.Fail("Could not find any cards of associated game.");
            }

            return Result<List<CardResponse>>.Ok(cardList.Value);
        }

        [HttpPost("create", Name = "CreateCard")]
        public async Task<Result<NewCardResponse>> Create([FromBody] NewCardRequest newCardRequest, CancellationToken ct)
        {
            // Eventually add pagination
            var cardResult = await _cardsService.CreateCard(newCardRequest, ct);
            if (!cardResult.Success || cardResult.Value == null)
            {
                return Result<NewCardResponse>.Fail("Failed to create card.");
            }

            return Result<NewCardResponse>.Ok(cardResult.Value);
        }

        [HttpPost("upload-image", Name = "UploadCardImage")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<ActionResult<Result<string>>> UploadImage(IFormFile file, CancellationToken ct)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(Result<string>.Fail("No file uploaded"));
                }

                var allowedContentTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
                if (!allowedContentTypes.Contains(file.ContentType.ToLower()))
                {
                    return BadRequest(Result<string>.Fail("Invalid file type. Only images are allowed (JPEG, PNG, GIF, WEBP)"));
                }

                const long maxFileSize = 10 * 1024 * 1024;
                if (file.Length > maxFileSize)
                {
                    return BadRequest(Result<string>.Fail("File size exceeds 10MB limit"));
                }

                using var stream = file.OpenReadStream();
                var imageUrl = await _fileStorageService.UploadFileAsync(
                    stream,
                    file.FileName,
                    file.ContentType,
                    folder: "cards/images",
                    ct: ct
                );

                _logger.LogInformation("Successfully uploaded card image: {ImageUrl}", imageUrl);

                return Ok(Result<string>.Ok(imageUrl));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading card image");
                return StatusCode(500, Result<string>.Fail("Failed to upload image"));
            }
        }


        [HttpDelete("delete-image", Name = "DeleteCardImage")]
        public async Task<ActionResult<Result>> DeleteImage([FromQuery] string imageUrl, CancellationToken ct)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageUrl))
                {
                    return BadRequest(Result.Fail("Image URL is required"));
                }

                var deleted = await _fileStorageService.DeleteFileAsync(imageUrl, ct);
                if (!deleted)
                {
                    return NotFound(Result.Fail("Image not found or already deleted"));
                }

                _logger.LogInformation("Successfully deleted card image: {ImageUrl}", imageUrl);

                return Ok(Result.Ok());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting card image");
                return StatusCode(500, Result.Fail("Failed to delete image"));
            }
        }
    }
}
