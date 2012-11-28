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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Book2Chart.Parser;

namespace Book2Chart.GUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class ExplorerWindow : Window
    {
        public ExplorerWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Book"; // Default file name
            dlg.DefaultExt = ".fodt"; // Default file extension
            dlg.Filter = "Flat ODT (.fodt)|*.fodt"; // Filter files by extension 

            // Show open file dialog box
            bool? result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;

                var parser = new Book2Chart.Parser.FodtParser();
                var book = parser.Parse(filename);
                this.DataContext = book;
            }


        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            new GraphWindow(DataContext as Book).Show();
        }
    }
}
