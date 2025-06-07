using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoAnalisis.Logica
{
    public class Pacientes
    {
        public Pacientes(int id, string nombre, Especialidades especialidad)
        {
            pacienteID = id;
            Nombre = nombre;
            Especialidad = especialidad;
        }

        public string Nombre { 
            get; set; 
        }
        public Especialidades Especialidad { 
            get; set; 
        }

        public int pacienteID { 
            get; set; 
        }
    }
}
