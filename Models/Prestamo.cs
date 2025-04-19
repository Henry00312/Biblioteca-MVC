using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca_MVC.Models
{
    public class Prestamo
    {
        [Key]
        public int Id { get; set; }

        public string Tipo { get; set; } // Prestamo o Devolucion

        public DateTime Fecha { get; set; }

        public int MaterialId { get; set; }
        [ForeignKey("MaterialId")]
        public Material Material { get; set; }

        public int PersonaId { get; set; }
        [ForeignKey("PersonaId")]
        public Persona Persona { get; set; }
    }
}
