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

        [Required]
        [Column("tipo")]
        public string Tipo { get; set; } = "Prestamo"; // Se asigna en el backend

        [Required]
        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now; // Se asigna en el backend

        // Fecha de devolución (vacía inicialmente)
        [Column("fechadevolucion")]
        public DateTime? FechaDevolucion { get; set; } // Nullable para que se mantenga en blanco

        // Relación con Material
        [Column("materialid")]
        public int MaterialId { get; set; }

        [ForeignKey("MaterialId")]
        public virtual Material? Material { get; set; } // opcional para evitar conflictos con filtros globales

        // Relación con Persona (opcional)
        [Column("personaid")]
        public int? PersonaId { get; set; }

        [ForeignKey("PersonaId")]
        public virtual Persona? Persona { get; set; }
    }
}
