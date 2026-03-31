using System;
using Proyecto2.Models;

namespace Proyecto2.TDAs
{
    public class ListaMensajes
    {
        private NodoMensaje? primero;
        private int count;

        public ListaMensajes()
        {
            primero = null;
            count = 0;
        }

        public void Agregar(Mensaje mensaje)
        {
            NodoMensaje nuevo = new NodoMensaje(mensaje);

            if (primero == null)
            {
                primero = nuevo;
            }
            else
            {
                NodoMensaje actual = primero;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevo;
            }
            count++;
        }

        public Mensaje? ObtenerPorNombre(string nombre)
        {
            NodoMensaje? actual = primero;
            while (actual != null)
            {
                if (actual.Data.Nombre == nombre)
                    return actual.Data;
                actual = actual.Siguiente;
            }
            return null;
        }

        public Mensaje? ObtenerPorIndice(int indice)
        {
            if (indice < 0 || indice >= count)
                return null;

            NodoMensaje actual = primero!;
            for (int i = 0; i < indice; i++)
            {
                actual = actual.Siguiente!;
            }
            return actual.Data;
        }

        public bool Eliminar(string nombre)
        {
            if (primero == null) return false;

            if (primero.Data.Nombre == nombre)
            {
                primero = primero.Siguiente;
                count--;
                return true;
            }

            NodoMensaje actual = primero;
            while (actual.Siguiente != null)
            {
                if (actual.Siguiente.Data.Nombre == nombre)
                {
                    actual.Siguiente = actual.Siguiente.Siguiente;
                    count--;
                    return true;
                }
                actual = actual.Siguiente;
            }
            return false;
        }

        public bool Existe(string nombre)
        {
            return ObtenerPorNombre(nombre) != null;
        }

        public ListaMensajes ObtenerOrdenadosAlfabeticamente()
        {
            ListaMensajes ordenada = new ListaMensajes();

            if (primero == null)
                return ordenada;

            ListaMensajes listaTemp = new ListaMensajes();
            NodoMensaje? actual = primero;
            while (actual != null)
            {
                listaTemp.Agregar(actual.Data);
                actual = actual.Siguiente;
            }

            while (!listaTemp.EstaVacia)
            {
                NodoMensaje? primeroTemp = listaTemp.GetPrimero();
                if (primeroTemp != null)
                {
                    Mensaje mensajeActual = primeroTemp.Data;
                    listaTemp.Eliminar(mensajeActual.Nombre);

                    if (ordenada.primero == null)
                    {
                        ordenada.Agregar(mensajeActual);
                    }
                    else
                    {
                        NodoMensaje? nodoOrden = ordenada.primero;
                        NodoMensaje? anterior = null;
                        bool insertado = false;

                        while (nodoOrden != null && !insertado)
                        {
                            if (string.Compare(mensajeActual.Nombre, nodoOrden.Data.Nombre) < 0)
                            {
                                NodoMensaje nuevo = new NodoMensaje(mensajeActual);
                                if (anterior == null)
                                {
                                    nuevo.Siguiente = ordenada.primero;
                                    ordenada.primero = nuevo;
                                }
                                else
                                {
                                    nuevo.Siguiente = anterior.Siguiente;
                                    anterior.Siguiente = nuevo;
                                }
                                ordenada.count++;
                                insertado = true;
                            }
                            anterior = nodoOrden;
                            nodoOrden = nodoOrden.Siguiente;
                        }

                        if (!insertado)
                        {
                            ordenada.Agregar(mensajeActual);
                        }
                    }
                }
            }

            return ordenada;
        }

        public bool Insertar(int indice, Mensaje mensaje)
        {
            if (indice < 0 || indice > count)
                return false;

            NodoMensaje nuevo = new NodoMensaje(mensaje);

            if (indice == 0)
            {
                nuevo.Siguiente = primero;
                primero = nuevo;
            }
            else
            {
                NodoMensaje actual = primero!;
                for (int i = 0; i < indice - 1; i++)
                {
                    actual = actual.Siguiente!;
                }
                nuevo.Siguiente = actual.Siguiente;
                actual.Siguiente = nuevo;
            }
            count++;
            return true;
        }

        // Reemplazamos ParaCada con métodos específicos
        /* public void MostrarTodos(Action<Mensaje> accion)
         {
             NodoMensaje? actual = primero;
             while (actual != null)
             {
                 accion(actual.Data);
                 actual = actual.Siguiente;
             }
         }*/

        // Eliminar el método MostrarTodos que usa Action<Mensaje>

        // En su lugar, usar métodos específicos:
        public string ObtenerListadoCompleto()
        {
            string resultado = "";
            NodoMensaje? actual = primero;
            int indice = 1;
            while (actual != null)
            {
                resultado += $"{indice}. {actual.Data.Nombre} - Instrucciones: {actual.Data.Instrucciones.Count}\n";
                actual = actual.Siguiente;
                indice++;
            }
            return resultado;
        }

        public string ObtenerResumenMensajes()
        {
            string resultado = "=== LISTA DE MENSAJES ===\n";
            NodoMensaje? actual = primero;
            while (actual != null)
            {
                resultado += $"Nombre: {actual.Data.Nombre}\n";
                resultado += $"Texto: {actual.Data.TextoOriginal}\n";
                resultado += $"Instrucciones: {actual.Data.Instrucciones.Count}\n";
                resultado += "------------------------\n";
                actual = actual.Siguiente;
            }
            return resultado;
        }

        public void EjecutarEnCadaMensaje(Action<Mensaje> accion)
        {
            // Este método aún tiene Action<Mensaje> - también es genérico
            // Mejor evitarlo y usar métodos específicos
        }

        // Método para obtener todos los nombres como string
        public string ObtenerNombresComoString()
        {
            string resultado = "";
            NodoMensaje? actual = primero;
            while (actual != null)
            {
                resultado += actual.Data.Nombre;
                if (actual.Siguiente != null)
                    resultado += ", ";
                actual = actual.Siguiente;
            }
            return resultado;
        }

        // Método para contar mensajes con un texto específico
        public int ContarMensajesConTexto(string texto)
        {
            int contador = 0;
            NodoMensaje? actual = primero;
            while (actual != null)
            {
                if (actual.Data.TextoOriginal == texto)
                    contador++;
                actual = actual.Siguiente;
            }
            return contador;
        }

        public int Count { get { return count; } }

        public bool EstaVacia { get { return count == 0; } }

        public NodoMensaje? GetPrimero()
        {
            return primero;
        }

        public void Limpiar()
        {
            primero = null;
            count = 0;
        }
    }
}