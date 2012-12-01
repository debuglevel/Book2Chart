using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book2Chart.Parser
{
    public class Style
    {
        public string Name { get; set; }
        public string ParentStyleName { get; set; }
        public Style ParentStyle { get; set; }
        public Boolean IsBaseStyle { get; set; }
        public StyleType StyleType { get; set; }
    }
}
