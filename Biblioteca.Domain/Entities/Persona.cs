using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

//test
namespace Biblioteca.Domain.Entities
{
    [Table("Persona")]
    public class Persona
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Column("cedula")]
        public string Cedula { get; set; } = string.Empty;

        [Required]
        [Column("rol")]
        public string Rol { get; set; } = string.Empty; // "Estudiante", "Profesor", "Administrativo"

        [Column("activo")]
        public bool Activo { get; set; } = true; // Para borrado lógico

        // Relación con préstamos
        public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
    }
}
