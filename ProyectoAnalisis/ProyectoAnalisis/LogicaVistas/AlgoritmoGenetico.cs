using ProyectoAnalisis.Logica;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoAnalisis.LogicaVistas
{
    public class AsignacionPaciente
    {
        public PacientesEnEspera Paciente { get; set; }
        public Consultorios Consultorio { get; set; }
        public Especialidades Especialidad { get; set; }
        public int OrdenEnCola { get; set; } 
    }

    public class Individuo
    {
        public List<AsignacionPaciente> Asignaciones { get; set; } = new List<AsignacionPaciente>();
        public double Fitness { get; set; }

        // Método para obtener las colas organizadas por consultorio
        public Dictionary<int, List<AsignacionPaciente>> ObtenerColasPorConsultorio()
        {
            return Asignaciones
                .GroupBy(a => a.Consultorio.NumeroConsultorio)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(a => a.OrdenEnCola).ToList()
                );
        }
    }

    public static class AlgoritmoGenetico
    {
        public static List<List<AsignacionPaciente>> Optimizar(
            List<PacientesEnEspera> pacientes,
            List<Consultorios> consultorios,
            int generaciones = 200, // Aumentar generaciones
            int tamPoblacion = 50)   // Aumentar población
        {
            var poblacion = new List<Individuo>();
            var rnd = new Random();

            // Crear población inicial 
            for (int i = 0; i < tamPoblacion; i++)
            {
                var individuo = CrearIndividuoAleatorio(pacientes, consultorios, rnd);
                individuo.Fitness = CalcularFitness(individuo);
                poblacion.Add(individuo);
            }

            // Evolución
            for (int gen = 0; gen < generaciones; gen++)
            {
                // Selección por torneo
                var seleccionados = SeleccionPorTorneo(poblacion, tamPoblacion / 2, rnd);

                var nuevaPoblacion = new List<Individuo>();

                // Mantener el mejor individuo
                var mejor = poblacion.OrderBy(x => x.Fitness).First();
                nuevaPoblacion.Add(CopiarIndividuo(mejor));

                // Generar descendencia
                while (nuevaPoblacion.Count < tamPoblacion)
                {
                    var padre1 = seleccionados[rnd.Next(seleccionados.Count)];
                    var padre2 = seleccionados[rnd.Next(seleccionados.Count)];

                    var hijo = Cruce(padre1, padre2, rnd);
                    Mutacion(hijo, consultorios, rnd);
                    hijo.Fitness = CalcularFitness(hijo);
                    nuevaPoblacion.Add(hijo);
                }
             
                poblacion = nuevaPoblacion;

                // mostrar progreso cada 20 generaciones
                if (gen % 20 == 0)
                {
                    var mejorActual = poblacion.OrderBy(x => x.Fitness).First();
                    Console.WriteLine($"Generación {gen}: Fitness = {mejorActual.Fitness}");
                }
            }

            var mejorSolucion = poblacion.OrderBy(x => x.Fitness).First();
            return mejorSolucion.ObtenerColasPorConsultorio().Values.ToList();
        }

        private static Individuo CrearIndividuoAleatorio(
            List<PacientesEnEspera> pacientes,
            List<Consultorios> consultorios,
            Random rnd)
        {
            var individuo = new Individuo();
            var contadoresOrden = new Dictionary<int, int>();

            // Inicializar contadores de orden para cada consultorio
            foreach (var consultorio in consultorios)
            {
                contadoresOrden[consultorio.NumeroConsultorio] = 0;
            }

            // Crear todas las asignaciones necesarias
            foreach (var paciente in pacientes)
            {
                foreach (var especialidad in paciente.Paciente.Especialidades)
                {
                    var consultoriosCompatibles = consultorios
                        .Where(c => c.Activo && c.Especialidades.Any(e => e.Nombre == especialidad.Nombre))
                        .ToList();

                    if (consultoriosCompatibles.Count > 0)
                    {
                        var consultorio = consultoriosCompatibles[rnd.Next(consultoriosCompatibles.Count)];

                        individuo.Asignaciones.Add(new AsignacionPaciente
                        {
                            Paciente = paciente,
                            Consultorio = consultorio,
                            Especialidad = especialidad,
                            OrdenEnCola = contadoresOrden[consultorio.NumeroConsultorio]++
                        });
                    }
                }
            }

            MezclarOrdenesAleatoriamente(individuo, rnd);

            return individuo;
        }

        private static void MezclarOrdenesAleatoriamente(Individuo individuo, Random rnd)
        {
            var colasPorConsultorio = individuo.ObtenerColasPorConsultorio();

            foreach (var kvp in colasPorConsultorio)
            {
                var cola = kvp.Value;
                
                for (int i = cola.Count - 1; i > 0; i--)
                {
                    int j = rnd.Next(i + 1);
                    cola[i].OrdenEnCola = j;
                    cola[j].OrdenEnCola = i;
                }

                
                for (int i = 0; i < cola.Count; i++)
                {
                    cola[i].OrdenEnCola = i;
                }
            }
        }

        private static double CalcularFitness(Individuo individuo)
        {
            double penalizacion = 0;
            var tiempoFinalizacionPorConsultorio = new Dictionary<int, int>();
            var tiemposPacientes = new Dictionary<Pacientes, List<(int inicio, int fin, int consultorio)>>();

            // Simular la atención en cada consultorio
            var colasPorConsultorio = individuo.ObtenerColasPorConsultorio();

            foreach (var kvp in colasPorConsultorio)
            {
                int consultorio = kvp.Key;
                var cola = kvp.Value;
                int tiempoActual = 0;

                foreach (var asignacion in cola)
                {
                    int tiempoInicio = tiempoActual;
                    int tiempoFin = tiempoActual + asignacion.Especialidad.Duracion;

                    // Registrar tiempo del paciente
                    if (!tiemposPacientes.ContainsKey(asignacion.Paciente.Paciente))
                        tiemposPacientes[asignacion.Paciente.Paciente] = new List<(int, int, int)>();

                    tiemposPacientes[asignacion.Paciente.Paciente].Add((tiempoInicio, tiempoFin, consultorio));

                    tiempoActual = tiempoFin;
                }

                tiempoFinalizacionPorConsultorio[consultorio] = tiempoActual;
            }

            // Verificar restricciones de pacientes
            foreach (var kvp in tiemposPacientes)
            {
                var paciente = kvp.Key;
                var tiempos = kvp.Value;

                // Verificar que tiene todas las especialidades requeridas
                var especialidadesAtendidas = individuo.Asignaciones
                    .Where(a => a.Paciente.Paciente == paciente)
                    .Select(a => a.Especialidad.Nombre)
                    .ToHashSet();

                var especialidadesRequeridas = paciente.Especialidades
                    .Select(e => e.Nombre)
                    .ToHashSet();

                if (!especialidadesRequeridas.IsSubsetOf(especialidadesAtendidas))
                {
                    penalizacion += 10000; // Penalización muy alta por especialidades faltantes
                }

                // Verificar que no se atiende en diferentes consultorios a la vez
                for (int i = 0; i < tiempos.Count; i++)
                {
                    for (int j = i + 1; j < tiempos.Count; j++)
                    {
                        if (tiempos[i].consultorio != tiempos[j].consultorio)
                        {
                            
                            if (!(tiempos[i].fin <= tiempos[j].inicio || tiempos[j].fin <= tiempos[i].inicio))
                            {
                                penalizacion += 5000; // Penalización por estar en dos consultorios a la vez
                            }
                        }
                    }
                }

                // Penalizar por tiempo total de espera del paciente
                if (tiempos.Count > 0)
                {
                    int tiempoInicioMinimo = tiempos.Min(t => t.inicio);
                    int tiempoFinMaximo = tiempos.Max(t => t.fin);
                    int tiempoTotalPaciente = tiempoFinMaximo - tiempoInicioMinimo;
                    penalizacion += tiempoTotalPaciente * 0.1; // Penalización leve por tiempo total
                }
            }

            
            int tiempoFinal = tiempoFinalizacionPorConsultorio.Count > 0 ?
                tiempoFinalizacionPorConsultorio.Values.Max() : 0;

            
            if (tiempoFinalizacionPorConsultorio.Count > 1)
            {
                int tiempoMinimo = tiempoFinalizacionPorConsultorio.Values.Min();
                int desbalance = tiempoFinal - tiempoMinimo;
                penalizacion += desbalance * 0.5; // Penalización por desbalance
            }

            return tiempoFinal + penalizacion;
        }

        private static List<Individuo> SeleccionPorTorneo(List<Individuo> poblacion, int cantidad, Random rnd)
        {
            var seleccionados = new List<Individuo>();
            int tamTorneo = 3; 

            for (int i = 0; i < cantidad; i++)
            {
                var participantes = new List<Individuo>();
                for (int j = 0; j < tamTorneo; j++)
                {
                    participantes.Add(poblacion[rnd.Next(poblacion.Count)]);
                }

                var ganador = participantes.OrderBy(p => p.Fitness).First();
                seleccionados.Add(CopiarIndividuo(ganador));
            }

            return seleccionados;
        }

        private static Individuo Cruce(Individuo padre1, Individuo padre2, Random rnd)
        {
            var hijo = new Individuo();

            // Obtener todas las asignaciones únicas (paciente-especialidad)
            var asignacionesPadre1 = padre1.Asignaciones.ToList();
            var asignacionesPadre2 = padre2.Asignaciones.ToList();

            // elegir aleatoriamente de que padre heredar cada asignación
            foreach (var asignacion1 in asignacionesPadre1)
            {
                var asignacion2 = asignacionesPadre2.FirstOrDefault(a =>
                    a.Paciente.Paciente == asignacion1.Paciente.Paciente &&
                    a.Especialidad.Nombre == asignacion1.Especialidad.Nombre);

                if (asignacion2 != null)
                {
                    // Elegir aleatoriamente entre padre1 y padre2
                    var asignacionElegida = rnd.NextDouble() < 0.5 ? asignacion1 : asignacion2;

                    hijo.Asignaciones.Add(new AsignacionPaciente
                    {
                        Paciente = asignacionElegida.Paciente,
                        Consultorio = asignacionElegida.Consultorio,
                        Especialidad = asignacionElegida.Especialidad,
                        OrdenEnCola = asignacionElegida.OrdenEnCola
                    });
                }
                else
                {
                    // Si no existe en padre2, tomar de padre1
                    hijo.Asignaciones.Add(new AsignacionPaciente
                    {
                        Paciente = asignacion1.Paciente,
                        Consultorio = asignacion1.Consultorio,
                        Especialidad = asignacion1.Especialidad,
                        OrdenEnCola = asignacion1.OrdenEnCola
                    });
                }
            }

            // Reajustar órdenes en cola para que sean consecutivos
            ReajustarOrdenes(hijo);

            return hijo;
        }

        private static void Mutacion(Individuo individuo, List<Consultorios> consultorios, Random rnd)
        {
            if (individuo.Asignaciones.Count == 0) return;

            double probabilidadMutacion = 0.01;

            // Mutaccion cambiar consultorio (si hay opciones)
            if (rnd.NextDouble() < probabilidadMutacion)
            {
                int idx = rnd.Next(individuo.Asignaciones.Count);
                var asignacion = individuo.Asignaciones[idx];

                var consultoriosCompatibles = consultorios
                    .Where(c => c.Activo &&
                               c.Especialidades.Any(e => e.Nombre == asignacion.Especialidad.Nombre) &&
                               c.NumeroConsultorio != asignacion.Consultorio.NumeroConsultorio)
                    .ToList();

                if (consultoriosCompatibles.Count > 0)
                {
                    asignacion.Consultorio = consultoriosCompatibles[rnd.Next(consultoriosCompatibles.Count)];
                    ReajustarOrdenes(individuo);
                }
            }

            // Mutación reordenar cola de un consultorio
            if (rnd.NextDouble() < probabilidadMutacion)
            {
                var colasPorConsultorio = individuo.ObtenerColasPorConsultorio();
                if (colasPorConsultorio.Count > 0)
                {
                    var consultorioElegido = colasPorConsultorio.Keys.ElementAt(rnd.Next(colasPorConsultorio.Count));
                    var cola = colasPorConsultorio[consultorioElegido];

                    if (cola.Count > 1)
                    {
                        // Intercambiar dos posiciones aleatorias
                        int pos1 = rnd.Next(cola.Count);
                        int pos2 = rnd.Next(cola.Count);

                        if (pos1 != pos2)
                        {
                            var temp = cola[pos1].OrdenEnCola;
                            cola[pos1].OrdenEnCola = cola[pos2].OrdenEnCola;
                            cola[pos2].OrdenEnCola = temp;
                        }
                    }
                }
            }
        }

        private static void ReajustarOrdenes(Individuo individuo)
        {
            var colasPorConsultorio = individuo.ObtenerColasPorConsultorio();

            foreach (var kvp in colasPorConsultorio)
            {
                var cola = kvp.Value;
                for (int i = 0; i < cola.Count; i++)
                {
                    cola[i].OrdenEnCola = i;
                }
            }
        }

        private static Individuo CopiarIndividuo(Individuo original)
        {
            var copia = new Individuo
            {
                Fitness = original.Fitness
            };

            foreach (var asignacion in original.Asignaciones)
            {
                copia.Asignaciones.Add(new AsignacionPaciente
                {
                    Paciente = asignacion.Paciente,
                    Consultorio = asignacion.Consultorio,
                    Especialidad = asignacion.Especialidad,
                    OrdenEnCola = asignacion.OrdenEnCola
                });
            }

            return copia;
        }
    }
}
