using card_library.Core.Application.Models.DTO.Request;
using card_library.Core.Application.Models.DTO.Response;
using card_library.Core.Application.Repository.Contracts;
using card_library.Core.Application.Services.Contracts;
using card_library.Core.Utils;

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

        public Task<Result<NewGameResponse>> CreateGameById(NewGameRequest newGameRequest)
        {
            throw new NotImplementedException();
        }

        public Task<Result<NewGameResponse>> EditGameContent(NewGameRequest newGameResponse)
        {
            throw new NotImplementedException();
        }

        public Task<Result<GameResponse>> GetGameById(Guid game_id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<GameResponse>>> GetGamesByName(string game_name)
        {
            throw new NotImplementedException();
        }
    }
}
