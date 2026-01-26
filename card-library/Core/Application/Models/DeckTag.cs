namespace card_library.Core.Application.Models
{
    public class DeckTag
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Type { get; set; } = string.Empty;

        // Relationships
        public Guid DeckId { get; set; }
        public Deck? Deck { get; set; }
    }
}
