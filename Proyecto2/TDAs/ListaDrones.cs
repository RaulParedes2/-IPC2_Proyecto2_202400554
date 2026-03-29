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

        // Agregar un dron al final de la lista
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

        // Agregar un dron en una posición específica
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

        // Obtener un dron por su nombre
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

        // Obtener un dron por su índice
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

        // Eliminar un dron por su nombre
        public bool Eliminar(string nombre)
        {
            if (primero == null) return false;

            // Si el dron a eliminar es el primero
            if (primero.Data.Nombre == nombre)
            {
                primero = primero.Siguiente;
                count--;
                return true;
            }

            // Buscar el dron en el resto de la lista
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

        // Verificar si existe un dron con el nombre dado
        public bool Existe(string nombre)
        {
            return ObtenerPorNombre(nombre) != null;
        }

        // Obtener todos los drones ordenados alfabéticamente
        public ListaDrones ObtenerOrdenadosAlfabeticamente()
        {
            ListaDrones ordenada = new ListaDrones();

            if (primero == null)
                return ordenada;

            // Copiar todos los drones a un array temporal
            Dron[] temporal = new Dron[count];
            NodoDron? actual = primero;
            int idx = 0;
            while (actual != null)
            {
                temporal[idx] = actual.Data;
                actual = actual.Siguiente;
                idx++;
            }

            // Ordenar usando burbuja
            for (int i = 0; i < count - 1; i++)
            {
                for (int j = 0; j < count - i - 1; j++)
                {
                    if (string.Compare(temporal[j].Nombre, temporal[j + 1].Nombre) > 0)
                    {
                        Dron temp = temporal[j];
                        temporal[j] = temporal[j + 1];
                        temporal[j + 1] = temp;
                    }
                }
            }

            // Reconstruir lista ordenada
            for (int i = 0; i < count; i++)
            {
                ordenada.Agregar(temporal[i]);
            }

            return ordenada;
        }

        // Método para recorrer la lista y ejecutar una acción en cada elemento
        public void ParaCada(Action<Dron> accion)
        {
            NodoDron? actual = primero;
            while (actual != null)
            {
                accion(actual.Data);
                actual = actual.Siguiente;
            }
        }

        // Método para obtener un string con todos los elementos
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

        // Propiedades
        public int Count { get { return count; } }

        public bool EstaVacia { get { return count == 0; } }

        // Obtener el primer nodo (para iteración manual)
        public NodoDron? GetPrimero()
        {
            return primero;
        }

        // Método para limpiar la lista
        public void Limpiar()
        {
            primero = null;
            count = 0;
        }

        // Método para copiar una lista (crea una nueva lista con los mismos elementos)
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
        public bool Existe(Predicate<Dron> condicion)
        {
            NodoDron? actual = primero;
            while (actual != null)
            {
                if (condicion(actual.Data))
                    return true;
                actual = actual.Siguiente;
            }
            return false;
        }

        // Método para obtener el índice de un dron por su nombre
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
    }
}