using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book2Chart.Parser
{
    public class Paragraph
    {
        public string Content { get; set; }
        public string StyleName { get; set; }
        public List<KeyValuePair<string, object>> DebugInformation { get; private set; }

        public Paragraph()
        {
            this.DebugInformation = new List<KeyValuePair<string, object>>();
        }
    }
}
