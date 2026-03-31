using System;
using Proyecto2.Models;

namespace Proyecto2.TDAs
{
    public class ListaDrones
    {
        private NodoDron? primero;
        private int count;

        public ListaDrones()
        {
            primero = null;
            count = 0;
        }

        public void Agregar(Dron dron)
        {
            NodoDron nuevo = new NodoDron(dron);

            if (primero == null)
            {
                primero = nuevo;
            }
            else
            {
                NodoDron actual = primero;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevo;
            }
            count++;
        }

        public bool Insertar(int indice, Dron dron)
        {
            if (indice < 0 || indice > count)
                return false;

            NodoDron nuevo = new NodoDron(dron);

            if (indice == 0)
            {
                nuevo.Siguiente = primero;
                primero = nuevo;
            }
            else
            {
                NodoDron? actual = primero;
                for (int i = 0; i < indice - 1; i++)
                {
                    actual = actual!.Siguiente;
                }
                nuevo.Siguiente = actual!.Siguiente;
                actual.Siguiente = nuevo;
            }
            count++;
            return true;
        }

        public Dron? ObtenerPorNombre(string nombre)
        {
            NodoDron? actual = primero;
            while (actual != null)
            {
                if (actual.Data.Nombre == nombre)
                    return actual.Data;
                actual = actual.Siguiente;
            }
            return null;
        }

        public Dron? ObtenerPorIndice(int indice)
        {
            if (indice < 0 || indice >= count)
                return null;

            NodoDron? actual = primero;
            for (int i = 0; i < indice; i++)
            {
                actual = actual!.Siguiente;
            }
            return actual!.Data;
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

            NodoDron actual = primero;
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

        public ListaDrones ObtenerOrdenadosAlfabeticamente()
        {
            ListaDrones ordenada = new ListaDrones();

            if (primero == null)
                return ordenada;

            ListaDrones listaTemp = new ListaDrones();
            NodoDron? actual = primero;
            while (actual != null)
            {
                listaTemp.Agregar(actual.Data);
                actual = actual.Siguiente;
            }

            while (!listaTemp.EstaVacia)
            {
                NodoDron? primeroTemp = listaTemp.GetPrimero();
                if (primeroTemp != null)
                {
                    Dron dronActual = primeroTemp.Data;
                    listaTemp.Eliminar(dronActual.Nombre);

                    if (ordenada.primero == null)
                    {
                        ordenada.Agregar(dronActual);
                    }
                    else
                    {
                        NodoDron? nodoOrden = ordenada.primero;
                        NodoDron? anterior = null;
                        bool insertado = false;

                        while (nodoOrden != null && !insertado)
                        {
                            if (string.Compare(dronActual.Nombre, nodoOrden.Data.Nombre) < 0)
                            {
                                NodoDron nuevo = new NodoDron(dronActual);
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
                            ordenada.Agregar(dronActual);
                        }
                    }
                }
            }

            return ordenada;
        }

        public void ParaCada(Action<Dron> accion)
        {
            NodoDron? actual = primero;
            while (actual != null)
            {
                accion(actual.Data);
                actual = actual.Siguiente;
            }
        }

        public string ObtenerListadoComoString()
        {
            string resultado = "";
            NodoDron? actual = primero;
            while (actual != null)
            {
                resultado += actual.Data.ToString();
                if (actual.Siguiente != null)
                    resultado += "\n";
                actual = actual.Siguiente;
            }
            return resultado;
        }

        public int Count { get { return count; } }

        public bool EstaVacia { get { return count == 0; } }

        public NodoDron? GetPrimero()
        {
            return primero;
        }

        public void Limpiar()
        {
            primero = null;
            count = 0;
        }

        public ListaDrones Copiar()
        {
            ListaDrones copia = new ListaDrones();
            NodoDron? actual = primero;
            while (actual != null)
            {
                copia.Agregar(new Dron(actual.Data.Nombre, actual.Data.AlturaInicial));
                actual = actual.Siguiente;
            }
            return copia;
        }

        // Método para buscar si existe algún dron que cumpla una condición
        // Reemplazamos Predicate<Dron> por un método específico
        public bool ExisteDronConNombre(string nombre)
        {
            return Existe(nombre);
        }

        public bool ExisteDronConAltura(int altura)
        {
            NodoDron? actual = primero;
            while (actual != null)
            {
                if (actual.Data.AlturaActual == altura)
                    return true;
                actual = actual.Siguiente;
            }
            return false;
        }

        public int IndiceDe(string nombre)
        {
            NodoDron? actual = primero;
            int indice = 0;
            while (actual != null)
            {
                if (actual.Data.Nombre == nombre)
                    return indice;
                actual = actual.Siguiente;
                indice++;
            }
            return -1;
        }

        /*
        public bool ExisteDronConAlturaMayorQue(int altura)
{
    NodoDron? actual = primero;
    while (actual != null)
    {
        if (actual.Data.AlturaActual > altura)
            return true;
        actual = actual.Siguiente;
    }
    return false;
}

public bool ExisteDronConNombreQueContenga(string texto)
{
    NodoDron? actual = primero;
    while (actual != null)
    {
        if (actual.Data.Nombre.Contains(texto))
            return true;
        actual = actual.Siguiente;
    }
    return false;
}
        */
    }
}