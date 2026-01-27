namespace card_library.Core.Application.Models.DTO.Response
{
    public class CardResponse
    {
        public Guid CardId { get; set; }
        public string CardName { get; set; } = "Untitled";
        public string HexColor { get; set; } = "FFFFFF";
        public string PublicImgUrl {  get; set; } = string.Empty;
        public List<CardSection> CardSections { get; set; } = new List<CardSection>();
        public List<CardTag> CardTags { get; set; } = new List<CardTag>();
    }
}
