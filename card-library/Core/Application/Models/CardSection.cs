namespace card_library.Core.Application.Models
{
    public class CardSection
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? TextField { get; set; }
        public bool IsBolded { get; set; } = false;
        public bool IsItalicized { get; set; } = false;
        public float TextSize { get; set; } = 0;
        public bool IsCustom { get; set; } = false;

        // Relationships
        public Guid CardId { get; set; }
        public Card? Card { get; set; }
    }
}
