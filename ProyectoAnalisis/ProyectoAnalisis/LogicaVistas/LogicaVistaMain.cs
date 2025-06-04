// Archivo: LogicaVistas/LogicaVistaMain.cs
using ProyectoAnalisis.Logica;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ProyectoAnalisis.LogicaVistas
{
    public static class LogicaVistaMain
    {
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

        public static void MostrarPacientes(List<Pacientes> pacientes, WrapPanel panel)
        {
            panel.Children.Clear();
            foreach (var paciente in pacientes)
            {
                Ellipse circulo = new Ellipse
                {
                    Width = 40,
                    Height = 40,
                    Fill = Brushes.LightBlue,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Margin = new System.Windows.Thickness(5),
                    ToolTip = $"{paciente.Nombre} - Revisión de: {paciente.Especialidad.Nombre}"
                };
                panel.Children.Add(circulo);
            }
        }
    }
}