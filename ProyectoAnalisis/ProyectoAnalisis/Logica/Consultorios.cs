using System;
using System.Collections.Generic;

namespace ProyectoAnalisis.Logica
{
    public class Consultorios
    {
        public Consultorios(int numeroConsultorio, string nombre, bool activo)
        {
            NumeroConsultorio = numeroConsultorio;
            Nombre = $"Consultorio {numeroConsultorio}";
            Activo = activo;
            ColaPacientes = new List<PacientesEnEspera>();
        }

        public int NumeroConsultorio { get; set; }
        public string Nombre { get; set; }
        public List<Especialidades> Especialidades { get; set; } = new List<Especialidades>();
        public bool Activo { get; set; }

        public List<PacientesEnEspera> ColaPacientes { get; set; }
    }
}