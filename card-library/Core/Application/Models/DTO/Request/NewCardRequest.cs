namespace card_library.Core.Application.Models.DTO.Request
{
    public class NewCardRequest
    {
        public string CardTitle { get; set; } = "Untitled";
        public string? HexCardColor { get; set; }
        public List<CardTag> Tags { get; set; } = new List<CardTag>();
    }
}
