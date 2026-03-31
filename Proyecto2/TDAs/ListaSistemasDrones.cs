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
        public ListaSistemasDrones ObtenerOrdenadosAlfabeticamente()
        {
            ListaSistemasDrones ordenada = new ListaSistemasDrones();

            if (primero == null)
                return ordenada;

            // Copiar todos los nodos a una nueva lista temporal
            ListaSistemasDrones listaTemp = new ListaSistemasDrones();
            NodoSistemaDrones? actual = primero;
            while (actual != null)
            {
                listaTemp.Agregar(actual.Data);
                actual = actual.Siguiente;
            }

            // Ordenamiento por inserción usando solo nodos (sin arreglos)
            while (!listaTemp.EstaVacia)
            {
                NodoSistemaDrones? primeroTemp = listaTemp.GetPrimero();
                if (primeroTemp != null)
                {
                    SistemaDrones sistemaActual = primeroTemp.Data;
                    listaTemp.Eliminar(sistemaActual.Nombre);

                    if (ordenada.primero == null)
                    {
                        ordenada.Agregar(sistemaActual);
                    }
                    else
                    {
                        NodoSistemaDrones? nodoOrden = ordenada.primero;
                        NodoSistemaDrones? anterior = null;
                        bool insertado = false;

                        while (nodoOrden != null && !insertado)
                        {
                            if (string.Compare(sistemaActual.Nombre, nodoOrden.Data.Nombre) < 0)
                            {
                                NodoSistemaDrones nuevo = new NodoSistemaDrones(sistemaActual);
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
                            ordenada.Agregar(sistemaActual);
                        }
                    }
                }
            }

            return ordenada;
        }
    }
}