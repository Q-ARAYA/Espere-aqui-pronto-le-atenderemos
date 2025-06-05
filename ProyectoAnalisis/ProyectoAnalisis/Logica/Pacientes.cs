using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoAnalisis.Logica
{
    public class Pacientes
    {
        public Pacientes(string nombre, Especialidades especialidad)
        {
            Nombre = nombre;
            Especialidad = especialidad;
        }

        public string Nombre { 
            get; set; 
        }
        public Especialidades Especialidad { 
            get; set; 
        }
    }
}
