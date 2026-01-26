namespace card_library.Core.Application.Models
{
    public class Game
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string GameName { get; set; } = "Untitled";
        public string Description { get; set; } = string.Empty;
        public string ImageRefUrl { get; set; } = string.Empty;
        public List<Deck> Decks { get; set; } = new List<Deck>();
    }
}
