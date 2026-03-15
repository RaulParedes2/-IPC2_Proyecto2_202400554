using IPC2_Proyecto2.Models;
using IPC2_Proyecto2.TDAs;


namespace IPC2_Proyecto2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== PRUEBA DE LISTA DRONES ===\n");

            // Crear lista de drones
            ListaDrones listaDrones = new ListaDrones();

            // Agregar algunos drones
            Console.WriteLine("Agregando drones...");
            listaDrones.Agregar(new Dron("Dron03"));
            listaDrones.Agregar(new Dron("Dron01"));
            listaDrones.Agregar(new Dron("Dron04"));
            listaDrones.Agregar(new Dron("Dron02"));

            Console.WriteLine($"Cantidad de drones: {listaDrones.Count}\n");

            // Mostrar drones en orden de inserción
            Console.WriteLine("Drones en orden de inserción:");
            NodoDron? actual = listaDrones.GetPrimero();
            int i = 1;
            while (actual != null)
            {
                Console.WriteLine($"  {i}. {actual.Data}");
                actual = actual.Siguiente;
                i++;
            }

            // Probar búsqueda
            Console.WriteLine("\nBuscando Dron03...");
            Dron? dronEncontrado = listaDrones.ObtenerPorNombre("Dron03");
            Console.WriteLine(dronEncontrado != null ? $"  Encontrado: {dronEncontrado}" : "  No encontrado");

            // Probar ordenamiento alfabético
            Console.WriteLine("\nDrones ordenados alfabéticamente:");
            ListaDrones ordenados = listaDrones.ObtenerOrdenadosAlfabeticamente();
            NodoDron? nodoOrdenado = ordenados.GetPrimero();
            int j = 1;
            while (nodoOrdenado != null)
            {
                Console.WriteLine($"  {j}. {nodoOrdenado.Data}");
                nodoOrdenado = nodoOrdenado.Siguiente;
                j++;
            }

            // Probar eliminación
            Console.WriteLine("\nEliminando Dron02...");
            bool eliminado = listaDrones.Eliminar("Dron02");
            Console.WriteLine(eliminado ? "  Eliminado correctamente" : "  No se pudo eliminar");

            Console.WriteLine($"\nDrones después de eliminar (total: {listaDrones.Count}):");
            actual = listaDrones.GetPrimero();
            int k = 1;
            while (actual != null)
            {
                Console.WriteLine($"  {k}. {actual.Data}");
                actual = actual.Siguiente;
                k++;
            }

            // Probar método ParaCada
            Console.WriteLine("\nUsando ParaCada para mostrar drones:");
            listaDrones.ParaCada(d => Console.WriteLine($"  - {d}"));

            // Probar método Existe con condición
            Console.WriteLine("\n¿Existe algún dron llamado Dron01?");
            bool existe = listaDrones.Existe(d => d.Nombre == "Dron01");
            Console.WriteLine(existe ? "  Sí" : "  No");

            // Probar acciones de dron
            Console.WriteLine("\n=== PRUEBA DE ACCIONES DE DRON ===");
            Dron dronPrueba = new Dron("DronPrueba", 5);
            Console.WriteLine($"Estado inicial: {dronPrueba}");

            dronPrueba.Subir();
            Console.WriteLine($"Después de subir: {dronPrueba}");

            dronPrueba.Bajar();
            dronPrueba.Bajar();
            Console.WriteLine($"Después de bajar 2 veces: {dronPrueba}");

            Console.WriteLine($"Acción necesaria para altura 3: {dronPrueba.AccionNecesaria(3)}");
            Console.WriteLine($"Acción necesaria para altura 5: {dronPrueba.AccionNecesaria(5)}");

            dronPrueba.EmitirLuz();
            Console.WriteLine($"¿Está emitiendo? {dronPrueba.EstaEmitiendo}");

            //Console.WriteLine("\nPresione cualquier tecla para salir...");
            //Console.ReadKey();
        }
    }
}