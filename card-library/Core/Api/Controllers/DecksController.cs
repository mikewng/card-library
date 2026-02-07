using card_library.Core.Application.Models.DTO.Request;
using card_library.Core.Application.Models.DTO.Response;
using card_library.Core.Application.Services.Contracts;
using card_library.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace card_library.Core.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DecksController : Controller
    {
        private readonly ILogger<DecksController> _logger;
        private readonly IDecksService _decksService;
        private readonly IFileStorageService _fileStorageService;

        public DecksController(
            ILogger<DecksController> logger,
            IDecksService decksService,
            IFileStorageService fileStorageService)
        {
            _logger = logger;
            _decksService = decksService;
            _fileStorageService = fileStorageService;
        }

        [HttpGet("id/{deckId:guid}", Name = "GetDeckById")]
        public async Task<ActionResult<Result<DeckResponse>>> GetById(Guid deckId)
        {
            var deck = await _decksService.GetDeckById(deckId);
            if (!deck.Success || deck.Value == null)
            {
                return Result<DeckResponse>.Fail("Could not find deck by given id.");
            }

            return Result<DeckResponse>.Ok(deck.Value);
        }

        [HttpGet("name/{deckName}", Name = "GetDecksByName")]
        public async Task<ActionResult<Result<List<DeckResponse>>>> GetByName(string deckName)
        {
            var deckList = await _decksService.GetDecksByName(deckName);
            if (!deckList.Success || deckList.Value == null)
            {
                return Result<List<DeckResponse>>.Fail("Could not find deck by given id.");
            }

            return Result<List<DeckResponse>>.Ok(deckList.Value);
        }

        [HttpPost("create", Name = "CreateDeck")]
        public async Task<ActionResult<Result<NewDeckResponse>>> Create([FromBody] NewDeckRequest newDeckRequest)
        {
            var deckResult = await _decksService.CreateDeck(newDeckRequest);
            if (!deckResult.Success || deckResult.Value == null)
            {
                return Result<NewDeckResponse>.Fail("Failed to create specified deck.");
            }

            return Result<NewDeckResponse>.Ok(deckResult.Value);
        }

        [HttpPut("edit", Name = "Edit")]
        public async Task<ActionResult<Result<NewDeckResponse>>> Update([FromBody] ExistingDeckRequest existingDeckRequest)
        {
            var deckResult = await _decksService.UpdateDeckById(existingDeckRequest);
            if (!deckResult.Success || deckResult.Value == null)
            {
                return Result<NewDeckResponse>.Fail("Failed to update specified deck.");
            }

            return Result<NewDeckResponse>.Ok(deckResult.Value);
        }

        [HttpPost("{deckId:guid}/upload-image", Name = "UploadDeckImage")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<ActionResult<Result<string>>> UploadImage(Guid deckId, IFormFile file, CancellationToken ct)
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
                var fileKey = await _fileStorageService.UploadFileAsync(
                    stream,
                    file.FileName,
                    file.ContentType,
                    folder: "decks/images",
                    ct: ct
                );

                var linkResult = await _decksService.UploadImage(deckId, fileKey, ct);
                if (!linkResult.Success)
                {
                    await _fileStorageService.DeleteFileAsync(fileKey, ct);
                    return NotFound(Result<string>.Fail(linkResult.Error ?? "Deck not found."));
                }

                var presignedUrl = await _fileStorageService.GetPresignedUrlAsync(fileKey, ct: ct);

                _logger.LogInformation("Successfully uploaded deck image with key: {FileKey} for deck: {DeckId}", fileKey, deckId);

                return Ok(Result<string>.Ok(presignedUrl));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading deck image");
                return StatusCode(500, Result<string>.Fail("Failed to upload image"));
            }
        }

        [HttpDelete("{deckId:guid}/delete-image", Name = "DeleteDeckImage")]
        public async Task<ActionResult<Result>> DeleteImage(Guid deckId, CancellationToken ct)
        {
            try
            {
                var deleteResult = await _decksService.DeleteImage(deckId, ct);
                if (!deleteResult.Success || deleteResult.Value == null)
                {
                    return NotFound(Result.Fail(deleteResult.Error ?? "Deck not found or has no image."));
                }

                var deleted = await _fileStorageService.DeleteFileAsync(deleteResult.Value, ct);
                if (!deleted)
                {
                    _logger.LogWarning("S3 file not found for deck {DeckId}, but ImageRefUrl was cleared", deckId);
                }

                _logger.LogInformation("Successfully deleted deck image for deck: {DeckId}", deckId);

                return Ok(Result.Ok());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting deck image");
                return StatusCode(500, Result.Fail("Failed to delete image"));
            }
        }
    }
}
