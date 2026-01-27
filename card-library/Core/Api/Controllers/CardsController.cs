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

        [HttpGet("{cardId:guid}", Name = "GetCardById")]
        public async Task<ActionResult<Result<CardResponse>>> Get(Guid cardId)
        {
            var card = await _cardsService.GetCardById(cardId);
            if (!card.Success || card.Value == null)
            {
                return Result<CardResponse>.Fail("Could not find deck by given id.");
            }

            return Result<CardResponse>.Ok(card.Value);
        }
    }
}
