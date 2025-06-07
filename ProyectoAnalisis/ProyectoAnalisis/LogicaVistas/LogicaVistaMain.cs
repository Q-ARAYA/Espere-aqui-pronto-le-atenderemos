// Archivo: LogicaVistas/LogicaVistaMain.cs
using ProyectoAnalisis.Logica;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ProyectoAnalisis.LogicaVistas
{
    public static class LogicaVistaMain
    {
        private static List<Consultorios> listaDeConsultorios = new List<Consultorios>();
        private static List<Pacientes> listaDePacientes = new List<Pacientes>(); // NUEVO
        private static List<PacientesEnEspera> listaDePacientesEnEspera = new List<PacientesEnEspera>();

        /// <summary>
        /// Valida los datos y crea una nueva especialidad en la capa de l�gica.
        /// </summary>
        /// <param name="nombre">Nombre de la especialidad.</param>
        /// <param name="duracion">Duraci�n en minutos.</param>
        /// <returns>Un mensaje de error si falla, o null si tiene �xito.</returns>
        public static string CrearEspecialidad(string nombre, int duracion)
        {
            // La validaci�n ahora est� en el controlador, m�s cerca de la l�gica de negocio.
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return "El nombre no puede estar vac�o.";
            }
            if (duracion <= 0)
            {
                return "La duraci�n debe ser un n�mero mayor a 0.";
            }

            // Creamos el objeto de la capa L�gica
            var nuevaEspecialidad = new Especialidades(nombre, duracion);

            // Intentamos agregarlo a trav�s de nuestro gestor de datos
            bool exito = GestorDatos.AgregarEspecialidad(nuevaEspecialidad);

            if (!exito)
            {
                return $"La especialidad '{nombre}' ya existe.";
            }

            return null; // Todo sali� bien
        }

        /// <summary>
        /// Obtiene la lista de especialidades desde la capa de l�gica.
        /// </summary>
        public static List<Especialidades> CargarEspecialidades()
        {
            return GestorDatos.ObtenerEspecialidades();
        }

        /// <summary>
        /// Crea y registra un consultorio si el n�mero no existe.
        /// </summary>
        /// <param name="numeroConsultorio">N�mero identificador del consultorio.</param>
        /// <param name="activo">Estado del consultorio.</param>
        /// <param name="especialidades">Lista de especialidades asignadas.</param>
        /// <returns>Mensaje de error si falla, o null si tiene �xito.</returns>
        public static string CrearConsultorio(int numeroConsultorio, bool activo, List<Especialidades> especialidades)
        {
            var consultorio = listaDeConsultorios.FirstOrDefault(c => c.NumeroConsultorio == numeroConsultorio);

            if (consultorio != null)
            {
                // Si ya existe, lo reactivamos y actualizamos especialidades
                consultorio.Activo = true;
                consultorio.Especialidades = especialidades;
                return null; // No es error, se reactiv�
            }

            // Si no existe, lo creamos normalmente
            consultorio = new Consultorios(numeroConsultorio, $"Consultorio {numeroConsultorio}", activo)
            {
                Especialidades = especialidades
            };

            listaDeConsultorios.Add(consultorio);
            return null;
        }

        public static void DesactivarConsultorio(int numeroConsultorio)
        {
            var consultorio = listaDeConsultorios.FirstOrDefault(c => c.NumeroConsultorio == numeroConsultorio);
            if (consultorio != null)
            {
                consultorio.Activo = false;
            }
        }

        /// <summary>
        /// Obtiene la lista de consultorios registrados.
        /// </summary>
        /// <returns>Lista de consultorios.</returns>
        public static List<Consultorios> ObtenerConsultorios()
        {
            return new List<Consultorios>(listaDeConsultorios);
        }

        public static void MostrarConsultorios(List<Consultorios> consultorios, WrapPanel panel)
        {
            panel.Children.Clear();
            foreach (var consultorio in consultorios)
            {
                Border border = new Border
                {
                    Width = 120,
                    Height = 60,
                    Margin = new System.Windows.Thickness(10),
                    Background = consultorio.Activo ? Brushes.LightGreen : Brushes.LightGray,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new System.Windows.Thickness(1),
                    Child = new TextBlock
                    {
                        Text = consultorio.Nombre,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        TextAlignment = System.Windows.TextAlignment.Center
                    }
                };
                panel.Children.Add(border);
            }
        }


        /// <summary>
        /// Crea y registra un paciente si el nombre no est� vac�o y la especialidad es v�lida.
        /// </summary>
        /// <param name="nombre">Nombre del paciente.</param>
        /// <param name="especialidad">Especialidad asignada.</param>
        /// <param name="prioridad">Prioridad del paciente.</param>
        /// <returns>Mensaje de error si falla, o null si tiene �xito.</returns>
        public static string CrearPaciente(int id, string nombre, List<Especialidades> especialidades)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return "El nombre del paciente no puede estar vac�o.";
            if (especialidades == null)
                return "Debe seleccionar una especialidad.";

            var paciente = new Pacientes(id, nombre, especialidades);
            listaDePacientes.Add(paciente);
            return null;
        }

        /// <summary>
        /// Obtiene la lista de pacientes registrados.
        /// </summary>
        /// <returns>Lista de pacientes.</returns>
        public static List<Pacientes> ObtenerPacientes()
        {
            return new List<Pacientes>(listaDePacientes);
        }

        public static string CrearPacienteEnEspera(Pacientes paciente, string imagen)
        {
            if (paciente == null)
                return "El paciente no puede ser nulo.";
            var pacienteEnEspera = new PacientesEnEspera(paciente, imagen);
            listaDePacientesEnEspera.Add(pacienteEnEspera);
            return null; // Todo sali� bien
        }

        public static List<PacientesEnEspera> ObtenerPacientesEnEspera()
        {
            return new List<PacientesEnEspera>(listaDePacientesEnEspera);
        }
    }
}
