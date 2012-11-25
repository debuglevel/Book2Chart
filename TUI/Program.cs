using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book2Chart.TUI
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new Book2Chart.Parser.Parser();
            parser.Parse("Book.fodt");

            Console.ReadLine();
        }
    }
}
