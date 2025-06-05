using Biblioteca.Domain.Entities;

namespace Biblioteca.Application.Interfaces
{
    public interface IPrestamoService
    {
        Task<IEnumerable<Prestamo>> GetAllAsync();
        Task<Prestamo?> GetByIdAsync(int id);
        Task AddAsync(Prestamo prestamo);
        Task UpdateAsync(Prestamo prestamo);
        Task DeleteAsync(int id);
    }
}
