using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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

        [Column("materialid")]
        public int MaterialId { get; set; }
        [ForeignKey("MaterialId")]

        [Column("material")]
        public Material Material { get; set; }

        [Column("personaid")]
        public int PersonaId { get; set; }
        [ForeignKey("PersonaId")]

        [Column("persona")]
        public Persona Persona { get; set; }

    }
}
