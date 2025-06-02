using ProyectoAnalisis.Logica;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ProyectoAnalisis.LogicaVistas
{
    public static class LogicaVistaMain
    {
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
