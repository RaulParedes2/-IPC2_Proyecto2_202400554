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
            return Codificacion.ObtenerLetra(nombreDron, altura);
        }

        public void ConfigurarCodificacion(string nombreDron, int altura, char letra)
        {
            Codificacion.Agregar(nombreDron, altura, letra);
        }

        public Dron? ObtenerDron(string nombreDron)
        {
            return Drones.ObtenerPorNombre(nombreDron);
        }
    }
}