namespace card_library.Core.Application.Models.DTO.Request
{
    public class NewGameRequest
    {
        public string GameName { get; set; } = "Untitled";
        public List<Guid> DeckList = new List<Guid>();
    }
}
