namespace card_library.Core.Application.Models.DTO.Request
{
    public class NewCardRequest
    {
        public string CardTitle { get; set; } = "Untitled";
        public string HexCardColor { get; set; } = "FFFFFF";
        public bool IsRawImageOnly { get; set; }

        // Attribute for Image Content
        public List<CardSection> Sections { get; set; } = new List<CardSection>();
        public List<CardTag> Tags { get; set; } = new List<CardTag>();
    }
}
