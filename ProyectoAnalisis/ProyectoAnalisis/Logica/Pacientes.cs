using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoAnalisis.Logica
{
    public class Pacientes
    {
        public Pacientes(string nombre, Especialidades especialidad, int prioridad)
        {
            Nombre = nombre;
            Especialidad = especialidad;
            Prioridad = prioridad;
        }

        public string Nombre { 
            get; set; 
        }
        public Especialidades Especialidad { 
            get; set; 
        }
        public int Prioridad { 
            get; set; 
        }
    }
}
