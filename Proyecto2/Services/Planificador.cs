/*
using System;
using Proyecto2.Models;
using Proyecto2.Models.Enums;
using Proyecto2.TDAs;

namespace Proyecto2.Services
{
    public class Planificador
    {
        public PlanVuelo? CalcularPlan(Mensaje mensaje, SistemaDrones sistema)
        {
            if (mensaje.Instrucciones.Count == 0)
                return null;

            Console.WriteLine($"=== Calculando plan para mensaje: {mensaje.Nombre} ===");

            // Obtener lista de drones ordenados alfabéticamente
            ListaDrones dronesSistema = sistema.Drones.ObtenerOrdenadosAlfabeticamente();
            int cantidadDrones = dronesSistema.Count;

            // Nombres de drones en orden alfabético
            string[] nombresDrones = new string[cantidadDrones];
            int idx = 0;
            NodoDron? dronActual = dronesSistema.GetPrimero();
            while (dronActual != null)
            {
                nombresDrones[idx] = dronActual.Data.Nombre;
                idx++;
                dronActual = dronActual.Siguiente;
            }

            // Altura actual de cada dron (comienzan en 1)
            int[] alturas = new int[cantidadDrones];
            for (int i = 0; i < cantidadDrones; i++)
            {
                alturas[i] = 1;
            }

            // Crear cola de instrucciones por dron
            ColaInstrucciones[] colasPorDron = new ColaInstrucciones[cantidadDrones];
            for (int i = 0; i < cantidadDrones; i++)
            {
                colasPorDron[i] = new ColaInstrucciones();
            }

            // Cargar instrucciones en las colas correspondientes
            NodoInstruccionEmision? instActual = mensaje.Instrucciones.GetPrimero();
            while (instActual != null)
            {
                string nombreDron = instActual.Data.NombreDron;
                for (int i = 0; i < cantidadDrones; i++)
                {
                    if (nombresDrones[i] == nombreDron)
                    {
                        colasPorDron[i].Encolar(instActual.Data);
                        Console.WriteLine($"Instrucción para {nombreDron}: altura {instActual.Data.AlturaObjetivo}");
                        break;
                    }
                }
                instActual = instActual.Siguiente;
            }

            PlanVuelo plan = new PlanVuelo();
            plan.NombreMensaje = mensaje.Nombre;
            plan.NombreSistema = sistema.Nombre;
            plan.MensajeOriginal = mensaje.TextoOriginal;

            int tiempo = 1;
            string mensajeRecibido = "";
            int instruccionesProcesadas = 0;
            int totalInstrucciones = mensaje.Instrucciones.Count;

            // Estado de cada dron
            int[] objetivo = new int[cantidadDrones];
            bool[] tieneObjetivo = new bool[cantidadDrones];
            bool[] listoParaEmitir = new bool[cantidadDrones];

            for (int i = 0; i < cantidadDrones; i++)
            {
                objetivo[i] = 1;
                tieneObjetivo[i] = false;
                listoParaEmitir[i] = false;
            }

            // Asignar primera instrucción a cada dron (movimiento paralelo desde el inicio)
            for (int i = 0; i < cantidadDrones; i++)
            {
                if (!colasPorDron[i].EstaVacia())
                {
                    InstruccionEmision? primera = colasPorDron[i].Desencolar();
                    if (primera != null)
                    {
                        objetivo[i] = primera.AlturaObjetivo;
                        tieneObjetivo[i] = true;
                        Console.WriteLine($"Asignando a {nombresDrones[i]} altura objetivo {objetivo[i]}");
                    }
                }
            }

            // Cola de instrucciones en orden global (para saber qué emitir después)
            ColaInstrucciones colaGlobal = new ColaInstrucciones();
            instActual = mensaje.Instrucciones.GetPrimero();
            while (instActual != null)
            {
                colaGlobal.Encolar(instActual.Data);
                instActual = instActual.Siguiente;
            }

            while (instruccionesProcesadas < totalInstrucciones)
            {
                PlanVuelo.AccionPorSegundo segundo = new PlanVuelo.AccionPorSegundo(tiempo);

                // Inicializar todos con Esperar
                for (int i = 0; i < cantidadDrones; i++)
                {
                    segundo.AgregarAccion(nombresDrones[i], "Esperar", alturas[i]);
                }

                // Mover drones hacia sus objetivos
                for (int i = 0; i < cantidadDrones; i++)
                {
                    if (tieneObjetivo[i] && !listoParaEmitir[i])
                    {
                        if (alturas[i] == objetivo[i])
                        {
                            listoParaEmitir[i] = true;
                            Console.WriteLine($"Segundo {tiempo}: {nombresDrones[i]} llega a altura {alturas[i]}");
                        }
                        else if (alturas[i] < objetivo[i])
                        {
                            alturas[i]++;
                            segundo.ReemplazarAccion(nombresDrones[i], "Subir", alturas[i]);
                            Console.WriteLine($"Segundo {tiempo}: {nombresDrones[i]} sube a {alturas[i]}");
                        }
                        else if (alturas[i] > objetivo[i])
                        {
                            alturas[i]--;
                            segundo.ReemplazarAccion(nombresDrones[i], "Bajar", alturas[i]);
                            Console.WriteLine($"Segundo {tiempo}: {nombresDrones[i]} baja a {alturas[i]}");
                        }
                    }
                }

                // Emitir en orden de instrucciones (la siguiente instrucción en la cola global que esté lista)
                InstruccionEmision? siguienteEmitir = null;
                if (!colaGlobal.EstaVacia())
                {
                    siguienteEmitir = colaGlobal.VerFrente();
                }

                if (siguienteEmitir != null)
                {
                    int dronIdx = ObtenerIndiceDron(nombresDrones, siguienteEmitir.NombreDron);
                    if (dronIdx >= 0 && listoParaEmitir[dronIdx])
                    {
                        segundo.ReemplazarAccion(nombresDrones[dronIdx], "Emitir Luz", alturas[dronIdx]);
                        char letra = sistema.ObtenerLetra(nombresDrones[dronIdx], alturas[dronIdx]);
                        mensajeRecibido += letra;
                        Console.WriteLine($"Segundo {tiempo}: {nombresDrones[dronIdx]} emite '{letra}' a altura {alturas[dronIdx]}");
                        listoParaEmitir[dronIdx] = false;
                        tieneObjetivo[dronIdx] = false;
                        colaGlobal.Desencolar();
                        instruccionesProcesadas++;

                        // En la parte de asignar siguiente instrucción después de emitir
                        if (!colasPorDron[dronIdx].EstaVacia())
                        {
                            InstruccionEmision? siguiente = colasPorDron[dronIdx].Desencolar();
                            if (siguiente != null)
                            {
                                objetivo[dronIdx] = siguiente.AlturaObjetivo;
                                tieneObjetivo[dronIdx] = true;
                                // IMPORTANTE: No reiniciar listoParaEmitir aquí, el dron necesita moverse
                                Console.WriteLine($"Asignando a {nombresDrones[dronIdx]} nueva altura objetivo {objetivo[dronIdx]}");
                            }
                        }
                    }
                }

                plan.AgregarSegundo(segundo);
                tiempo++;

                if (tiempo > 500) break;
            }

            plan.MensajeRecibido = mensajeRecibido;
            plan.TiempoOptimo = tiempo - 1;

            Console.WriteLine($"Plan completado en {plan.TiempoOptimo} segundos");
            Console.WriteLine($"Mensaje recibido: {mensajeRecibido}");

            return plan;
        }

        private int ObtenerIndiceDron(string[] nombres, string nombreDron)
        {
            for (int i = 0; i < nombres.Length; i++)
            {
                if (nombres[i] == nombreDron)
                    return i;
            }
            return -1;
        }
    }
}
*/

