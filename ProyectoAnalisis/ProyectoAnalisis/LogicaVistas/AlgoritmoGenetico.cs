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
    }

    public class Individuo
    {
        public List<AsignacionPaciente> Asignaciones { get; set; } = new List<AsignacionPaciente>();
        public int Fitness { get; set; }
    }

    public static class AlgoritmoGenetico
    {
        public static List<List<AsignacionPaciente>> Optimizar(
            List<PacientesEnEspera> pacientes,
            List<Consultorios> consultorios,
            int generaciones = 100,
            int tamPoblacion = 30)
        {
            // Inicialización de la población
            var poblacion = new List<Individuo>();
            var rnd = new Random();

            for (int i = 0; i < tamPoblacion; i++)
            {
                var individuo = new Individuo();
                // En AlgoritmoGenetico.Optimizar, cambia el foreach de especialidades:
                foreach (var paciente in pacientes)
                {
                    var especialidadPendiente = paciente.EspecialidadPendiente;
                    if (especialidadPendiente == null)
                        continue;

                    var consultoriosCompatibles = consultorios
                        .Where(c => c.Activo && c.Especialidades.Any(e => e.Nombre == especialidadPendiente.Nombre))
                        .ToList();

                    if (consultoriosCompatibles.Count > 0)
                    {
                        var consultorio = consultoriosCompatibles[rnd.Next(consultoriosCompatibles.Count)];
                        individuo.Asignaciones.Add(new AsignacionPaciente
                        {
                            Paciente = paciente,
                            Consultorio = consultorio,
                            Especialidad = especialidadPendiente
                        });
                    }
                }
                individuo.Fitness = CalcularFitness(individuo);
                poblacion.Add(individuo);
            }

            // Evolución
            for (int gen = 0; gen < generaciones; gen++)
            {
                // Selección (los mejores)
                var seleccionados = poblacion.OrderBy(x => x.Fitness).Take(tamPoblacion / 2).ToList();

                // Cruce y mutación
                var nuevaPoblacion = new List<Individuo>(seleccionados);
                while (nuevaPoblacion.Count < tamPoblacion)
                {
                    var padre1 = seleccionados[rnd.Next(seleccionados.Count)];
                    var padre2 = seleccionados[rnd.Next(seleccionados.Count)];
                    var hijo = Cruce(padre1, padre2, rnd);
                    Mutar(hijo, consultorios, rnd);
                    hijo.Fitness = CalcularFitness(hijo);
                    nuevaPoblacion.Add(hijo);
                }
                poblacion = nuevaPoblacion;
            }

            // Mejor individuo
            var mejor = poblacion.OrderBy(x => x.Fitness).First();
            // Agrupar por consultorio para mostrar en la UI
            return mejor.Asignaciones
                .GroupBy(a => a.Consultorio.NumeroConsultorio)
                .Select(g => g.ToList())
                .ToList();
        }

        private static int CalcularFitness(Individuo individuo)
        {
            // Suma de tiempos de espera por consultorio (simulación simple)
            var tiemposPorConsultorio = new Dictionary<int, int>();
            foreach (var asignacion in individuo.Asignaciones)
            {
                if (!tiemposPorConsultorio.ContainsKey(asignacion.Consultorio.NumeroConsultorio))
                    tiemposPorConsultorio[asignacion.Consultorio.NumeroConsultorio] = 0;
                tiemposPorConsultorio[asignacion.Consultorio.NumeroConsultorio] += asignacion.Especialidad.Duracion;
            }
            return tiemposPorConsultorio.Values.Max(); // El objetivo es minimizar el tiempo máximo de espera
        }

        private static Individuo Cruce(Individuo padre1, Individuo padre2, Random rnd)
        {
            var hijo = new Individuo();
            for (int i = 0; i < padre1.Asignaciones.Count; i++)
            {
                hijo.Asignaciones.Add(rnd.NextDouble() < 0.5 ? padre1.Asignaciones[i] : padre2.Asignaciones[i]);
            }
            return hijo;
        }

        private static void Mutar(Individuo individuo, List<Consultorios> consultorios, Random rnd)
        {
            // Cambia aleatoriamente la asignación de un paciente a otro consultorio compatible
            if (individuo.Asignaciones.Count == 0) return;
            int idx = rnd.Next(individuo.Asignaciones.Count);
            var asignacion = individuo.Asignaciones[idx];
            var compatibles = consultorios
                .Where(c => c.Activo && c.Especialidades.Any(e => e.Nombre == asignacion.Especialidad.Nombre))
                .ToList();
            if (compatibles.Count > 0)
                asignacion.Consultorio = compatibles[rnd.Next(compatibles.Count)];
        }
    }
}
