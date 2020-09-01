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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace StructureOnWPF
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class StructureUserControl : UserControl
    {
        public IEnumerable<string> StructuresIds { get; set; }

        public string DisplayingId { get; set; }
        public MainViewModel ViewModel { get; set; }
        public StructureUserControl(MainViewModel mvm)
        {
            ViewModel = mvm;
            StructuresIds = ViewModel.Sts.Select(e => e.Id);
            InitializeComponent();
            Display.ItemsSource = StructuresIds;
            Display.SelectedItem = ViewModel.Displaying.Id;
            MyModel.Content = (ViewModel.Model3D);
            UpdateCamPerspective();

        }

        private void Display_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MyModel.Children.Clear();
            MyModel.Content = ViewModel.UpdateDisplaying(Display.SelectedItem.ToString());

        }
        private void AddToModel(GeometryModel3D geoModel)
        {
            var nb = new ModelVisual3D();
            nb.Content = geoModel;
            MyModel.Children.Add(nb);
        }
        private void UpdateCamPerspective()
        {
            var bounds = ViewModel.Displaying.Mesh.Bounds;
            var pt = new Point3D((bounds.X + bounds.SizeX) / 20, (bounds.Y + bounds.SizeY) / 20, (bounds.Z + bounds.SizeZ) / 20);
            var negpt = new Vector3D(-(bounds.X + bounds.SizeX) / 20, -(bounds.Y + bounds.SizeY) / 20, -(bounds.Z + bounds.SizeZ) / 20);

            camMain.Position = pt;
            camMain.LookDirection = negpt;
        }

        private void CopyStructure_Click(object sender, RoutedEventArgs e)
        {
            var dis = ViewModel.CopyDisplaying();
            StructuresIds =  ViewModel.Sts.Select(s=>s.Id);
            Display.ItemsSource = StructuresIds;
            Display.SelectedItem = dis;
            Logger.Text += "\n" + ViewModel.Logger;

        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9]+\.?[0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void MoveStructure_Click(object sender, RoutedEventArgs e)
        {
            var moved = ViewModel.MoveDisplaying(double.Parse(xDisplace.Text), double.Parse(yDisplace.Text), double.Parse(zDisplace.Text));
            StructuresIds = ViewModel.Sts.Select(s => s.Id);
            Display.ItemsSource = StructuresIds;
            //Display.SelectedItem = moved;
            Logger.Text += "\n" + ViewModel.Logger;
            AddToModel(ViewModel.UpdateDisplaying(moved));
        }
    }
}
