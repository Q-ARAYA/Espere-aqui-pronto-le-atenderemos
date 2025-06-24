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

            // Metodo que agrega un nuevo consultorio al sistema, si no se ha llegado al limite de 15
            // Le pone un numero automatico y lo deja activo por defecto
            public void AgregarConsultorio(string nombre)
            {
                if (Consultorios.Count < 15)
                {
                    int numeroConsultorio = Consultorios.Count + 1;
                    Consultorios.Add(new Consultorios(numeroConsultorio, nombre, true));
                }
            }

            // Metodo que cierra un consultorio cambiando su estado a inactivo
            // Lo busca por el nombre
            public void CerrarConsultorio(string nombre)
            {
                var consultorio = Consultorios.Find(c => c.Nombre == nombre);
                if (consultorio != null)
                    consultorio.Activo = false;
            }

            // Este metodo agrega una especialidad nueva al consultorio indicado
            public void AgregarEspecialidad(string nombreConsultorio, Especialidades nuevaEspecialidad)
            {
                var consultorio = Consultorios.Find(c => c.Nombre == nombreConsultorio);
                if (consultorio != null)
                    consultorio.Especialidades.Add(nuevaEspecialidad);
            }

            // Metodo que elimina una especialidad del consultorio, buscandola por su nombre
            public void QuitarEspecialidad(string nombreConsultorio, string nombreEspecialidad)
            {
                var consultorio = Consultorios.Find(c => c.Nombre == nombreConsultorio);
                if (consultorio != null)
                    consultorio.Especialidades.RemoveAll(e => e.Nombre == nombreEspecialidad);
            }
        }



    }
}
