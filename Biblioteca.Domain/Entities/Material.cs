using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Biblioteca.Domain.Entities; // si Prestamo está en el mismo namespace

namespace Biblioteca.Domain.Entities
{
    [Table("Material")]
    public class Material
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [Column("fecharegistro")]
        public DateTime FechaRegistro { get; set; }

        [Required]
        [Column("cantidadregistrada")]
        public int CantidadRegistrada { get; set; }

        [Required]
        [Column("cantidadactual")]
        public int CantidadActual { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true; // Para borrado lógico

        public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
    }
}
