using card_library.Core.Application.Models.DTO.Request;
using card_library.Core.Application.Models.DTO.Response;
using card_library.Core.Application.Services.Contracts;
using card_library.Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;

namespace card_library.Core.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GamesController : Controller
    {
        private readonly ILogger<GamesController> _logger;
        private readonly IGamesService _gamesService;

        public GamesController(ILogger<GamesController> logger, IGamesService gamesService)
        {
            _logger = logger;
            _gamesService = gamesService;
        }

        [HttpPost("create", Name = "CreateGame")]
        public async Task<ActionResult<Result>> Create([FromBody] NewGameRequest newGameRequest)
        {
            var res = await _gamesService.CreateGameById(newGameRequest);
            if (!res.Success || res.Value == null)
            {
                return Result.Fail("Could not create this new game.");
            }

            return Result.Ok();
        }

        [HttpGet("get/id/{game_id:guid}", Name = "GetGameById")]
        public async Task<ActionResult<Result<GameResponse>>> GetById(Guid game_id)
        {
            var res = await _gamesService.GetGameById(game_id);
            if (!res.Success || res.Value == null)
            {
                return Result<GameResponse>.Fail("Could not get game of associated ID.");
            }

            return Result<GameResponse>.Ok(res.Value);
        }

        [HttpGet("get/name/{game_name:guid}", Name = "GetGamesByName")]
        public async Task<ActionResult<Result<List<GameResponse>>>> GetByName(string game_name)
        {
            var res = await _gamesService.GetGamesByName(game_name);
            if (!res.Success || res.Value == null)
            {
                return Result<List<GameResponse>>.Fail("Could not get game of associated name.");
            }

            return Result<List<GameResponse>>.Ok(res.Value);
        }
    }
}
