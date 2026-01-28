using card_library.Core.Application.Models.DTO.Response;
using card_library.Core.Utils;
using card_library.Main.Models.DTO.Request;


namespace card_library.Core.Application.Services.Contracts
{
    public interface IAuthService
    {
        public Task<Result<AuthResponse>> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken);
        public Task<Result> RegisterAsync(RegisterRequest registerRequest, CancellationToken cancellationToken);
    }
}
