namespace card_library.Core.Application.Models.DTO.Request
{
    public class ExistingCardRequest
    {
        public Guid DeckId { get; set; }
        public Guid CardId { get; set; }
    }
}
