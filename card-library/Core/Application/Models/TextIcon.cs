namespace card_library.Core.Application.Models
{
    public class TextIcon
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string ImageRefUrl { get; set; } = string.Empty;
    }
}
