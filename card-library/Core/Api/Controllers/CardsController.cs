using card_library.Core.Application.Models;
using card_library.Core.Application.Models.DTO.Request;
using card_library.Core.Application.Models.DTO.Response;
using card_library.Core.Application.Services.Contracts;
using card_library.Core.Utils;
using Microsoft.AspNetCore.Mvc;

namespace card_library.Core.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CardsController : Controller
    {
        private readonly ILogger<CardsController> _logger;
        private readonly ICardsService _cardsService;

        public CardsController(ILogger<CardsController> logger, ICardsService cardsService)
        {
            _logger = logger;
            _cardsService = cardsService;
        }

        [HttpGet("id/{cardId:guid}", Name = "GetCardById")]
        public async Task<ActionResult<Result<CardResponse>>> GetById(Guid cardId)
        {
            var card = await _cardsService.GetCardById(cardId);
            if (!card.Success || card.Value == null)
            {
                return Result<CardResponse>.Fail("Could not find deck by given id.");
            }

            return Result<CardResponse>.Ok(card.Value);
        }

        [HttpGet("name/{cardName}", Name = "GetCardsByName")]
        public async Task<ActionResult<Result<List<CardResponse>>>> GetByName(string cardName)
        {
            var cardList = await _cardsService.GetCardsByName(cardName);
            if (!cardList.Success || cardList.Value == null)
            {
                return Result<List<CardResponse>>.Fail("Could not find any cards by given name.");
            }

            return Result<List<CardResponse>>.Ok(cardList.Value);
        }

        [HttpGet("deckId/{deckId:guid}", Name = "GetCardsByDeckId")]
        public async Task<Result<List<CardResponse>>> GetByDeckId(Guid deckId)
        {
            var cardList = await _cardsService.GetCardsByDeckId(deckId);
            if (!cardList.Success || cardList.Value == null)
            {
                return Result<List<CardResponse>>.Fail("Could not find any cards of associated deck.");
            }

            return Result<List<CardResponse>>.Ok(cardList.Value);
        }

        [HttpGet("deckTags", Name = "GetCardsByTags")]
        public async Task<Result<List<CardResponse>>> GetByTags([FromBody] TagRequest request)
        {
            var cardList = await _cardsService.GetCardsByTags(request.CardTags);
            if (!cardList.Success || cardList.Value == null)
            {
                return Result<List<CardResponse>>.Fail("Could not find any cards of associated tag(s).");
            }

            return Result<List<CardResponse>>.Ok(cardList.Value);

        }

        [HttpGet("gameId/{gameId:guid}", Name = "GetCardsByGameId")]
        public async Task<Result<List<CardResponse>>> GetByGameId(Guid gameId)
        {
            // Eventually add pagination
            var cardList = await _cardsService.GetCardsByGameId(gameId);
            if (!cardList.Success || cardList.Value == null)
            {
                return Result<List<CardResponse>>.Fail("Could not find any cards of associated game.");
            }

            return Result<List<CardResponse>>.Ok(cardList.Value);
        }

        [HttpPost("create", Name = "CreateCard")]
        public async Task<Result<NewCardResponse>> Create([FromBody] NewCardRequest newCardRequest)
        {
            // Eventually add pagination
            var cardResult = await _cardsService.CreateCard(newCardRequest);
            if (!cardResult.Success || cardResult.Value == null)
            {
                return Result<NewCardResponse>.Fail("Failed to create card.");
            }

            return Result<NewCardResponse>.Ok(cardResult.Value);
        }
    }
}
