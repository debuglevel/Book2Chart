using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Book2Chart.Parser
{
    /// <summary>
    /// Parses a Flat ODT file with specified styles and creates a book object
    /// </summary>
    public class FodtParser
    {
        /// <summary>
        /// parses a given Flat ODT file
        /// </summary>
        /// <param name="filename">the path to the FODT file</param>
        /// <returns>a book object containing the FODT's information</returns>
        public Book Parse(string filename)
        {
            XElement document = this.loadXML(filename);

            var paragraphs = this.getParagraphs(document);

            var book = new Book();

            Chapter lastChapter = new Chapter() { Title = "DUMMY" };
            Chapter currentChapter = new Chapter() { Title = "DUMMY" };
            foreach (var paragraph in paragraphs)
            {
                if (paragraph.StyleName.StartsWith("ZZ_20_Titel"))
                {
                    lastChapter = currentChapter;

                    currentChapter = new Chapter();
                    book.Chapters.Add(currentChapter);

                    currentChapter.PrecedingChapter = lastChapter;
                    lastChapter.SucceedingChapter = currentChapter;

                    currentChapter.Title = paragraph.Content;
                    currentChapter.RevisionStatus = this.getRevisionStatus(paragraph.StyleName);
                }
                else if (paragraph.StyleName == "ZZ_20_Einordnung_20_danach")
                {
                    currentChapter.SucceedingChapterReferences.Add(paragraph.Content);
                }
                else if (paragraph.StyleName == "ZZ_20_Einordnung_20_vorher")
                {
                    currentChapter.PrecedingChapterReferences.Add(paragraph.Content);
                }
                else if (paragraph.StyleName == "ZZ_20_Zusammenfassung")
                {
                    currentChapter.Summary.Add(paragraph.Content);
                }
                else if (paragraph.StyleName == "ZZ_20_Kommentar")
                {
                    currentChapter.Comment.Add(paragraph.Content);
                }
                else if (paragraph.StyleName == "ZZ_20_Inhalt")
                {
                    currentChapter.Text.Add(paragraph.Content);
                }
                else
                {
                    Trace.TraceWarning("unknown style name used: " + paragraph.StyleName);
                    paragraph.DebugInformation.Add(new KeyValuePair<DebugInformationType,object>(DebugInformationType.UnknownStyle, paragraph.StyleName));
                }
            }

            this.checkChaptersErrors(book.Chapters);

            return book;
        }

        private Chapter.RevisionStatuses getRevisionStatus(string stylename)
        {
            if (stylename == "ZZ_20_Titel_20_geprueft")
            {
                return Chapter.RevisionStatuses.Good;
            }
            else if (stylename == "ZZ_20_Titel_20_verbesserungsbeduerftig")
            {
                return Chapter.RevisionStatuses.Improvable;
            }
            else if (stylename == "ZZ_20_Titel_20_ungeprueft")
            {
                return Chapter.RevisionStatuses.Unreviewed;
            }

            return Chapter.RevisionStatuses.Unknown;
        }

        private XElement loadXML(string filename)
        {
            var sw = new StringWriter();
            XmlWriter writer = XmlWriter.Create(sw, new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Fragment });

            XElement doc = XElement.Load(filename);
            return doc;
        }

        private IEnumerable<Paragraph> getParagraphs(XElement doc)
        {
            XNamespace nsOffice = "urn:oasis:names:tc:opendocument:xmlns:office:1.0";
            XNamespace nsText = "urn:oasis:names:tc:opendocument:xmlns:text:1.0";

            var xmlParagraphs = doc.Descendants(nsOffice + "body").Descendants(nsOffice + "text").Descendants(nsText + "p");

            var paragraphs = from item in xmlParagraphs
                             select new Paragraph()
                             {
                                 Content = item.Value,
                                 StyleName = item.Attribute(nsText + "style-name").Value
                             };
            return paragraphs;
        }

        private void checkChaptersErrors(IEnumerable<Chapter> chapters)
        {
            foreach (var chapter in chapters)
            {
                this.checkChapterErrors(chapters, chapter);
            }
        }

        private void checkChapterErrors(IEnumerable<Chapter> chapters, Chapter chapter)
        {
            this.checkReferences(chapters, chapter);
            this.checkTitle(chapter);
            this.checkSummary(chapter);
        }

        private Boolean checkSummary(Chapter chapter)
        {
            var success = chapter.Summary.Any(p => String.IsNullOrWhiteSpace(p) == false);

            if (success == false)
            {
                Trace.TraceInformation("Chapter '"+chapter.Title+"' has no summary");
                chapter.DebugInformation.Add(new KeyValuePair<DebugInformationType,object>(DebugInformationType.EmptySummary, null));
            }

            return !success;
        }

        private Boolean checkTitle(Chapter chapter)
        {
            if (String.IsNullOrWhiteSpace(chapter.Title))
            {
                Trace.TraceInformation("Chapter between '" + chapter.PrecedingChapter.Title + "' and '" + chapter.SucceedingChapter.Title + "' has no name.");
                chapter.DebugInformation.Add(new KeyValuePair<DebugInformationType, object>(DebugInformationType.EmptyTitle, null));

                return false;
            }

            return true;
        }

        private Boolean checkReferences(IEnumerable<Chapter> chapters, Chapter chapter)
        {
            bool failed = false;

            foreach (var sibling in chapter.PrecedingChapterReferences.Concat(chapter.SucceedingChapterReferences))
            {
                bool exists = chapters.Any(x => x.Title == sibling);
                if (exists == false)
                {
                    failed = true;
                    Trace.TraceInformation("Chapter '" + sibling + "' by '" + chapter.Title + "' referenced but does not exist.");
                    chapter.DebugInformation.Add(new KeyValuePair<DebugInformationType, object>(DebugInformationType.MissingReference, sibling));
                }
            }

            return failed;
        }
    }
}
