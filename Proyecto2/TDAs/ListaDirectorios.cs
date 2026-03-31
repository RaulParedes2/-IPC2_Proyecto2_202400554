namespace Proyecto2.TDAs
{
    public class ListaDirectorios
    {
        public class NodoDirectorio
        {
            public string Data { get; set; }
            public NodoDirectorio? Siguiente { get; set; }
            
            public NodoDirectorio(string data)
            {
                Data = data;
                Siguiente = null;
            }
        }
        
        private NodoDirectorio? primero;
        private int count;
        
        public ListaDirectorios()
        {
            primero = null;
            count = 0;
        }
        
        public void Agregar(string directorio)
        {
            NodoDirectorio nuevo = new NodoDirectorio(directorio);
            
            if (primero == null)
            {
                primero = nuevo;
            }
            else
            {
                NodoDirectorio actual = primero;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevo;
            }
            count++;
        }
        
        public NodoDirectorio? GetPrimero()
        {
            return primero;
        }
        
        public int Count { get { return count; } }
        
        public bool EstaVacia { get { return count == 0; } }
    }
}