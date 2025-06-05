using Biblioteca.Domain.Entities;

namespace Biblioteca.Application.Interfaces
{
    public interface IPersonaService
    {
        Task<IEnumerable<Persona>> GetAllAsync();
        Task<Persona?> GetByIdAsync(int id);
        Task AddAsync(Persona persona);
        Task UpdateAsync(Persona persona);
        Task DeleteAsync(int id);
    }
}
