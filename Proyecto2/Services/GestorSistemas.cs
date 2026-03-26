using Proyecto2.Models;
using Proyecto2.TDAs;

namespace Proyecto2.Services
{
    public class GestorSistemas
    {
        private ListaSistemasDrones sistemas;

        public GestorSistemas()
        {
            sistemas = new ListaSistemasDrones();
        }

        public ListaSistemasDrones ObtenerTodos()
        {
            return sistemas;
        }

        public SistemaDrones? ObtenerPorNombre(string nombre)
        {
            return sistemas.ObtenerPorNombre(nombre);
        }

        public bool AgregarSistema(string nombre)
        {
            if (sistemas.Existe(nombre))
                return false;

            sistemas.Agregar(new SistemaDrones(nombre));
            return true;
        }

        public bool AgregarDronASistema(string nombreSistema, string nombreDron, int alturaInicial = 1)
        {
            SistemaDrones? sistema = sistemas.ObtenerPorNombre(nombreSistema);
            if (sistema == null)
                return false;

            if (sistema.Drones.Existe(nombreDron))
                return false;

            sistema.AgregarDron(nombreDron, alturaInicial);
            return true;
        }

        public bool ConfigurarCodificacion(string nombreSistema, string nombreDron, int altura, char letra)
        {
            SistemaDrones? sistema = sistemas.ObtenerPorNombre(nombreSistema);
            if (sistema == null)
                return false;

            if (!sistema.Drones.Existe(nombreDron))
                return false;

            sistema.ConfigurarCodificacion(nombreDron, altura, letra);
            return true;
        }

        public char ObtenerLetra(string nombreSistema, string nombreDron, int altura)
        {
            SistemaDrones? sistema = sistemas.ObtenerPorNombre(nombreSistema);
            if (sistema == null)
                return '?';

            return sistema.ObtenerLetra(nombreDron, altura);
        }

        public ListaDrones? ObtenerDronesSistema(string nombreSistema)
        {
            SistemaDrones? sistema = sistemas.ObtenerPorNombre(nombreSistema);
            if (sistema == null)
                return null;

            return sistema.Drones;
        }

        public bool Eliminar(string nombre)
        {
            return sistemas.Eliminar(nombre);
        }
    }
}