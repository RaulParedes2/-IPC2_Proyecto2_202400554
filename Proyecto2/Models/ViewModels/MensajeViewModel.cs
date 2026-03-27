using Proyecto2.TDAs;

namespace Proyecto2.Models.ViewModels
{
    public class MensajeViewModel
    {
        public string Nombre { get; set; } = string.Empty;
        public string TextoOriginal { get; set; } = string.Empty;
        public string SistemaSeleccionado { get; set; } = string.Empty;
        public ListaSistemasDrones SistemasDisponibles { get; set; } = new ListaSistemasDrones();
        public ListaInstruccionesEmision Instrucciones { get; set; } = new ListaInstruccionesEmision();
        public int TiempoOptimo { get; set; }
        public string MensajeRecibido { get; set; } = string.Empty;
    }
}