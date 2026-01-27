namespace card_library.Core.Application.Models
{
    public class DeckCardMapping
    {
        public Guid DeckId { get; set; }
        public Deck Deck { get; set; } = null!;
        public Guid CardId { get; set; }
        public Card Card { get; set; } = null!;
        public int Count { get; set; } = 1;
    }
}
