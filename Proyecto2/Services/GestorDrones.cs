using Proyecto2.Models;
using Proyecto2.TDAs;

namespace Proyecto2.Services
{
    public class GestorDrones
    {
        private ListaDrones drones;

        public GestorDrones()
        {
            drones = new ListaDrones();

        }
        public ListaDrones ObtenerTodos()
        {
            return drones.ObtenerOrdenadosAlfabeticamente();
        }

        public Dron? ObtenerPorNombre(string nombre)
        {
            return drones.ObtenerPorNombre(nombre);
        }

        public bool AgregarDron(string nombre, int alturaInicial = 1)
        {
            if (drones.Existe(nombre))
                return false;

                drones.Agregar(new Dron(nombre, alturaInicial));
                return true;
        }
        public bool EliminarDron(string nombre)
        {
            return drones.Eliminar(nombre);
        }

        public bool ExisteDron(string nombre)
        {
            return drones.Existe(nombre);
        }

         public int CantidadDrones()
        {
            return drones.Count;
        }


    }
}