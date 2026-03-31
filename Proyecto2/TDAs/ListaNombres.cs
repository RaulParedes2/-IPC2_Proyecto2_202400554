namespace Proyecto2.TDAs
{
    public class ListaNombres
    {
        public class NodoNombre
        {
            public string Data { get; set; }
            public NodoNombre? Siguiente { get; set; }

            public NodoNombre(string data)
            {
                Data = data;
                Siguiente = null;
            }
        }

        private NodoNombre? primero;
        private int count;

        public ListaNombres()
        {
            primero = null;
            count = 0;
        }

        public void Agregar(string nombre)
        {
            NodoNombre nuevo = new NodoNombre(nombre);

            if (primero == null)
            {
                primero = nuevo;
            }
            else
            {
                NodoNombre actual = primero;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevo;
            }
            count++;
        }

        public string Obtener(int indice)
        {
            if (indice < 0 || indice >= count)
                return "";

            NodoNombre actual = primero!;
            for (int i = 0; i < indice; i++)
            {
                actual = actual.Siguiente!;
            }
            return actual.Data;
        }

        public int ObtenerIndice(string nombre)
        {
            NodoNombre? actual = primero;
            int indice = 0;
            while (actual != null)
            {
                if (actual.Data == nombre)
                    return indice;
                actual = actual.Siguiente;
                indice++;
            }
            return -1;
        }

        public int Count { get { return count; } }
    }
}