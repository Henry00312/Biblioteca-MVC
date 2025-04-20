using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca_MVC.Models
{
    [Table("Prestamo")]
    public class Prestamo
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("tipo")]
        public string Tipo { get; set; } // Prestamo o Devolucion

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        // FK hacia Material
        [Column("materialid")]
        public int MaterialId { get; set; }

        [ForeignKey("MaterialId")]
        public Material Material { get; set; }

        // FK hacia Persona
        [Column("personaid")]
        public int PersonaId { get; set; }

        [ForeignKey("PersonaId")]
        public Persona Persona { get; set; }
    }
}
