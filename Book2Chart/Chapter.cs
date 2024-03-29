﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book2Chart.Parser
{
    public class Chapter
    {
        public string Title { get; set; }
        public List<string> Text { get; set; }
        public List<string> Comment { get; set; }
        public List<string> Summary { get; set; }
        public List<string> PrecedingChapterReferences { get; private set; }
        public List<string> SucceedingChapterReferences { get; private set; }
        public Chapter PrecedingChapter { get; set; }
        public Chapter SucceedingChapter { get; set; }
        public List<KeyValuePair<DebugInformationType, object>> DebugInformation { get; private set; }
        public RevisionStatus RevisionStatus { get; set; }

        public String TextAsString { get { return String.Join(Environment.NewLine, this.Text); } }
        public String CommentAsString { get { return String.Join(Environment.NewLine, this.Comment); } }
        public String SummaryAsString { get { return String.Join(Environment.NewLine, this.Summary); } }

        public Chapter()
        {
            this.Comment = new List<string>();
            this.Summary = new List<string>();
            this.Text = new List<string>();
            this.PrecedingChapterReferences = new List<string>();
            this.SucceedingChapterReferences = new List<string>();
            this.DebugInformation = new List<KeyValuePair<DebugInformationType, object>>();
        }
    }
}
