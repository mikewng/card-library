namespace card_library.Core.Application.Models.DTO.Response
{
    public class TextIconResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PublicImgUrl { get; set; } = string.Empty;
    }
}
