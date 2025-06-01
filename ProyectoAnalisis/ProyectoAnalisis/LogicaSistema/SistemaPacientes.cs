using ProyectoAnalisis.Logica;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoAnalisis.LogicaSistema
{
    public class SistemaPacientes
    {
        public Queue<Pacientes> PacientesEnCola { get; set; } = new Queue<Pacientes>();

        public void AgregarPaciente(Pacientes paciente)
        {
            PacientesEnCola.Enqueue(paciente);
        }
        public Pacientes AtenderPaciente()
        {
            if (PacientesEnCola.Count > 0)
            {
                return PacientesEnCola.Dequeue();
            }
            else
            {
                throw new InvalidOperationException("No hay pacientes en la cola para atender.");
            }
        }
            
        public void EliminarPaciente(Pacientes paciente)
        {
            var nuevaCola = new Queue<Pacientes>();

            while (PacientesEnCola.Count > 0)
            {
                var actual = PacientesEnCola.Dequeue();
                if (!(actual.Nombre == paciente.Nombre && actual.Especialidad == paciente.Especialidad && actual.Prioridad == paciente.Prioridad))
                {
                    nuevaCola.Enqueue(actual);
                }
            }

            PacientesEnCola = nuevaCola;
        }
    }
}
