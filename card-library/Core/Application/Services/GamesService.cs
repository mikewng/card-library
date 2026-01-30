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
        public GamesService(IGameRepository gameRepository, IUnitOfWork unitOfWork)
        {
            _game = gameRepository;
            _unitOfWork = unitOfWork;
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


        // TBI
        public async Task<Result<GameResponse>> GetGameById(Guid game_id, CancellationToken ct)
        {
            var gameResult = await _game.GetById(game_id, ct);
            if (gameResult == null)
            {
                return Result<GameResponse>.Fail("Failed to get game with associated ID");
            }

            GameResponse gameResponse = new GameResponse
            {
                
            };

            return Result<GameResponse>.Ok(gameResponse);
        }

        public Task<Result<List<GameResponse>>> GetGamesByName(string game_name, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
