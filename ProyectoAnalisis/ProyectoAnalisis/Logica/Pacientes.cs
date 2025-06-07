using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoAnalisis.Logica
{
    public class Pacientes
    {
        public Pacientes(int id, string nombre, List<Especialidades> especialidades)
        {
            pacienteID = id;
            Nombre = nombre;
            Especialidades = especialidades;
        }

        public string Nombre { 
            get; set; 
        }
        public List<Especialidades> Especialidades { get; set; }

        public int pacienteID { 
            get; set; 
        }
    }
}
