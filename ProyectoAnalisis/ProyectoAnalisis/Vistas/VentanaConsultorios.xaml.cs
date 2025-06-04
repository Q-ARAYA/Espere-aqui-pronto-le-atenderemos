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
        }

        private void btnEspecialidades_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new VentanaEspecialidades();
            ventana.ShowDialog();
        }
    }
}
