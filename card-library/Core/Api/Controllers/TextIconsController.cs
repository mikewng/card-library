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
    public class TextIconsController : Controller
    {
        private readonly ILogger<TextIconsController> _logger;
        private readonly ITextIconService _textIconService;
        private readonly IFileStorageService _fileStorageService;

        public TextIconsController(ILogger<TextIconsController> logger, ITextIconService textIconService, IFileStorageService fileStorageService)
        {
            _logger = logger;
            _textIconService = textIconService;
            _fileStorageService = fileStorageService;
        }

        [HttpPost("create", Name = "CreateTextIcon")]
        public async Task<ActionResult<Result<NewTextIconResponse>>> Create([FromBody] NewTextIconRequest request, CancellationToken ct)
        {
            var result = await _textIconService.CreateIcon(request, ct);
            if (!result.Success || result.Value == null)
            {
                return BadRequest(Result<NewTextIconResponse>.Fail(result.Error ?? "Failed to create icon."));
            }

            return Ok(Result<NewTextIconResponse>.Ok(result.Value));
        }

        [HttpGet(Name = "GetTextIcons")]
        public async Task<ActionResult<Result<List<TextIconResponse>>>> GetAll(CancellationToken ct)
        {
            var result = await _textIconService.GetAllIcons(ct);
            if (!result.Success || result.Value == null)
            {
                return Result<List<TextIconResponse>>.Fail("Failed to retrieve icons.");
            }

            return Result<List<TextIconResponse>>.Ok(result.Value);
        }

        [HttpGet("id/{iconId:guid}", Name = "GetTextIconById")]
        public async Task<ActionResult<Result<TextIconResponse>>> GetById(Guid iconId, CancellationToken ct)
        {
            var result = await _textIconService.GetIconById(iconId, ct);
            if (!result.Success || result.Value == null)
            {
                return NotFound(Result<TextIconResponse>.Fail("Could not find icon by given id."));
            }

            return Ok(Result<TextIconResponse>.Ok(result.Value));
        }

        [HttpPost("{iconId:guid}/upload-image", Name = "UploadTextIconImage")]
        [RequestSizeLimit(2 * 1024 * 1024)]
        public async Task<ActionResult<Result<string>>> UploadImage(Guid iconId, IFormFile file, CancellationToken ct)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(Result<string>.Fail("No file uploaded"));
                }

                var allowedContentTypes = new[] { "image/png" };
                if (!allowedContentTypes.Contains(file.ContentType.ToLower()))
                {
                    return BadRequest(Result<string>.Fail("Invalid file type. Only PNG images are allowed."));
                }

                const long maxFileSize = 2 * 1024 * 1024;
                if (file.Length > maxFileSize)
                {
                    return BadRequest(Result<string>.Fail("File size exceeds 2MB limit"));
                }

                using var stream = file.OpenReadStream();
                var fileKey = await _fileStorageService.UploadFileAsync(
                    stream,
                    file.FileName,
                    file.ContentType,
                    folder: "icons/images",
                    ct: ct
                );

                var linkResult = await _textIconService.UploadImage(iconId, fileKey, ct);
                if (!linkResult.Success)
                {
                    await _fileStorageService.DeleteFileAsync(fileKey, ct);
                    return NotFound(Result<string>.Fail(linkResult.Error ?? "Icon not found."));
                }

                var presignedUrl = await _fileStorageService.GetPresignedUrlAsync(fileKey, ct: ct);

                _logger.LogInformation("Successfully uploaded icon image with key: {FileKey} for icon: {IconId}", fileKey, iconId);

                return Ok(Result<string>.Ok(presignedUrl));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading icon image");
                return StatusCode(500, Result<string>.Fail("Failed to upload image"));
            }
        }

        [HttpDelete("{iconId:guid}", Name = "DeleteTextIcon")]
        public async Task<ActionResult<Result>> Delete(Guid iconId, CancellationToken ct)
        {
            try
            {
                var deleteResult = await _textIconService.DeleteIcon(iconId, ct);
                if (!deleteResult.Success || deleteResult.Value == null)
                {
                    return NotFound(Result.Fail(deleteResult.Error ?? "Icon not found."));
                }

                if (!string.IsNullOrEmpty(deleteResult.Value))
                {
                    var deleted = await _fileStorageService.DeleteFileAsync(deleteResult.Value, ct);
                    if (!deleted)
                    {
                        _logger.LogWarning("S3 file not found for icon {IconId}, but icon was deleted", iconId);
                    }
                }

                _logger.LogInformation("Successfully deleted icon: {IconId}", iconId);

                return Ok(Result.Ok());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting icon");
                return StatusCode(500, Result.Fail("Failed to delete icon"));
            }
        }
    }
}
