namespace card_library.Core.Application.Models
{
    public class Deck
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageRefUrl { get; set; } = string.Empty;
        public List<DeckCardMapping> DeckCards { get; set; } = new List<DeckCardMapping>();
        public List<DeckTag> DeckTags { get; set; } = new List<DeckTag>();

        // Relationships
        public Guid GameId { get; set; }
        public Game? Game { get; set; }
    }
}
