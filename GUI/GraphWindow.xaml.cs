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
using Book2Chart.Parser;

namespace Book2Chart.GUI
{
    /// <summary>
    /// Interaktionslogik für GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Window
    {
        public Book Book { get; private set; }

        public GraphWindow()
        {
            InitializeComponent();
        }

        public GraphWindow(Book book) : this()
        {
            this.Book = book;
            this.DataContext = book;
        }
    }
}
