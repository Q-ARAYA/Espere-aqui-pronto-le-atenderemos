using ProyectoAnalisis.LogicaVistas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

                string error = LogicaVistaMain.CrearEspecialidad(nombre, duracion);
                if (error != null)
                {
                    throw new Exception($"Error al cargar especialidad: {error}");
                }

                var ultimo = LogicaVistaMain.CargarEspecialidades().ElementAtOrDefault(LogicaVistaMain.CargarEspecialidades().Count - 1);
                if (ultimo != null)
                {
                    especialidades.Add(ultimo);
                }
                else
                {
                    throw new Exception("No se pudo cargar la especialidad desde la lógica.");
                }
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

            // Obtener la ruta de imágenes para selección aleatoria
            string rutaBase = AppDomain.CurrentDomain.BaseDirectory;
            string rutaImagenes = System.IO.Path.GetFullPath(System.IO.Path.Combine(rutaBase, @"..\..\Imagenes"));
            var imagenes = Directory.GetFiles(rutaImagenes, "*.png").ToList();
            Random random = new Random();

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

                var ultimoPacienteID = LogicaVistaMain.ObtenerPacientes().Count;
                var resultado = LogicaVistaMain.CrearPaciente(ultimoPacienteID, nombre, especialidadesPaciente);

                if (resultado == null) // Si se creó correctamente
                {
                    // Obtener el paciente recién creado
                    var pacienteCreado = LogicaVistaMain.ObtenerPacientes().LastOrDefault();
                    if (pacienteCreado != null)
                    {
                        pacientes.Add(pacienteCreado);

                        // Seleccionar imagen aleatoria
                        string imagenSeleccionada = imagenes.Count > 0
                            ? imagenes[random.Next(imagenes.Count)]
                            : null;

                        // Agregar paciente a la lista de espera
                        LogicaVistaMain.CrearPacienteEnEspera(pacienteCreado, imagenSeleccionada);
                    }
                }
            }

            return pacientes;
        }
    }
}


