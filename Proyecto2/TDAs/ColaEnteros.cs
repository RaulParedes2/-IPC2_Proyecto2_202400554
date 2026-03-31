public class ColaEnteros
{
    private NodoEntero? frente;

    public void Encolar(int valor)
    {
        NodoEntero nuevo = new NodoEntero(valor);
        if (frente == null)
        {
            frente = nuevo;
        }
        else
        {
            NodoEntero aux = frente;
            while (aux.Siguiente != null)
                aux = aux.Siguiente;

            aux.Siguiente = nuevo;
        }
    }

    public int Desencolar()
    {
        if (frente == null) return -1;

        int val = frente.Valor;
        frente = frente.Siguiente;
        return val;
    }

    public bool EstaVacia()
    {
        return frente == null;
    }

    public int Count()
    {
        int c = 0;
        NodoEntero? aux = frente;
        while (aux != null)
        {
            c++;
            aux = aux.Siguiente;
        }
        return c;
    }
}

public class NodoEntero
{
    public int Valor;
    public NodoEntero? Siguiente;

    public NodoEntero(int v)
    {
        Valor = v;
        Siguiente = null;
    }
}

