using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book2Chart.Parser
{
    public class Book
    {
        public List<Chapter> Chapters { get; private set; }

        public Book()
        {
            this.Chapters = new List<Chapter>();
        }
    }
}
