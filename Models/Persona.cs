using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca_MVC.Models
{
    [Table("Persona")]
    public class Persona
    {
        [Key]
        [Column("id")]

        public int Id { get; set; }

        [Required]
        [Column("nombre")]

        public string Nombre { get; set; }

        [Required]
        [Column("cedula")]

        public string Cedula { get; set; }

        [Required]
        [Column("rol")]
        public string Rol { get; set; } // Estudiante, Profesor, Administrativo

        [Column("prestamos")]
        public virtual ICollection<Prestamo> Prestamos { get; set; }
    }
}
