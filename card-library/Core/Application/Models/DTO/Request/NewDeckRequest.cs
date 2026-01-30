namespace card_library.Core.Application.Models.DTO.Request
{
    public class CardInDeck
    {
        public Guid CardId { get; set; }
        public int CardCount { get; set; }
    }
    public class NewDeckRequest
    {
        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
        public string DeckName { get; set; } = string.Empty;
        public string DeckDescription { get; set; } = string.Empty;
        public List<CardInDeck> CardIds { get; set; } = new List<CardInDeck>();
    }
}
