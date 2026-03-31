namespace Proyecto2.TDAs
{
    public class ListaRutas
    {
        public class NodoRuta
        {
            public string Data { get; set; }
            public NodoRuta? Siguiente { get; set; }
            
            public NodoRuta(string data)
            {
                Data = data;
                Siguiente = null;
            }
        }
        
        private NodoRuta? primero;
        private int count;
        
        public ListaRutas()
        {
            primero = null;
            count = 0;
        }
        
        public void Agregar(string ruta)
        {
            NodoRuta nuevo = new NodoRuta(ruta);
            
            if (primero == null)
            {
                primero = nuevo;
            }
            else
            {
                NodoRuta actual = primero;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevo;
            }
            count++;
        }
        
        public NodoRuta? GetPrimero()
        {
            return primero;
        }
        
        public int Count { get { return count; } }
        
        public bool EstaVacia { get { return count == 0; } }
    }
}