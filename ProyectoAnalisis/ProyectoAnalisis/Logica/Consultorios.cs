using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoAnalisis.Logica
{
    public class Consultorios
    {
        // Crea un nuevo consultorio con un numero, un nombre y su estado (activo o no)
        // Tambien inicializa la lista de pacientes en espera
        public Consultorios(int numeroConsultorio, string nombre, bool activo)
        {
            NumeroConsultorio = numeroConsultorio;
            Nombre = $"Consultorio {numeroConsultorio}";
            Activo = activo;
            ColaPacientes = new List<PacientesEnEspera>();
        }

        public int NumeroConsultorio { get; set; }
        public string Nombre { get; set; }
        public List<Especialidades> Especialidades { get; set; } = new List<Especialidades>();
        public bool Activo { get; set; }

        public List<PacientesEnEspera> ColaPacientes { get; set; }

        // Calcula el tiempo total que tardaria en atender a todos los pacientes en la cola
        // Suma la duracion de cada especialidad pendiente de cada paciente en espera
        public int CalcularDuracionTotal()
        {
            if (ColaPacientes == null || ColaPacientes.Count == 0)
                return 0;

            return ColaPacientes.Sum(p => p.EspecialidadPendiente?.Duracion ?? 0);
        }

        // Devuelve el tiempo total de espera en un texto ya formateado, listo para mostrar
        public string ObtenerTiempoEsperaFormateado()
        {
            int duracionMinutos = CalcularDuracionTotal();
            return $"Espera aprox: {duracionMinutos} min";
        }

    }
}