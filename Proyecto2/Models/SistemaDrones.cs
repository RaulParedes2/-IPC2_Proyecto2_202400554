using Proyecto2.TDAs;

namespace Proyecto2.Models
{
    public class SistemaDrones
    {
        public string Nombre { get; set; }
        public ListaDrones Drones { get; set; }
        public TablaCodificacion Codificacion { get; set; }

        public SistemaDrones(string nombre)
        {
            Nombre = nombre;
            Drones = new ListaDrones();
            Codificacion = new TablaCodificacion();
        }

        public void AgregarDron(Dron dron)
        {
            if (!Drones.Existe(dron.Nombre))
            {
                Drones.Agregar(dron);
            }
        }

        public void AgregarDron(string nombreDron, int alturaInicial = 1)
        {
            if (!Drones.Existe(nombreDron))
            {
                Drones.Agregar(new Dron(nombreDron, alturaInicial));
            }
        }

        public char ObtenerLetra(string nombreDron, int altura)
        {
            char letra = Codificacion.ObtenerLetra(nombreDron, altura);
            Console.WriteLine($"  Buscando letra: {nombreDron} @ {altura}m = '{letra}'");
            return letra;
        }

        public void ConfigurarCodificacion(string nombreDron, int altura, char letra)
        {
            // Primero eliminar si ya existe
    Codificacion.EliminarCombinacion(nombreDron, altura);
    // Luego agregar la nueva
    Codificacion.Agregar(nombreDron, altura, letra);
        }

        public Dron? ObtenerDron(string nombreDron)
        {
            return Drones.ObtenerPorNombre(nombreDron);
        }

        // Método para validar si una instrucción existe en la tabla de codificación
        public bool ValidarInstruccion(string nombreDron, int altura)
        {
            return Codificacion.ExisteCombinacion(nombreDron, altura);
        }

        // Método para validar si un dron existe en el sistema
        public bool ValidarDron(string nombreDron)
        {
            return Drones.Existe(nombreDron);
        }
    }
}