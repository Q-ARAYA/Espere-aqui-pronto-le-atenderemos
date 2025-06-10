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
        }

        public Pacientes Paciente { get; set; }
        public string Imagen { get; set; }
        public Especialidades EspecialidadPendiente { get; set; }
    }
}

