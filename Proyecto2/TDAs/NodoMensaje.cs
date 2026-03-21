using System;

using IPC2_Proyecto2.Models;

namespace IPC2_Proyecto2.TDAs
{
    public class NodoMensaje
    {
        public Mensaje Data{get; set;}
        public NodoMensaje? Siguiente{get; set;}

        public NodoMensaje(Mensaje mensaje)
        {
            Data = mensaje;
            Siguiente = null;
        }

    }
}