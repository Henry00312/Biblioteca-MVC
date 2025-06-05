using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca.Domain.Entities
{
    [Table("Prestamo")]
    public class Prestamo
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("tipo")]
        public string Tipo { get; set; } = "Prestamo";

        [Required]
        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Column("fechadevolucion")]
        public DateTime? FechaDevolucion { get; set; }

        // 🔗 Relación con Material
        [Column("materialid")]
        public int MaterialId { get; set; }

        [ForeignKey("MaterialId")]
        public virtual Material? Material { get; set; }

        // 🔗 Relación con Persona
        [Column("personaid")]
        public int? PersonaId { get; set; }

        [ForeignKey("PersonaId")]
        public virtual Persona? Persona { get; set; }

        // ✅ Nuevos campos para conservar nombre y cédula
        [Column("personanombre")]
        public string? PersonaNombre { get; set; }

        [Column("personacedula")]
        public string? PersonaCedula { get; set; }
    }
}
