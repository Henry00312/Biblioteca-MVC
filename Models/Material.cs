using System;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca_MVC.Models
{
    public class Material
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; }

        public DateTime FechaRegistro { get; set; }

        public int CantidadRegistrada { get; set; }

        public int CantidadActual { get; set; }
    }
}
