using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoAnalisis.Logica
{
    public class Especialidades
    {
        public Especialidades(string nombre, int duracion)
        {
            Nombre = nombre;
            Duracion = duracion;
        }

        public string Nombre { get; set; }
        public int Duracion { get; set; }

    }
}
