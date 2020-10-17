using Microsoft.Win32;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.Models;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void BtnCoord_Click(object sender, RoutedEventArgs e)
        {
            var ctfac = new CoordinateTransformationFactory();
            var toCS = GeographicCoordinateSystem.WGS84;
            var fromCS = ProjectedCoordinateSystem.WGS84_UTM(32, true);
            var trans = ctfac.CreateFromCoordinateSystems(fromCS, toCS);

            double[] fromPoint = new double[] { 344354, 5676502 };  
            double[] toPoint = trans.MathTransform.Transform(fromPoint);

            var utmX = Convert.ToInt32(fromPoint[0]);
            var utmY = Convert.ToInt32(fromPoint[1]);

            var latDecimal = toPoint[1].ToString(new CultureInfo("en-US"));
            var lonDecimal = toPoint[0].ToString(new CultureInfo("en-US"));
            var url1 = $@"https://www.openstreetmap.de/karte.html?lat={latDecimal}&lon={lonDecimal}&zoom=17&layers=B000";
            var url2 = $@"https://maps.duesseldorf.de/stk/index.html?Zoom=6&UTM32={utmX},{utmY}";

            Log($"Coords: {toPoint[0]}, {toPoint[1]}");
            Log($"Url: {url2}");

            //webBrowser.Navigate(url2);

            Process.Start(url2);


        }
        private void BtnOne_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var list = ReadCSV(openFileDialog.FileName).Where(lsa => lsa != null);
                
                Log($"Added {list.Count()} units.");
                unitDataGrid.ItemsSource = list;
            }
        }
        private void DataGrid_DblClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            // iteratively traverse the visual tree
            while ((dep != null) && !(dep is DataGridCell))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }
            if (dep == null) return;

            if (dep is DataGridCell)
            {
                while ((dep != null) && !(dep is DataGridRow))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }
                DataGridRow row = dep as DataGridRow;
                var lsa = row.Item as LichtSignalAnlage;

                var utmX = Convert.ToInt32(lsa.UtmEast);
                var utmY = Convert.ToInt32(lsa.UtmNorth);

                var url2 = $@"https://maps.duesseldorf.de/stk/index.html?Zoom=6&UTM32={utmX},{utmY}";
                Process.Start(url2);
                Log($"{lsa.NodeName}");
            }
        }
        public IEnumerable<LichtSignalAnlage> ReadCSV(string fileName)
        {
            var errorCount = 0;
            var linesCount = 0;
            string[] lines = MyReadAllLines(System.IO.Path.ChangeExtension(fileName, ".csv"));
            var result = lines.Select(line =>
            {
                linesCount++;
                string[] data = line.Split(';');
                try
                {
                    var lsa = new LichtSignalAnlage(data[0], data[4], data[8], Convert.ToDouble(data[12]), Convert.ToDouble(data[13]));
                    return lsa;
                }
                catch (Exception)
                {
                    errorCount++;
                    return null;
                }
            }).ToList();

            Log($"Read {fileName}, {linesCount} lines read, {errorCount} errors.");
            return result;
        }
        private string[] MyReadAllLines(String path)
        {
            using (var csv = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(csv, Encoding.Default))
            {
                List<string> file = new List<string>();
                while (!sr.EndOfStream)
                {
                    file.Add(sr.ReadLine());
                }
                return file.ToArray();
            }
        }
        private void Log(string msg)
        {
            logTextBox.Text += msg + Environment.NewLine;
            logTextBox.ScrollToEnd();
        }

    }
}
