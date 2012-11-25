using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book2Chart.Parser
{
    class Chapter
    {
        public string Title { get; set; }
        public List<string> Comment { get; set; }
        public List<string> Summary { get; set; }
        public List<string> Text { get; set; }
        public List<string> VorherRefs { get; set; }
        public List<string> NacherRefs { get; set; }
        public Chapter Vorher { get; set; }
        public Chapter Nacher { get; set; }

        public Chapter()
        {
            this.Comment = new List<string>();
            this.Summary = new List<string>();
            this.Text = new List<string>();
            this.VorherRefs = new List<string>();
            this.NacherRefs = new List<string>();
        }
    }
}
