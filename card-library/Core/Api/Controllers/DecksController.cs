using card_library.Core.Application.Models.DTO.Request;
using card_library.Core.Application.Models.DTO.Response;
using card_library.Core.Application.Services.Contracts;
using card_library.Core.Utils;
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

        [HttpGet("id/{deckId:guid}", Name = "GetDeckById")]
        public async Task<ActionResult<Result<DeckResponse>>> GetById(Guid deckId)
        {
            var deck = await _decksService.GetDeckById(deckId);
            if (!deck.Success || deck.Value == null)
            {
                return Result<DeckResponse>.Fail("Could not find deck by given id.");
            }

            return Result<DeckResponse>.Ok(deck.Value);
        }

        [HttpGet("name/{deckName}", Name = "GetDecksByName")]
        public async Task<ActionResult<Result<List<DeckResponse>>>> GetByName(string deckName)
        {
            var deckList = await _decksService.GetDecksByName(deckName);
            if (!deckList.Success || deckList.Value == null)
            {
                return Result<List<DeckResponse>>.Fail("Could not find deck by given id.");
            }

            return Result<List<DeckResponse>>.Ok(deckList.Value);
        }

        [HttpPost("create", Name = "CreateDeck")]
        public async Task<ActionResult<Result<NewDeckResponse>>> Create([FromBody] NewDeckRequest newDeckRequest)
        {
            var deckResult = await _decksService.CreateDeck(newDeckRequest);
            if (!deckResult.Success || deckResult.Value == null)
            {
                return Result<NewDeckResponse>.Fail("Failed to create specified deck.");
            }

            return Result<NewDeckResponse>.Ok(deckResult.Value);
        }

        [HttpPut("edit", Name = "Edit")]
        public async Task<ActionResult<Result<NewDeckResponse>>> Update([FromBody] ExistingDeckRequest existingDeckRequest)
        {
            var deckResult = await _decksService.UpdateDeckById(existingDeckRequest);
            if (!deckResult.Success || deckResult.Value == null)
            {
                return Result<NewDeckResponse>.Fail("Failed to update specified deck.");
            }

            return Result<NewDeckResponse>.Ok(deckResult.Value);
        }

    }
}
