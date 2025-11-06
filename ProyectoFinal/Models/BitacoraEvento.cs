using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal.Models
{
    public class BitacoraEvento
    {
        [Key]
        public int IdEvento { get; set; }
        public string TablaDeEvento { get; set; }
        public string TipoDeEvento { get; set; }
        public DateTime FechaDeEvento { get; set; }
        public string DescripcionDeEvento { get; set; }
        public string StackTrace { get; set; }
        public string DatosAnteriores { get; set; }
        public string DatosPosteriores { get; set; }
    }
}