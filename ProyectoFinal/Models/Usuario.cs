using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinal.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [ForeignKey("Comercio")]
        public int IdComercio { get; set; }

        public Guid? IdNetUser { get; set; }

        public string Nombres { get; set; }

        public string PrimerApellido { get; set; } 
        public string SegundoApellido { get; set; }

        public string Identificacion { get; set; }

        public string CorreoElectronico { get; set; }

        public DateTime FechaDeRegistro { get; set; }

        public DateTime? FechaDeModificacion { get; set; }

        public bool Estado { get; set; } = true;

       
        public Comercio? Comercio { get; set; }
    }
}