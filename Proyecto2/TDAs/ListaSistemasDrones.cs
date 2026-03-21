using System;
using Proyecto2.Models;

namespace Proyecto2.TDAs
{
    public class ListaSistemasDrones
    {
        private NodoSistemaDrones? primero;
        private int count;

        public ListaSistemasDrones()
        {
            primero = null;
            count = 0;
        }

        public void Agregar(SistemaDrones sistema)
        {
            NodoSistemaDrones nuevo = new NodoSistemaDrones(sistema);

            if (primero == null)
            {
                primero = nuevo;
            }
            else
            {
                NodoSistemaDrones actual = primero;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevo;
            }
            count++;
        }

        public SistemaDrones? ObtenerPorNombre(string nombre)
        {
            NodoSistemaDrones? actual = primero;
            while (actual != null)
            {
                if (actual.Data.Nombre == nombre)
                    return actual.Data;
                actual = actual.Siguiente;
            }
            return null;
        }

        public SistemaDrones? ObtenerPorIndice(int indice)
        {
            if (indice < 0 || indice >= count)
                return null;

            NodoSistemaDrones actual = primero!;
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

            NodoSistemaDrones actual = primero;
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

        public int Count { get { return count; } }
        
        public bool EstaVacia { get { return count == 0; } }

        public NodoSistemaDrones? GetPrimero()
        {
            return primero;
        }

        public void Limpiar()
        {
            primero = null;
            count = 0;
        }

        public void ParaCada(Action<SistemaDrones> accion)
        {
            NodoSistemaDrones? actual = primero;
            while (actual != null)
            {
                accion(actual.Data);
                actual = actual.Siguiente;
            }
        }
    }
}