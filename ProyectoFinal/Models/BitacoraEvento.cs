using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal.Models
{
    public class BitacoraEvento
    {
        [Key]
        public int IdEvento { get; set; }
        
        [Required]
        public string TablaDeEvento { get; set; }
        
        [Required]
        public string TipoDeEvento { get; set; }
        
        [Required]
        public DateTime FechaDeEvento { get; set; }
        
        [Required]
        public string DescripcionDeEvento { get; set; }
        
        public string? StackTrace { get; set; }
        
        public string? DatosAnteriores { get; set; }
        
        public string? DatosPosteriores { get; set; }
    }
}