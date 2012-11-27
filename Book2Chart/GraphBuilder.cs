using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graphviz4Net.Graphs;

namespace Book2Chart.Parser
{
    public class Arrow { }

    public class GraphBuilder
    {
        public Graph<Chapter> CreateGraph(IEnumerable<Chapter> chapters)
        {
            var graph = new Graph<Chapter>();

            foreach (var chapter in chapters)
            {
                graph.AddVertex(chapter);
            }

            foreach (var chapter in chapters)
            {
                foreach (var precedingChapterString in chapter.VorherRefs)
                {
                    Chapter preceedingChapter = this.findChapterByTitle(chapters, precedingChapterString);
                    if (preceedingChapter != null)
                    {
                        graph.AddEdge(new Edge<Chapter>(preceedingChapter, chapter, destinationArrow: new Test.Blubb.PrecedingArrow()));
                    }
                }

                foreach (var succeedingChapterString in chapter.NacherRefs)
                {
                    Chapter succeedingChapter = this.findChapterByTitle(chapters, succeedingChapterString);
                    if (succeedingChapter != null)
                    {
                        graph.AddEdge(new Edge<Chapter>(chapter, succeedingChapter, destinationArrow: new Test.Blubb.SucceedingArrow()));
                    }
                }
            }

            return graph;
        }

        private Chapter findChapterByTitle(IEnumerable<Chapter> chapters, String chapterTitle)
        {
            return chapters.FirstOrDefault(c => c.Title == chapterTitle);
        }
    }
}
