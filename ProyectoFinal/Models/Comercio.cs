using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal.Models
{
    public class Comercio
    {
        [Key]
        public int IdComercio { get; set; }


        public string Identificacion { get; set; } = string.Empty;

 
        public int TipoIdentificacion { get; set; }

        public string Nombre { get; set; } = string.Empty;


        public int TipoDeComercio { get; set; }


        public string Telefono { get; set; } = string.Empty;

        public string CorreoElectronico { get; set; } = string.Empty;

        public string Direccion { get; set; } = string.Empty;

        public DateTime FechaDeRegistro { get; set; }

        public DateTime? FechaDeModificacion { get; set; }

        public bool Estado { get; set; }
    }
}
