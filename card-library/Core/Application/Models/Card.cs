namespace card_library.Core.Application.Models
{
    public class Card
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string HexCardColor {  get; set; } = "FFFFFF";
        public string ImageRefUrl { get; set; } = string.Empty;
        public bool IsRawCardImage { get; set; } = false;
        public List<CardSection> CardSections { get; set; } = new List<CardSection>();
        public List<CardTag> CardTags { get; set; } = new List<CardTag>();
    }
}
