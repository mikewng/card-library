using card_library.Core.Application.Models;

namespace card_library.Core.Application.Repository.Contracts
{
    public interface ITextIconRepository
    {
        Task AddAsync(TextIcon textIcon, CancellationToken ct = default);
        Task<TextIcon?> GetById(Guid id, CancellationToken ct = default);
        Task<List<TextIcon>> GetAll(CancellationToken ct = default);
        Task<TextIcon?> GetByName(string name, CancellationToken ct = default);
        void Delete(TextIcon textIcon);
    }
}
