using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinal.Models
{
    public class Configuracion
    {
        [Key]
        public int IdConfiguracion { get; set; }

        [ForeignKey("Comercio")]
        public int IdComercio { get; set; }

        public int TipoConfiguracion { get; set; }

        public int Comision { get; set; }

        public DateTime FechaDeRegistro { get; set; }

        public DateTime? FechaDeModificacion { get; set; }

        public bool Estado { get; set; } = true;


        public Comercio? Comercio { get; set; }
    }
}