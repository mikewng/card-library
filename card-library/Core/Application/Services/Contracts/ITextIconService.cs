using card_library.Core.Application.Models.DTO.Request;
using card_library.Core.Application.Models.DTO.Response;
using card_library.Core.Utils;

namespace card_library.Core.Application.Services.Contracts
{
    public interface ITextIconService
    {
        Task<Result<NewTextIconResponse>> CreateIcon(NewTextIconRequest request, CancellationToken ct);
        Task<Result<List<TextIconResponse>>> GetAllIcons(CancellationToken ct);
        Task<Result<TextIconResponse>> GetIconById(Guid iconId, CancellationToken ct);
        Task<Result<string>> UploadImage(Guid iconId, string s3Key, CancellationToken ct);
        Task<Result<string>> DeleteIcon(Guid iconId, CancellationToken ct);
    }
}
