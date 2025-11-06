using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal.Models
{
    public class Sinpe
    {
        [Key]
        public int IdSinpe { get; set; }
        public string TelefonoOrigen { get; set; }
        public string NombreOrigen { get; set; }
        public string TelefonoDestinatario { get; set; }
        public string NombreDestinatario { get; set; }
        public decimal Monto { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaDeRegistro { get; set; }
        public bool Estado { get; set; }
    }
}