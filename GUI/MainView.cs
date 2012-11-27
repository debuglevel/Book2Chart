using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Graphviz4Net.WPF.Example
{
    using Graphs;

    public class Person
    {
    }

    public class MainWindowViewModel 
    {
        public MainWindowViewModel()
        {
            var graph = new Graph<Person>();
            var a = new Person();
            var b = new Person();
            var c = new Person();

            graph.AddVertex(a);
            graph.AddVertex(b);
            graph.AddVertex(c);

            graph.AddEdge(new Edge<Person>(c, a));

            this.Graph = graph;
        }

        public Graph<Person> Graph { get; private set; }
    }
}
