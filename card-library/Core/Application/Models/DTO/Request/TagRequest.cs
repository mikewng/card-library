namespace card_library.Core.Application.Models.DTO.Request
{
    public class TagRequest
    {
        public List<CardTag> CardTags { get; set; } = new List<CardTag>();
        public List<DeckTag> DeckTags { get; set; } = new List<DeckTag>();
    }
}
