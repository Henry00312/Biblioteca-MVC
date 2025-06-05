using Biblioteca.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Infrastructure.Data
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Material>().HasQueryFilter(m => m.Activo);

            // Prestamo → Persona (opcional)
            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Persona)
                .WithMany(p => p.Prestamos)
                .HasForeignKey(p => p.PersonaId)
                .OnDelete(DeleteBehavior.SetNull);

            // Prestamo → Material (ahora opcional correctamente)
            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Material)
                .WithMany(m => m.Prestamos)
                .HasForeignKey(p => p.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
