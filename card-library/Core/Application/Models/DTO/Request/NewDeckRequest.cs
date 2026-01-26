namespace card_library.Core.Application.Models.DTO.Request
{
    public class NewDeckRequest
    {
        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
        public string DeckName { get; set; } = string.Empty;
        public string DeckDescription { get; set; } = string.Empty;
        public List<Guid> CardIds { get; set; } = new List<Guid>();
    }
}
