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

            NodoMensaje? actual = primero;
            while (actual != null)
            {
                Mensaje mensajeActual = actual.Data;

                if (ordenada.primero == null)
                {
                    ordenada.Agregar(mensajeActual);
                }
                else
                {
                    NodoMensaje? actualOrdenada = ordenada.primero;
                    int posicion = 0;
                    bool insertado = false;

                    while (actualOrdenada != null && !insertado)
                    {
                        if (string.Compare(mensajeActual.Nombre, actualOrdenada.Data.Nombre) < 0)
                        {
                            ordenada.Insertar(posicion, mensajeActual);
                            insertado = true;
                        }
                        actualOrdenada = actualOrdenada.Siguiente;
                        posicion++;
                    }

                    if (!insertado)
                    {
                        ordenada.Agregar(mensajeActual);
                    }
                }

                actual = actual.Siguiente;
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
        
        public void ParaCada(Action<Mensaje> accion)
        {
            NodoMensaje? actual = primero;
            while (actual != null)
            {
                accion(actual.Data);
                actual = actual.Siguiente;
            }
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