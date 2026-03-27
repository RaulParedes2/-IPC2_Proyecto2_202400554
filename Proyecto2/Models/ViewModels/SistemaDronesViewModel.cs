using Proyecto2.TDAs;

namespace Proyecto2.Models.ViewModels
{
    public class SistemaDronesViewModel
    {
        public string Nombre { get; set; } = string.Empty;
        public ListaDrones Drones { get; set; } = new ListaDrones();
        public TablaCodificacion Codificacion { get; set; } = new TablaCodificacion();
        public string DronSeleccionado { get; set; } = string.Empty;
        public int Altura { get; set; }
        public char Letra { get; set; }
    }
}