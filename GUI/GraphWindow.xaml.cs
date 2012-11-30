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
using Graphviz4Net.Graphs;

namespace Book2Chart.GUI
{
    /// <summary>
    /// Interaktionslogik für GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Window
    {
        public Graph<Chapter> Graph { get; private set; }

        public GraphWindow()
        {
            InitializeComponent();
        }

        public GraphWindow(Book book) : this()
        {
            var graphBuilder = new GraphBuilder();
            this.Graph = graphBuilder.CreateGraph(book.Chapters);
            this.DataContext = this;
        }

        private void zoomcontrol_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (this.zoomcontrol.Zoom >= this.zoomcontrol.MaxZoom)
                {
                    return;
                }

                this.zoomcontrol.Zoom += 0.1;
            }
            else if (e.Delta < 0)
            {
                if (this.zoomcontrol.Zoom <= this.zoomcontrol.MinZoom)
                {
                    return;
                }

                this.zoomcontrol.Zoom -= 0.1;
            }
        }
    }
}
