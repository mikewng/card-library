using card_library.Core.Application.Models.DTO.Response;
using card_library.Core.Application.Services.Contracts;
using card_library.Core.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;

namespace card_library.Core.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DecksController : Controller
    {
        private readonly ILogger<DecksController> _logger;
        private readonly IDecksService _decksService;

        public DecksController(ILogger<DecksController> logger, IDecksService decksService)
        {
            _logger = logger;
            _decksService = decksService;
        }

        [HttpGet("{deckId:guid}", Name = "GetDeckById")]
        public async Task<ActionResult<Result<DeckResponse>>> Get(Guid deckId)
        {
            var deck = await _decksService.GetDeckById(deckId);
            if (!deck.Success || deck.Value == null)
            {
                return Result<DeckResponse>.Fail("Could not find deck by given id.");
            }

            return Result<DeckResponse>.Ok(deck.Value);
        }
    }
}
