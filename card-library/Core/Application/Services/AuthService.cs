using card_library.Core.Application.Models;
using card_library.Core.Application.Repository.Contracts;
using card_library.Core.Application.Services.Contracts;
using card_library.Core.Utils;
using card_library.Main.Models.DTO.Request;

namespace tasking_api.Main.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _users = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return Result.Fail("Email and password are required");
            }

            // Get User
            var user = await _users.GetByEmailAsync(loginRequest.Email, cancellationToken);
            if (user == null)
            {
                return Result.Fail("Invalid email or password");
            }

            if (!Cryptography.VerifyPassword(loginRequest.Password, user.PasswordHash))
            {
                return Result.Fail("Invalid email or password");
            }

            // Update Login Time
            user.LastLoginAt = DateTime.UtcNow;
            var updateSuccess = await _users.UpdateAsync(user, cancellationToken);
            if (!updateSuccess)
            {
                return Result.Fail("Failed to update user login time");
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }

        public async Task<Result> RegisterAsync(RegisterRequest registerRequest, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(registerRequest.Username) ||
                string.IsNullOrWhiteSpace(registerRequest.Email) ||
                string.IsNullOrWhiteSpace(registerRequest.Password))
            {
                return Result.Fail("Username, email, and password are required");
            }

            if (registerRequest.Password.Length < 6)
            {
                return Result.Fail("Password must be at least 6 characters long");
            }

            if (await _users.EmailExistsAsync(registerRequest.Email, cancellationToken))
            {
                return Result.Fail("Email already exists");
            }

            if (await _users.UsernameExistsAsync(registerRequest.Username, cancellationToken))
            {
                return Result.Fail("Username already exists");
            }

            // Create Password Hash and New User Object
            var passwordHash = Cryptography.HashPassword(registerRequest.Password);
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = registerRequest.Username,
                Email = registerRequest.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow
            };

            await _users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }


    }
}
