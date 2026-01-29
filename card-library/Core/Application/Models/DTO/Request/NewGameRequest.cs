namespace card_library.Core.Application.Models.DTO.Request
{
    public class NewGameRequest
    {
        public Guid Id { get; set; }
        public string GameName { get; set; } = "Untitled";
        public string Description { get; set; } = string.Empty;
    }
}
