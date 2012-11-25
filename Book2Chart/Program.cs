using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Book2Chart
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new StringWriter();
            XmlWriter writer = XmlWriter.Create(sw, new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Fragment });

            XElement doc = XElement.Load("Book.fodt");

            XNamespace nsOffice = "urn:oasis:names:tc:opendocument:xmlns:office:1.0";
            XNamespace nsText = "urn:oasis:names:tc:opendocument:xmlns:text:1.0";

            var xmlParagraphs = doc.Descendants(nsOffice + "body").Descendants(nsOffice + "text").Descendants(nsText + "p");

            var paragraphs = from item in xmlParagraphs
                       select new Paragraph()
                           {
                               Content = item.Value,
                               StyleName = item.Attribute(nsText + "style-name").Value
                           };

            var cleanedParagrphs = paragraphs;//.Where(x => x.StyleName != "Inhalt");

            var chapters = new List<Chapter>();

            Chapter lastChapter = new Chapter() { Title = "DUMMY" };
            Chapter currentChapter = new Chapter() { Title = "DUMMY" };
            foreach (var paragraph in cleanedParagrphs)
            {
                if (paragraph.StyleName == "P1" || paragraph.StyleName.StartsWith("Titel"))
                {
                    lastChapter = currentChapter;

                    currentChapter = new Chapter();
                    chapters.Add(currentChapter);

                    currentChapter.Vorher = lastChapter;
                    lastChapter.Nacher = currentChapter;

                    currentChapter.Title = paragraph.Content;
                }

                if (paragraph.StyleName.EndsWith("danach"))
                {
                    currentChapter.NacherRefs.Add(paragraph.Content);
                }

                if (paragraph.StyleName.EndsWith("vorher"))
                {
                    currentChapter.VorherRefs.Add(paragraph.Content);
                }

                if (paragraph.StyleName == "Zusammenfassung")
                {
                    currentChapter.Summary.Add(paragraph.Content);
                }

                if (paragraph.StyleName == "Kommentar")
                {
                    currentChapter.Comment.Add(paragraph.Content);
                }

                if (paragraph.StyleName == "Inhalt")
                {
                    currentChapter.Text.Add(paragraph.Content);
                }

            }

            checkChaptersErrors(chapters);

            Console.ReadLine();

        }

        private static void checkChaptersErrors(IEnumerable<Chapter> chapters)
        {
            foreach (var chapter in chapters)
            {
                checkChapterErrors(chapters, chapter);
            }
        }

        private static void checkChapterErrors(IEnumerable<Chapter> chapters, Chapter chapter)
        {
            checkSiblings(chapters, chapter);
            checkTitle(chapter);
            checkSummary(chapter);
        }

        private static Boolean checkSummary(Chapter chapter)
        {
            var success = chapter.Summary.Any(x=>x.Trim().Length > 0);

            if (success == false)
            {
                Console.WriteLine("Chapter '"+chapter.Title+"' has no summary");
            }

            return !success;
        }

        private static Boolean checkTitle(Chapter chapter)
        {
            bool failed = false;

            if (chapter.Title.Any() == false)
            {
                failed = true;
                Console.WriteLine("Chapter between '" + chapter.Vorher.Title + "' and '" + chapter.Nacher.Title + "' has no name.");
            }

            return failed;
        }

        private static Boolean checkSiblings(IEnumerable<Chapter> chapters, Chapter chapter)
        {
            bool failed = false;

            foreach (var sibling in chapter.VorherRefs.Concat(chapter.NacherRefs))
            {
                bool exists = chapters.Any(x => x.Title == sibling);
                if (exists == false)
                {
                    failed = true;
                    Console.WriteLine("Chapter '" + sibling + "' referenced but does not exist.");
                }
            }

            return failed;
        }
    }
}
