using card_library.Core.Application.Models;
using card_library.Core.Application.Models.DTO.Patches;
using card_library.Core.Application.Models.DTO.Request;
using card_library.Core.Application.Models.DTO.Response;
using card_library.Core.Application.Repository.Contracts;
using card_library.Core.Application.Services.Contracts;
using card_library.Core.Utils;
using Microsoft.EntityFrameworkCore;

namespace card_library.Core.Application.Services
{
    public class GamesService : IGamesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGameRepository _game;
        private readonly IFileStorageService _fileStorageService;

        public GamesService(IGameRepository gameRepository, IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
        {
            _game = gameRepository;
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
        }

        public async Task<Result<NewGameResponse>> CreateGameById(NewGameRequest newGameRequest, CancellationToken ct)
        {
            Game newGame = new Game{
                GameName = newGameRequest.GameName,
                Description = newGameRequest.Description,
                ImageRefUrl = newGameRequest.ImageRefUrl ?? string.Empty
            };

            try
            {
                await _game.AddAsync(newGame, ct);
                await _unitOfWork.SaveChangesAsync(ct);

                return Result<NewGameResponse>.Ok(new NewGameResponse
                {
                    Id = newGame.Id,
                    Name = newGame.GameName
                });
            }
            catch
            {
                return Result<NewGameResponse>.Fail("Failed to create new game");
            }
        }

        public async Task<Result<NewGameResponse>> EditGameContent(NewGameRequest editGameRequest, CancellationToken ct)
        {
            GamePatch gamePatch = new GamePatch
            {
                Id = editGameRequest.Id,
                GameName = editGameRequest.GameName,
                Description = editGameRequest.Description,
                ImageRefUrl = editGameRequest.ImageRefUrl ?? string.Empty
            };
            bool isUpdated = await _game.UpdateGame(gamePatch, ct);
            if (!isUpdated)
            {
                return Result<NewGameResponse>.Fail("Failed to update game of associated ID.");
            }

            return Result<NewGameResponse>.Ok(new NewGameResponse
            {
                Id = gamePatch.Id,
                Name = gamePatch.GameName,
            });
        }


        public async Task<Result<GameResponse>> GetGameById(Guid game_id, CancellationToken ct)
        {
            var gameResult = await _game.GetById(game_id, ct);
            if (gameResult == null)
            {
                return Result<GameResponse>.Fail("Failed to get game with associated ID");
            }

            GameResponse gameResponse = new GameResponse
            {
                GameId = game_id,
                GameName = gameResult.GameName,
                Description = gameResult.Description,
                PublicImgUrl = await ResolvePresignedUrl(gameResult.ImageRefUrl, ct)
            };

            return Result<GameResponse>.Ok(gameResponse);
        }

        public async Task<Result<List<GameResponse>>> GetGamesByName(string game_name, CancellationToken ct)
        {
            var gamesList = await _game.GetListByName(game_name, ct);
            if (gamesList == null || gamesList.Count == 0)
            {
                return Result<List<GameResponse>>.Fail("Game of given name not found.");
            }

            var responses = new List<GameResponse>();
            foreach (var game in gamesList)
            {
                responses.Add(new GameResponse
                {
                    GameId = game.Id,
                    GameName = game.GameName,
                    Description = game.Description,
                    PublicImgUrl = await ResolvePresignedUrl(game.ImageRefUrl, ct)
                });
            }

            return Result<List<GameResponse>>.Ok(responses);
        }

        public async Task<Result<string>> UploadImage(Guid gameId, string s3Key, CancellationToken ct)
        {
            var game = await _game.GetById(gameId, ct);
            if (game == null)
            {
                return Result<string>.Fail("Game not found.");
            }

            game.ImageRefUrl = s3Key;
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<string>.Ok(s3Key);
        }

        public async Task<Result<string>> DeleteImage(Guid gameId, CancellationToken ct)
        {
            var game = await _game.GetById(gameId, ct);
            if (game == null)
            {
                return Result<string>.Fail("Game not found.");
            }

            if (string.IsNullOrEmpty(game.ImageRefUrl))
            {
                return Result<string>.Fail("Game has no image to delete.");
            }

            var s3Key = game.ImageRefUrl;
            game.ImageRefUrl = string.Empty;
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<string>.Ok(s3Key);
        }

        private async Task<string> ResolvePresignedUrl(string imageRefUrl, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(imageRefUrl))
                return string.Empty;

            return await _fileStorageService.GetPresignedUrlAsync(imageRefUrl, ct: ct);
        }
    }
}
