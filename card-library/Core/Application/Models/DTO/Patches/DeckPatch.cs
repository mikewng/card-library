namespace card_library.Core.Application.Models.DTO.Patches
{
    public class DeckPatch
    {
        public Guid Id { get; set; }
        public string DeckName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
