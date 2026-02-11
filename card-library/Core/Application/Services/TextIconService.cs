using card_library.Core.Application.Models;
using card_library.Core.Application.Models.DTO.Request;
using card_library.Core.Application.Models.DTO.Response;
using card_library.Core.Application.Repository.Contracts;
using card_library.Core.Application.Services.Contracts;
using card_library.Core.Utils;

namespace card_library.Core.Application.Services
{
    public class TextIconService : ITextIconService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITextIconRepository _textIconRepository;
        private readonly IFileStorageService _fileStorageService;

        public TextIconService(ITextIconRepository textIconRepository, IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
        {
            _textIconRepository = textIconRepository;
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
        }

        public async Task<Result<NewTextIconResponse>> CreateIcon(NewTextIconRequest request, CancellationToken ct)
        {
            var existing = await _textIconRepository.GetByName(request.Name, ct);
            if (existing != null)
            {
                return Result<NewTextIconResponse>.Fail("An icon with this name already exists.");
            }

            var textIcon = new TextIcon
            {
                Name = request.Name
            };

            await _textIconRepository.AddAsync(textIcon, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            var response = new NewTextIconResponse
            {
                Id = textIcon.Id,
                Name = textIcon.Name
            };

            return Result<NewTextIconResponse>.Ok(response);
        }

        public async Task<Result<List<TextIconResponse>>> GetAllIcons(CancellationToken ct)
        {
            var icons = await _textIconRepository.GetAll(ct);

            var responses = new List<TextIconResponse>();
            foreach (var icon in icons)
            {
                responses.Add(new TextIconResponse
                {
                    Id = icon.Id,
                    Name = icon.Name,
                    PublicImgUrl = await ResolvePresignedUrl(icon.ImageRefUrl, ct)
                });
            }

            return Result<List<TextIconResponse>>.Ok(responses);
        }

        public async Task<Result<TextIconResponse>> GetIconById(Guid iconId, CancellationToken ct)
        {
            var icon = await _textIconRepository.GetById(iconId, ct);
            if (icon == null)
            {
                return Result<TextIconResponse>.Fail("Icon not found.");
            }

            var response = new TextIconResponse
            {
                Id = icon.Id,
                Name = icon.Name,
                PublicImgUrl = await ResolvePresignedUrl(icon.ImageRefUrl, ct)
            };

            return Result<TextIconResponse>.Ok(response);
        }

        public async Task<Result<string>> UploadImage(Guid iconId, string s3Key, CancellationToken ct)
        {
            var icon = await _textIconRepository.GetById(iconId, ct);
            if (icon == null)
            {
                return Result<string>.Fail("Icon not found.");
            }

            icon.ImageRefUrl = s3Key;
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<string>.Ok(s3Key);
        }

        public async Task<Result<string>> DeleteIcon(Guid iconId, CancellationToken ct)
        {
            var icon = await _textIconRepository.GetById(iconId, ct);
            if (icon == null)
            {
                return Result<string>.Fail("Icon not found.");
            }

            var s3Key = icon.ImageRefUrl;
            _textIconRepository.Delete(icon);
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
