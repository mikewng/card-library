namespace card_library.Core.Application.Models.DTO.Response
{
    public class GameResponse
    {
        public Guid GameId { get; set; }
        public string GameName { get; set; } = "Untitled";
        public string Description { get; set; } = string.Empty;
        public string PublicImgUrl { get; set; } = string.Empty;
    }
}
