namespace Proyecto2.Models
{
    public class InstruccionEmision
    {
        public string NombreDron { get; set; }
        public int AlturaObjetivo { get; set; }
        public char LetraRepresenta { get; set; }
        
        public InstruccionEmision(string nombreDron, int alturaObjetivo, char letraRepresenta)
        {
            NombreDron = nombreDron;
            AlturaObjetivo = alturaObjetivo;
            LetraRepresenta = letraRepresenta;
        }

        public override string ToString()
        {
            return $"{NombreDron},{AlturaObjetivo} - {LetraRepresenta}";
        }
    }
}