using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca_MVC.Models
{
    public class BibliotecaContext : DbContext
    {
        public BibliotecaContext(DbContextOptions<BibliotecaContext> options)
            : base(options)
        {
        }

        public DbSet<Material> Materiales { get; set; }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }
    }
}
