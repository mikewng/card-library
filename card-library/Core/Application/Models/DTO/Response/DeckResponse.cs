namespace card_library.Core.Application.Models.DTO.Response
{
    public class DeckResponse
    {
        public Guid DeckId { get; set; }
        public string DeckName { get; set; } = "Untitled";
        public string DeckDescription { get; set; } = string.Empty;
        public string PublicImgUrl { get; set; } = string.Empty;
    }
}
