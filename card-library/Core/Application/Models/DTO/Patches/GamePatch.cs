namespace card_library.Core.Application.Models.DTO.Patches
{
    public class GamePatch
    {
        public Guid Id { get; set; }
        public string GameName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageRefUrl { get; set; } = string.Empty;
    }
}
