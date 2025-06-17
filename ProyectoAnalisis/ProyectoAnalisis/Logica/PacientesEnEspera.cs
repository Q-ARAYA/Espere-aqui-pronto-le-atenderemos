using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoAnalisis.Logica
{
    public class PacientesEnEspera
    {
        public PacientesEnEspera(Pacientes paciente, string imagen)
        {
            Paciente = paciente;
            Imagen = imagen;
            EspecialidadPendiente = paciente.Especialidades.FirstOrDefault();
            TiempoEspera = 0;
            Prioridad = 0;
        }

        public Pacientes Paciente { get; set; }
        public string Imagen { get; set; }
        public Especialidades EspecialidadPendiente { get; set; }
        public int TiempoEspera { get; set; }
        public int Prioridad { get; set; }

        // Método para aumentar la prioridad basado en el tiempo de espera
        public void IncrementarPrioridad(bool especialidadDisponible)
        {
            TiempoEspera++;
            Prioridad += especialidadDisponible ? 1 : 3; // Mayor prioridad si no hay especialidad disponible
        }
    }
}

