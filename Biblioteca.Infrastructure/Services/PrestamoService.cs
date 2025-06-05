using Biblioteca.Application.Interfaces;
using Biblioteca.Domain.Entities;
using Biblioteca.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Infrastructure.Services
{
    public class PrestamoService : IPrestamoService
    {
        private readonly BibliotecaContext _context;

        public PrestamoService(BibliotecaContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Prestamo>> GetAllAsync()
        {
            return await _context.Prestamos
                .Include(p => p.Persona)
                .Include(p => p.Material)
                .ToListAsync();
        }

        public async Task<Prestamo?> GetByIdAsync(int id)
        {
            return await _context.Prestamos
                .Include(p => p.Persona)
                .Include(p => p.Material)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Prestamo prestamo)
        {
            // Buscar el material asociado
            var material = await _context.Materiales.FindAsync(prestamo.MaterialId);

            if (material == null)
                throw new InvalidOperationException("El material no existe.");

            if (material.CantidadActual <= 0)
                throw new InvalidOperationException("No hay unidades disponibles para prestar.");

            // Restar 1 unidad al stock actual
            material.CantidadActual--;

            _context.Prestamos.Add(prestamo);
            await _context.SaveChangesAsync();
        }



        public async Task UpdateAsync(Prestamo prestamo)
        {
            _context.Prestamos.Update(prestamo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var prestamo = await _context.Prestamos.FindAsync(id);
            if (prestamo != null)
            {
                _context.Prestamos.Remove(prestamo);
                await _context.SaveChangesAsync();
            }
        }
    }
}
