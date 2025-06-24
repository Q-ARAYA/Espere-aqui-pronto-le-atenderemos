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

        // Esta funcion agrega un paciente a la cola
        // Metiendolo al final usando Enqueue
        public void AgregarPaciente(Pacientes paciente)
        {
            PacientesEnCola.Enqueue(paciente);
        }

        // Funcion para atender al primer paciente en la cola
        // Sacando el primero con Dequeue, si hay pacientes disponibles
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

        // Funcion que elimina un paciente especifico de la cola
        // Crea una nueva cola y copiando todos los que no coincidan con el que se quiere eliminar
        public void EliminarPaciente(Pacientes paciente)
        {
            var nuevaCola = new Queue<Pacientes>();

            while (PacientesEnCola.Count > 0)
            {
                var actual = PacientesEnCola.Dequeue();
                if (!(actual.Nombre == paciente.Nombre && actual.Especialidades == paciente.Especialidades))
                {
                    nuevaCola.Enqueue(actual);
                }
            }

            PacientesEnCola = nuevaCola;
        }
    }
}
