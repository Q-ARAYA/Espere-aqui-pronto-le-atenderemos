using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoAnalisis.Logica
{
    public class Consultorios
    {
        public Consultorios(string nombre, bool activo)
        {
            Nombre = nombre;
            Activo = activo;
        }
        public string Nombre { 
            get; set; 
        }
        public List<Especialidades> Especialidades { 
            get; set; 
        } = new List<Especialidades>(); 
        
        public bool Activo { 
            get; set; 
        }
    }
}
