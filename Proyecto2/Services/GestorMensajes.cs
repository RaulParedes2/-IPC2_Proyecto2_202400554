using Proyecto2.Models;
using Proyecto2.TDAs;

namespace Proyecto2.Services
{

    public class GestorMensajes
    {
        private ListaMensajes mensajes;

        public GestorMensajes()
        {
            mensajes = new ListaMensajes();
        }

        public ListaMensajes ObtenerTodos()
        {
            return mensajes.ObtenerOrdenadosAlfabeticamente();
        }

        public Mensaje? ObtenerPorNombre(string nombre)
        {
            return mensajes.ObtenerPorNombre(nombre);
        }

        public bool AgregarMensaje(string nombre, string texto = "")
        {
            if (mensajes.Existe(nombre))
                return false;

            Mensaje nuevoMensaje = new Mensaje(nombre);
            nuevoMensaje.TextoOriginal = texto;
            mensajes.Agregar(nuevoMensaje);
            return true;
        }

        public bool AgregarInstruccion(string nombreMensaje, string nombreDron, int altura, char letra)
        {
            Mensaje? mensaje = mensajes.ObtenerPorNombre(nombreMensaje);
            if (mensaje == null)
                return false;

            mensaje.AgregarInstruccion(nombreDron, altura, letra);
            return true;
        }

        public bool EliminarMensaje(string nombre)
        {
            return mensajes.Eliminar(nombre);
        }

        public ListaInstruccionesEmision? ObtenerInstrucciones(string nombreMensaje)
        {
            Mensaje? mensaje = mensajes.ObtenerPorNombre(nombreMensaje);
            if (mensaje == null)
                return null;

            return mensaje.Instrucciones;
        }
    }
}