using System;
using Proyecto2.Models;
using Proyecto2.Models.Enums;
using Proyecto2.TDAs;

namespace Proyecto2.Services
{
    public class Planificador
    {
        public PlanVuelo? CalcularPlan(Mensaje mensaje, SistemaDrones sistema)
        {
            if (mensaje.Instrucciones.Count == 0)
                return null;

            Console.WriteLine($"=== Calculando plan para mensaje: {mensaje.Nombre} ===");

            // Obtener lista de drones ordenados alfabéticamente
            ListaDrones dronesSistema = sistema.Drones.ObtenerOrdenadosAlfabeticamente();
            int cantidadDrones = dronesSistema.Count;
            
            // Nombres de drones en orden alfabético
            string[] nombresDrones = new string[cantidadDrones];
            int idx = 0;
            NodoDron? dronActual = dronesSistema.GetPrimero();
            while (dronActual != null)
            {
                nombresDrones[idx] = dronActual.Data.Nombre;
                idx++;
                dronActual = dronActual.Siguiente;
            }

            // Altura actual de cada dron (comienzan en 1)
            int[] alturas = new int[cantidadDrones];
            for (int i = 0; i < cantidadDrones; i++)
            {
                alturas[i] = 0;
            }

            // Crear cola de instrucciones por dron
            ColaInstrucciones[] colasPorDron = new ColaInstrucciones[cantidadDrones];
            for (int i = 0; i < cantidadDrones; i++)
            {
                colasPorDron[i] = new ColaInstrucciones();
            }

            // Cargar instrucciones en las colas correspondientes
            NodoInstruccionEmision? instActual = mensaje.Instrucciones.GetPrimero();
            while (instActual != null)
            {
                string nombreDron = instActual.Data.NombreDron;
                for (int i = 0; i < cantidadDrones; i++)
                {
                    if (nombresDrones[i] == nombreDron)
                    {
                        colasPorDron[i].Encolar(instActual.Data);
                        Console.WriteLine($"Instrucción para {nombreDron}: altura {instActual.Data.AlturaObjetivo}");
                        break;
                    }
                }
                instActual = instActual.Siguiente;
            }

            PlanVuelo plan = new PlanVuelo();
            plan.NombreMensaje = mensaje.Nombre;
            plan.NombreSistema = sistema.Nombre;
            plan.MensajeOriginal = mensaje.TextoOriginal;

            int tiempo = 1;
            string mensajeRecibido = "";
            int instruccionesProcesadas = 0;
            int totalInstrucciones = mensaje.Instrucciones.Count;

            // Estado de cada dron
            int[] objetivo = new int[cantidadDrones];
            bool[] tieneObjetivo = new bool[cantidadDrones];
            bool[] listoParaEmitir = new bool[cantidadDrones];
            bool[] haEmitido = new bool[cantidadDrones];
            
            for (int i = 0; i < cantidadDrones; i++)
            {
                objetivo[i] = 1;
                tieneObjetivo[i] = false;
                listoParaEmitir[i] = false;
                haEmitido[i] = false;
            }

            // Asignar primera instrucción a cada dron
            for (int i = 0; i < cantidadDrones; i++)
            {
                if (!colasPorDron[i].EstaVacia())
                {
                    InstruccionEmision? primera = colasPorDron[i].Desencolar();
                    if (primera != null)
                    {
                        objetivo[i] = primera.AlturaObjetivo;
                        tieneObjetivo[i] = true;
                        Console.WriteLine($"Asignando a {nombresDrones[i]} altura objetivo {objetivo[i]}");
                    }
                }
            }

            // Cola global de instrucciones en orden
            ColaInstrucciones colaGlobal = new ColaInstrucciones();
            instActual = mensaje.Instrucciones.GetPrimero();
            while (instActual != null)
            {
                colaGlobal.Encolar(instActual.Data);
                instActual = instActual.Siguiente;
            }

            while (instruccionesProcesadas < totalInstrucciones)
            {
                PlanVuelo.AccionPorSegundo segundo = new PlanVuelo.AccionPorSegundo(tiempo);
                
                // Inicializar todos con Esperar
                for (int i = 0; i < cantidadDrones; i++)
                {
                    segundo.AgregarAccion(nombresDrones[i], "Esperar", alturas[i]);
                }
                
                // Mover drones hacia sus objetivos
                for (int i = 0; i < cantidadDrones; i++)
                {
                    if (tieneObjetivo[i] && !listoParaEmitir[i])
                    {
                        if (alturas[i] == objetivo[i])
                        {
                            listoParaEmitir[i] = true;
                            Console.WriteLine($"Segundo {tiempo}: {nombresDrones[i]} llega a altura {alturas[i]}");
                        }
                        else if (alturas[i] < objetivo[i])
                        {
                            alturas[i]++;
                            segundo.ReemplazarAccion(nombresDrones[i], "Subir", alturas[i]);
                            Console.WriteLine($"Segundo {tiempo}: {nombresDrones[i]} sube a {alturas[i]}");
                        }
                        else if (alturas[i] > objetivo[i])
                        {
                            alturas[i]--;
                            segundo.ReemplazarAccion(nombresDrones[i], "Bajar", alturas[i]);
                            Console.WriteLine($"Segundo {tiempo}: {nombresDrones[i]} baja a {alturas[i]}");
                        }
                    }
                }
                
                // Emitir en orden de instrucciones
                InstruccionEmision? siguienteEmitir = colaGlobal.VerFrente();
                if (siguienteEmitir != null)
                {
                    int dronIdx = ObtenerIndiceDron(nombresDrones, siguienteEmitir.NombreDron);
                    if (dronIdx >= 0 && listoParaEmitir[dronIdx])
                    {
                        segundo.ReemplazarAccion(nombresDrones[dronIdx], "Emitir Luz", alturas[dronIdx]);
                        char letra = sistema.ObtenerLetra(nombresDrones[dronIdx], alturas[dronIdx]);
                        mensajeRecibido += letra;
                        Console.WriteLine($"Segundo {tiempo}: {nombresDrones[dronIdx]} emite '{letra}' a altura {alturas[dronIdx]}");
                        listoParaEmitir[dronIdx] = false;
                        tieneObjetivo[dronIdx] = false;
                        haEmitido[dronIdx] = true;
                        colaGlobal.Desencolar();
                        instruccionesProcesadas++;
                        
                        // Asignar siguiente instrucción para este dron si existe
                        if (!colasPorDron[dronIdx].EstaVacia())
                        {
                            InstruccionEmision? siguiente = colasPorDron[dronIdx].Desencolar();
                            if (siguiente != null)
                            {
                                objetivo[dronIdx] = siguiente.AlturaObjetivo;
                                tieneObjetivo[dronIdx] = true;
                                listoParaEmitir[dronIdx] = false;
                                Console.WriteLine($"Asignando a {nombresDrones[dronIdx]} nueva altura objetivo {objetivo[dronIdx]}");
                            }
                        }
                    }
                }
                
                plan.AgregarSegundo(segundo);
                tiempo++;
                
                if (tiempo > 500) break;
            }

            plan.MensajeRecibido = mensajeRecibido;
            plan.TiempoOptimo = tiempo - 1;
            
            Console.WriteLine($"Plan completado en {plan.TiempoOptimo} segundos");
            Console.WriteLine($"Mensaje recibido: {mensajeRecibido}");
            
            return plan;
        }
        
        private int ObtenerIndiceDron(string[] nombres, string nombreDron)
        {
            for (int i = 0; i < nombres.Length; i++)
            {
                if (nombres[i] == nombreDron)
                    return i;
            }
            return -1;
        }
    }
}