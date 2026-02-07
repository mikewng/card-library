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
    public class GamesController : Controller
    {
        private readonly ILogger<GamesController> _logger;
        private readonly IGamesService _gamesService;
        private readonly IFileStorageService _fileStorageService;

        public GamesController(
            ILogger<GamesController> logger,
            IGamesService gamesService,
            IFileStorageService fileStorageService)
        {
            _logger = logger;
            _gamesService = gamesService;
            _fileStorageService = fileStorageService;
        }

        [HttpPost("create", Name = "CreateGame")]
        public async Task<ActionResult<Result>> Create([FromBody] NewGameRequest newGameRequest, CancellationToken ct)
        {
            var res = await _gamesService.CreateGameById(newGameRequest, ct);
            if (!res.Success || res.Value == null)
            {
                return Result.Fail("Could not create this new game.");
            }

            return Result.Ok();
        }

        [HttpGet("get/id/{game_id:guid}", Name = "GetGameById")]
        public async Task<ActionResult<Result<GameResponse>>> GetById(Guid game_id, CancellationToken ct)
        {
            var res = await _gamesService.GetGameById(game_id, ct);
            if (!res.Success || res.Value == null)
            {
                return Result<GameResponse>.Fail("Could not get game of associated ID.");
            }

            return Result<GameResponse>.Ok(res.Value);
        }


        [HttpPost("edit/", Name = "EditGame")]
        public async Task<ActionResult<Result<NewGameResponse>>> Edit([FromBody] NewGameRequest gameEditRequest, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var res = await _gamesService.EditGameContent(gameEditRequest, ct);
            if (!res.Success || res.Value == null)
            {
                return Result<NewGameResponse>.Fail("Failed to edit game details");
            }

            return Result<NewGameResponse>.Ok(res.Value);
        }


        [HttpGet("get/name/{game_name}", Name = "GetGamesByName")]
        public async Task<ActionResult<Result<List<GameResponse>>>> GetByName(string game_name, CancellationToken ct)
        {
            var res = await _gamesService.GetGamesByName(game_name, ct);
            if (!res.Success || res.Value == null)
            {
                return Result<List<GameResponse>>.Fail("Could not get game of associated name.");
            }

            return Result<List<GameResponse>>.Ok(res.Value);
        }

        [HttpPost("{gameId:guid}/upload-image", Name = "UploadGameImage")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<ActionResult<Result<string>>> UploadImage(Guid gameId, IFormFile file, CancellationToken ct)
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
                    folder: "games/images",
                    ct: ct
                );

                var linkResult = await _gamesService.UploadImage(gameId, fileKey, ct);
                if (!linkResult.Success)
                {
                    await _fileStorageService.DeleteFileAsync(fileKey, ct);
                    return NotFound(Result<string>.Fail(linkResult.Error ?? "Game not found."));
                }

                var presignedUrl = await _fileStorageService.GetPresignedUrlAsync(fileKey, ct: ct);

                _logger.LogInformation("Successfully uploaded game image with key: {FileKey} for game: {GameId}", fileKey, gameId);

                return Ok(Result<string>.Ok(presignedUrl));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading game image");
                return StatusCode(500, Result<string>.Fail("Failed to upload image"));
            }
        }

        [HttpDelete("{gameId:guid}/delete-image", Name = "DeleteGameImage")]
        public async Task<ActionResult<Result>> DeleteImage(Guid gameId, CancellationToken ct)
        {
            try
            {
                var deleteResult = await _gamesService.DeleteImage(gameId, ct);
                if (!deleteResult.Success || deleteResult.Value == null)
                {
                    return NotFound(Result.Fail(deleteResult.Error ?? "Game not found or has no image."));
                }

                var deleted = await _fileStorageService.DeleteFileAsync(deleteResult.Value, ct);
                if (!deleted)
                {
                    _logger.LogWarning("S3 file not found for game {GameId}, but ImageRefUrl was cleared", gameId);
                }

                _logger.LogInformation("Successfully deleted game image for game: {GameId}", gameId);

                return Ok(Result.Ok());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting game image");
                return StatusCode(500, Result.Fail("Failed to delete image"));
            }
        }
    }
}
