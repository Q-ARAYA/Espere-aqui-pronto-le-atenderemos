using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoAnalisis.Logica
{
    public class Pacientes
    {
        public Pacientes(string nombre, string especialidad, int prioridad)
        {
            Nombre = nombre;
            Especialidad = especialidad;
            Prioridad = prioridad;
        }

        public string Nombre { 
            get; set; 
        }
        public string Especialidad { 
            get; set; 
        }
        public int Prioridad { 
            get; set; 
        }
    }
}
