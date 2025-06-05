using Biblioteca.Application.Interfaces;
using Biblioteca.Domain.Entities;
using Biblioteca.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Infrastructure.Services
{
    public class MaterialService : IMaterialService
    {
        private readonly BibliotecaContext _context;

        public MaterialService(BibliotecaContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Material>> GetAllAsync()
            => await _context.Materiales.ToListAsync();

        public async Task<Material?> GetByIdAsync(int id)
         => await _context.Materiales.FindAsync(id);

        public async Task AddAsync(Material material)
        {
            _context.Materiales.Add(material);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Material material)
        {
            _context.Materiales.Update(material);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var material = await _context.Materiales
                .Include(m => m.Prestamos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (material == null)
                return;

            if (material.Prestamos.Any())
                throw new InvalidOperationException("No se puede eliminar un material con préstamos asociados.");

            _context.Materiales.Remove(material);
            await _context.SaveChangesAsync();
        }

    }
}
