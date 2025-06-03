using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ProyectoAnalisis.Logica;
using ProyectoAnalisis.LogicaVistas;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ProyectoAnalisis.Vistas
{
    public partial class VentanaConsultorios : Window
    {
        public VentanaConsultorios()
        {
            InitializeComponent();
            CargarConsultorios(); // Aquí llamas la lógica para crear los cuadros
            CargarPacientes();
        }

        private void CargarConsultorios()
        {
            // Puedes reemplazar esto con tu lógica real (por ejemplo, leer desde base de datos o archivo)
            var consultorios = new List<Consultorios>
            {
                new Consultorios("Consultorio A", true),
                new Consultorios("Consultorio B", false),
                new Consultorios("Consultorio C", true),
            };

            // Llamada a la lógica de la vista que llena el panel
            LogicaVistaMain.MostrarConsultorios(consultorios, PanelConsultorios);
        }

        private void CargarPacientes()
        {
            var pacientes = new List<Pacientes>
            {
                new Pacientes("Carlos", new Especialidades("Cardiología", 30), 1),
                new Pacientes("Ana", new Especialidades("Dermatología", 20), 2),
                new Pacientes("Luis", new Especialidades("Neurología", 40), 3),
                new Pacientes("María", new Especialidades("Pediatría", 25), 1)
            };

            LogicaVistaMain.MostrarPacientes(pacientes, PanelPacientes);
        }
    }
}
