using Biblioteca.Application.Interfaces;
using Biblioteca.Domain.Entities;
using Biblioteca.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Biblioteca.Infrastructure.Services
{
    public class PersonaService : IPersonaService
    {
        private readonly BibliotecaContext _context;

        public PersonaService(BibliotecaContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Persona>> GetAllAsync()
            => await _context.Personas.ToListAsync();

        public async Task<Persona?> GetByIdAsync(int id)
            => await _context.Personas.FindAsync(id);

        public async Task AddAsync(Persona persona)
        {
            _context.Personas.Add(persona);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Persona persona)
        {
            _context.Personas.Update(persona);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var persona = await _context.Personas.FindAsync(id);
            if (persona != null)
            {
                _context.Personas.Remove(persona);
                await _context.SaveChangesAsync();
            }
        }
    }
}
