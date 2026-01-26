namespace card_library.Core.Application.Models
{
    public class CardTag
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Type { get; set; } = string.Empty;

        // Relationships
        public Guid CardId { get; set; }
        public Card? Card { get; set; }
    }
}
