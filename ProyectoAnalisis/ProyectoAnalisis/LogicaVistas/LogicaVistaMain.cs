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
        /// Valida los datos y crea una nueva especialidad en la capa de lógica.
        /// </summary>
        /// <param name="nombre">Nombre de la especialidad.</param>
        /// <param name="duracion">Duración en minutos.</param>
        /// <returns>Un mensaje de error si falla, o null si tiene éxito.</returns>
        public static string CrearEspecialidad(string nombre, int duracion)
        {
            // La validación ahora está en el controlador, más cerca de la lógica de negocio.
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return "El nombre no puede estar vacío.";
            }
            if (duracion <= 0)
            {
                return "La duración debe ser un número mayor a 0.";
            }

            // Creamos el objeto de la capa Lógica
            var nuevaEspecialidad = new Especialidades(nombre, duracion);

            // Intentamos agregarlo a través de nuestro gestor de datos
            bool exito = GestorDatos.AgregarEspecialidad(nuevaEspecialidad);

            if (!exito)
            {
                return $"La especialidad '{nombre}' ya existe.";
            }

            return null; // Todo salió bien
        }

        /// <summary>
        /// Obtiene la lista de especialidades desde la capa de lógica.
        /// </summary>
        public static List<Especialidades> CargarEspecialidades()
        {
            return GestorDatos.ObtenerEspecialidades();
        }

        /// <summary>
        /// Crea y registra un consultorio si el número no existe.
        /// </summary>
        /// <param name="numeroConsultorio">Número identificador del consultorio.</param>
        /// <param name="activo">Estado del consultorio.</param>
        /// <param name="especialidades">Lista de especialidades asignadas.</param>
        /// <returns>Mensaje de error si falla, o null si tiene éxito.</returns>
        public static string CrearConsultorio(int numeroConsultorio, bool activo, List<Especialidades> especialidades)
        {
            var consultorio = listaDeConsultorios.FirstOrDefault(c => c.NumeroConsultorio == numeroConsultorio);

            if (consultorio != null)
            {
                // Si ya existe, lo reactivamos y actualizamos especialidades
                consultorio.Activo = true;
                consultorio.Especialidades = especialidades;
                return null; // No es error, se reactivó
            }

            // Si no existe, lo creamos normalmente
            consultorio = new Consultorios(numeroConsultorio, $"Consultorio {numeroConsultorio}", activo)
            {
                Especialidades = especialidades
            };

            listaDeConsultorios.Add(consultorio);
            return null;
        }


        // Funcion que desactiva a un consultorio usando su numero
        // Buscandolo en la lista y cambiando su estado activo a false
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

        // Esta funcion muestra todos los consultorios en el panel visual
        // Lo hace limpiando el panel y agregando bloques visuales con color segun si estan activos
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
        /// Crea y registra un paciente si el nombre no está vacío y la especialidad es válida.
        /// </summary>
        /// <param name="nombre">Nombre del paciente.</param>
        /// <param name="especialidad">Especialidad asignada.</param>
        /// <param name="prioridad">Prioridad del paciente.</param>
        /// <returns>Mensaje de error si falla, o null si tiene éxito.</returns>
        public static string CrearPaciente(int id, string nombre, List<Especialidades> especialidades)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return "El nombre del paciente no puede estar vacío.";
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

        // Esta funcion agrega un paciente a la lista de espera si no esta duplicado
        // Compara el ID y las especialidades para asegurarse de que no se repita antes de agregarlo
        public static string CrearPacienteEnEspera(Pacientes paciente, string imagen)
        {
            if (paciente == null)
                return "El paciente no puede ser nulo.";

            // Evitar duplicados: mismo paciente y mismas especialidades
            if (listaDePacientesEnEspera.Any(p =>
                p.Paciente.pacienteID == paciente.pacienteID &&
                p.Paciente.Especialidades.Select(e => e.Nombre).OrderBy(x => x)
                    .SequenceEqual(paciente.Especialidades.Select(e => e.Nombre).OrderBy(x => x))))
            {
                return null;
            }

            var pacienteEnEspera = new PacientesEnEspera(paciente, imagen);
            listaDePacientesEnEspera.Add(pacienteEnEspera);
            return null; // Todo salió bien
        }

        // Esta funcion devuelve la lista de pacientes que estan en espera
        // Lo hace devolviendo una nueva lista con los mismos elementos para evitar cambios directos
        public static List<PacientesEnEspera> ObtenerPacientesEnEspera()
        {
            return new List<PacientesEnEspera>(listaDePacientesEnEspera);
        }

        // Esta funcion elimina a un paciente especifico de la lista de espera
        // Lo hace usando el metodo Remove sobre la lista
        public static void EliminarPacienteEnEspera(PacientesEnEspera paciente)
        {
            listaDePacientesEnEspera.Remove(paciente);
        }

    }
}
