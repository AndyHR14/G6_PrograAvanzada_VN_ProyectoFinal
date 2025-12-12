using System.Threading.Tasks;

namespace ProyectoFinal.Services
{
    public interface IBitacoraService
    {
        Task RegistrarEventoAsync(string tabla, string tipoEvento, string descripcion, 
            object datosAnteriores = null, object datosPosteriores = null);
        
        Task RegistrarErrorAsync(string tabla, string descripcion, Exception ex);
    }
}