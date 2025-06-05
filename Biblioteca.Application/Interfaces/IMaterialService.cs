using Biblioteca.Domain.Entities;

namespace Biblioteca.Application.Interfaces
{
    public interface IMaterialService
    {
        Task<IEnumerable<Material>> GetAllAsync();
        Task<Material?> GetByIdAsync(int id);
        Task AddAsync(Material material);
        Task UpdateAsync(Material material);
        Task DeleteAsync(int id);
    }
}
