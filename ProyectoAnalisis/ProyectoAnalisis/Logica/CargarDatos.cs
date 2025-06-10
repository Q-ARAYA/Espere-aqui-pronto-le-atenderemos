using System;
using System.Collections.Generic;
using System.Xml;

namespace ProyectoAnalisis.Logica
{
    public class CargaDatos
    {
        // Especialidades
        public static List<Especialidades> CargarEspecialidadesDesdeXML(string rutaArchivo)
        {
            List<Especialidades> especialidades = new List<Especialidades>();

            XmlDocument doc = new XmlDocument();
            doc.Load(rutaArchivo);

            XmlNodeList nodos = doc.SelectNodes("//Especialidades/Especialidad");

            foreach (XmlNode nodo in nodos)
            {
                string nombre = nodo.Attributes["nombre"].Value;
                int duracion = int.Parse(nodo.Attributes["duracion"].Value);

                especialidades.Add(new Especialidades(nombre, duracion));
            }

            return especialidades;
        }

        // Pacientes
        public static List<Pacientes> CargarPacientesDesdeXML(string rutaArchivo, List<Especialidades> listaEspecialidades)
        {
            List<Pacientes> pacientes = new List<Pacientes>();

            XmlDocument doc = new XmlDocument();
            doc.Load(rutaArchivo);

            XmlNodeList nodosPacientes = doc.SelectNodes("//Pacientes/Paciente");

            foreach (XmlNode nodoPaciente in nodosPacientes)
            {
                int id = int.Parse(nodoPaciente.Attributes["id"].Value);
                string nombre = nodoPaciente.Attributes["nombre"].Value;

                List<Especialidades> especialidadesPaciente = new List<Especialidades>();

                XmlNodeList nodosEsp = nodoPaciente.SelectNodes("Especialidad");
                foreach (XmlNode nodoEsp in nodosEsp)
                {
                    string nombreEsp = nodoEsp.Attributes["nombre"].Value;

                    // Buscar la especialidad en lista cargada globalmente
                    Especialidades encontrada = listaEspecialidades.Find(e => e.Nombre == nombreEsp);
                    if (encontrada != null)
                    {
                        especialidadesPaciente.Add(encontrada);
                    }
                }

                pacientes.Add(new Pacientes(id, nombre, especialidadesPaciente));
            }

            return pacientes;
        }
    }
}
