using ProyectoAnalisis.Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoAnalisis.LogicaSistema
{
    public class SistemaConsultorios
    {
        public class GestorConsultorios
        {
            public List<Consultorios> Consultorios { get; set; } = new List<Consultorios>();

            public void AgregarConsultorio(string nombre)
            {
                if (Consultorios.Count < 15)
                    Consultorios.Add(new Consultorios(nombre, true));
            }

            public void CerrarConsultorio(string nombre)
            {
                var consultorio = Consultorios.Find(c => c.Nombre == nombre);
                if (consultorio != null)
                    consultorio.Activo = false;
            }

            public void AgregarEspecialidad(string nombreConsultorio, Especialidades nuevaEspecialidad)
            {
                var consultorio = Consultorios.Find(c => c.Nombre == nombreConsultorio);
                if (consultorio != null)
                    consultorio.Especialidades.Add(nuevaEspecialidad);
            }

            public void QuitarEspecialidad(string nombreConsultorio, string nombreEspecialidad)
            {
                var consultorio = Consultorios.Find(c => c.Nombre == nombreConsultorio);
                if (consultorio != null)
                    consultorio.Especialidades.RemoveAll(e => e.Nombre == nombreEspecialidad);
            }
        }



    }
}
