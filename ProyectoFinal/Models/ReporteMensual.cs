using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal.Models
{
    public class ReporteMensual
    {
        [Key]
        public int IdReporte { get; set; }
        public int IdComercio { get; set; }
        public int CantidadDeCajas { get; set; }
        public decimal MontoTotalRecaudado { get; set; }
        public int CantidadDeSINPES { get; set; }
        public decimal MontoTotalComision { get; set; }
        public DateTime FechaDelReporte { get; set; }

    }
}