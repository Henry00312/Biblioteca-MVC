using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca_MVC.Models
{
    [Table("Material")]
    public class Material
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("titulo")]
        public string Titulo { get; set; }

        [Column("henry@HENRYfecharegistro")]
        public DateTime FechaRegistro { get; set; }

        [Column("cantidadregistrada")]
        public int CantidadRegistrada { get; set; }

        [Column("cantidadactual")]
        public int CantidadActual { get; set; }
    }
}
