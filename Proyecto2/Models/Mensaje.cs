using IPC2_Proyecto2.TDAs;

namespace IPC2_Proyecto2.Models
{
   public class Mensaje
    {
        public string Nombre { get; set; }
        public string TextoOriginal { get; set; }
        public ListaInstruccionesEmision Instrucciones { get; set; }

        public Mensaje(string nombre)
        {
            Nombre = nombre;
            TextoOriginal = "";
            Instrucciones = new ListaInstruccionesEmision();
        }

        public void AgregarInstruccion(string nombreDron, int altura, char letra)
        {
            Instrucciones.Agregar(new InstruccionEmision(nombreDron, altura, letra));
        }

        public void ConstruirDesdeTexto(string texto, SistemaDrones sistema)
        {
            TextoOriginal = texto;
            // Aquí iría la lógica para convertir el texto en instrucciones
            // usando la tabla de codificación del sistema
        }
    }
}