using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca_MVC.Models
{
    public class Persona
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Cedula { get; set; }

        [Required]
        public string Rol { get; set; } // Estudiante, Profesor, Administrativo

        public virtual ICollection<Prestamo> Prestamos { get; set; }
    }
}
