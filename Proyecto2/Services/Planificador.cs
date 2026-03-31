

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

            // VALIDAR: Verificar que todos los drones de las instrucciones existan en el sistema
            NodoInstruccionEmision? instActual = mensaje.Instrucciones.GetPrimero();
            bool dronNoEncontrado = false;

            while (instActual != null)
            {
                string nombreDron = instActual.Data.NombreDron;
                if (!sistema.Drones.Existe(nombreDron))
                {
                    Console.WriteLine($"ERROR: El dron '{nombreDron}' no existe en el sistema '{sistema.Nombre}'");
                    dronNoEncontrado = true;
                }
                instActual = instActual.Siguiente;
            }

            if (dronNoEncontrado)
            {
                Console.WriteLine("No se puede calcular el plan. Los drones no coinciden con el sistema.");
                return null;
            }

            Console.WriteLine($"=== Calculando plan para mensaje: {mensaje.Nombre} ===");

            // Obtener lista de drones ordenados alfabéticamente
            ListaDrones dronesSistema = sistema.Drones.ObtenerOrdenadosAlfabeticamente();
            int cantidadDrones = dronesSistema.Count;

            // Lista de nombres de drones en orden alfabético
            ListaNombres nombresDrones = new ListaNombres();
            NodoDron? dronActual = dronesSistema.GetPrimero();
            while (dronActual != null)
            {
                nombresDrones.Agregar(dronActual.Data.Nombre);
                dronActual = dronActual.Siguiente;
            }

            // Altura actual de cada dron (comienzan en 1)
            ListaAlturas alturas = new ListaAlturas();
            for (int i = 0; i < cantidadDrones; i++)
            {
                alturas.Agregar(0); // CORREGIDO: comienzan en 1, no en 0
            }

            // Crear cola de instrucciones por dron
            ListaColasInstrucciones colasPorDron = new ListaColasInstrucciones();
            for (int i = 0; i < cantidadDrones; i++)
            {
                colasPorDron.Agregar(new ColaInstrucciones());
            }

            // Cargar instrucciones en las colas correspondientes
            instActual = mensaje.Instrucciones.GetPrimero();
            while (instActual != null)
            {
                string nombreDron = instActual.Data.NombreDron;
                int dronIdx = nombresDrones.ObtenerIndice(nombreDron);
                if (dronIdx >= 0)
                {
                    ColaInstrucciones? cola = colasPorDron.ObtenerPorIndice(dronIdx);
                    if (cola != null)
                    {
                        cola.Encolar(instActual.Data);
                        Console.WriteLine($"Instrucción para {nombreDron}: altura {instActual.Data.AlturaObjetivo}");
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
            ListaObjetivos objetivo = new ListaObjetivos();
            ListaBooleanos tieneObjetivo = new ListaBooleanos();
            ListaBooleanos listoParaEmitir = new ListaBooleanos();

            for (int i = 0; i < cantidadDrones; i++)
            {
                objetivo.Agregar(1);
                tieneObjetivo.Agregar(false);
                listoParaEmitir.Agregar(false);
            }

            // Asignar primera instrucción a cada dron
            for (int i = 0; i < cantidadDrones; i++)
            {
                ColaInstrucciones? cola = colasPorDron.ObtenerPorIndice(i);
                if (cola != null && !cola.EstaVacia())
                {
                    InstruccionEmision? primera = cola.Desencolar();
                    if (primera != null)
                    {
                        objetivo.Actualizar(i, primera.AlturaObjetivo);
                        tieneObjetivo.Actualizar(i, true);
                        Console.WriteLine($"Asignando a {nombresDrones.Obtener(i)} altura objetivo {primera.AlturaObjetivo}");
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
                    segundo.AgregarAccion(nombresDrones.Obtener(i), "Esperar", alturas.ObtenerPorIndice(i));
                }

                // Mover drones hacia sus objetivos
                for (int i = 0; i < cantidadDrones; i++)
                {
                    if (tieneObjetivo.Obtener(i) && !listoParaEmitir.Obtener(i))
                    {
                        int alturaActual = alturas.ObtenerPorIndice(i);
                        int objetivoActual = objetivo.Obtener(i);

                        if (alturaActual == objetivoActual)
                        {
                            listoParaEmitir.Actualizar(i, true);
                            Console.WriteLine($"Segundo {tiempo}: {nombresDrones.Obtener(i)} llega a altura {alturaActual}");
                        }
                        else if (alturaActual < objetivoActual)
                        {
                            int nuevaAltura = alturaActual + 1;
                            alturas.ActualizarPorIndice(i, nuevaAltura);
                            segundo.ReemplazarAccion(nombresDrones.Obtener(i), "Subir", nuevaAltura);
                            Console.WriteLine($"Segundo {tiempo}: {nombresDrones.Obtener(i)} sube a {nuevaAltura}");
                        }
                        else if (alturaActual > objetivoActual)
                        {
                            int nuevaAltura = alturaActual - 1;
                            alturas.ActualizarPorIndice(i, nuevaAltura);
                            segundo.ReemplazarAccion(nombresDrones.Obtener(i), "Bajar", nuevaAltura);
                            Console.WriteLine($"Segundo {tiempo}: {nombresDrones.Obtener(i)} baja a {nuevaAltura}");
                        }
                    }
                }

                // Emitir en orden de instrucciones
                InstruccionEmision? siguienteEmitir = colaGlobal.VerFrente();
                if (siguienteEmitir != null)
                {
                    int dronIdx = nombresDrones.ObtenerIndice(siguienteEmitir.NombreDron);
                    if (dronIdx >= 0 && listoParaEmitir.Obtener(dronIdx))
                    {
                        int alturaActual = alturas.ObtenerPorIndice(dronIdx);
                        segundo.ReemplazarAccion(nombresDrones.Obtener(dronIdx), "Emitir Luz", alturaActual);
                        char letra = sistema.ObtenerLetra(nombresDrones.Obtener(dronIdx), alturaActual);
                        mensajeRecibido += letra;
                        Console.WriteLine($"Segundo {tiempo}: {nombresDrones.Obtener(dronIdx)} emite '{letra}' a altura {alturaActual}");
                        listoParaEmitir.Actualizar(dronIdx, false);
                        tieneObjetivo.Actualizar(dronIdx, false);
                        colaGlobal.Desencolar();
                        instruccionesProcesadas++;

                        // Asignar siguiente instrucción para este dron si existe
                        ColaInstrucciones? cola = colasPorDron.ObtenerPorIndice(dronIdx);
                        if (cola != null && !cola.EstaVacia())
                        {
                            InstruccionEmision? siguiente = cola.Desencolar();
                            if (siguiente != null)
                            {
                                objetivo.Actualizar(dronIdx, siguiente.AlturaObjetivo);
                                tieneObjetivo.Actualizar(dronIdx, true);
                                Console.WriteLine($"Asignando a {nombresDrones.Obtener(dronIdx)} nueva altura objetivo {siguiente.AlturaObjetivo}");
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
    }
}