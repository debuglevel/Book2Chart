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

            var styles = this.getStyles(document);

            var paragraphs = this.getParagraphs(document);

            var book = new Book();

            Chapter lastChapter = new Chapter() { Title = "DUMMY" };
            Chapter currentChapter = new Chapter() { Title = "DUMMY" };
            foreach (var paragraph in paragraphs)
            {
                paragraph.Style = this.getStyle(styles, paragraph);
                var styleType = paragraph.Style.StyleType;

                if (styleType == StyleType.Title)
                {
                    lastChapter = currentChapter;

                    currentChapter = new Chapter();
                    book.Chapters.Add(currentChapter);

                    currentChapter.PrecedingChapter = lastChapter;
                    lastChapter.SucceedingChapter = currentChapter;

                    currentChapter.Title = paragraph.Content;
                    currentChapter.RevisionStatus = this.getRevisionStatus(paragraph.StyleName);
                }
                else if (styleType == StyleType.Successor)
                {
                    currentChapter.SucceedingChapterReferences.Add(paragraph.Content);
                }
                else if (styleType == StyleType.Precessor)
                {
                    currentChapter.PrecedingChapterReferences.Add(paragraph.Content);
                }
                else if (styleType == StyleType.Summary)
                {
                    currentChapter.Summary.Add(paragraph.Content);
                }
                else if (styleType == StyleType.Comment)
                {
                    currentChapter.Comment.Add(paragraph.Content);
                }
                else if (styleType == StyleType.Content)
                {
                    currentChapter.Text.Add(paragraph.Content);
                }
                else
                {

                }
            }

            this.checkChaptersErrors(book.Chapters);

            return book;
        }

        private Style getStyle(IEnumerable<Style> styles, Paragraph paragraph)
        {
            string styleName = paragraph.StyleName;

            var style = styles.FirstOrDefault(s => s.Name == styleName);
            if (style.IsBaseStyle)
            {
                return style;
            }
            else
            {
                return style.ParentStyle;
            }
        }

        private StyleType getStyleType(string styleName)
        {
            if (styleName.StartsWith("ZZTitel"))
            {
                return StyleType.Title;
            }
            else if (styleName == "ZZEinordnungDanach")
            {
                return StyleType.Successor;
            }
            else if (styleName == "ZZEinordnungVorher")
            {
                return StyleType.Precessor;
            }
            else if (styleName == "ZZZusammenfassung")
            {
                return StyleType.Summary;
            }
            else if (styleName == "ZZKommentar")
            {
                return StyleType.Comment;
            }
            else if (styleName == "ZZInhalt")
            {
                return StyleType.Content;
            }
            else
            {
                //Trace.TraceWarning("unknown style name used: " + paragraph.StyleName);
                //paragraph.DebugInformation.Add(new KeyValuePair<DebugInformationType, object>(DebugInformationType.UnknownStyle, paragraph.StyleName));

                return StyleType.Unkown;
            }
        }

        private IEnumerable<Style> getStyles(XElement document)
        {
            var styles = this.getAllStyles(document);
            this.assignAutomaticStyles(styles);
            this.assignBaseStyleTypes(styles);

            return styles;
        }

        private void assignBaseStyleTypes(IEnumerable<Style> styles)
        {
            foreach (var style in styles.Where(s=>s.IsBaseStyle))
            {
                style.StyleType = this.getStyleType(style.Name);
            }
        }

        private IEnumerable<Style> getAllStyles(XElement document)
        {
            XNamespace nsOffice = "urn:oasis:names:tc:opendocument:xmlns:office:1.0";
            XNamespace nsStyle = "urn:oasis:names:tc:opendocument:xmlns:style:1.0";

            var xmlDefinedStyles = document.Descendants(nsOffice + "styles").Descendants(nsStyle + "style");
            var definedStyles = from item in xmlDefinedStyles
                                select new Style
                                {
                                    Name = item.Attribute(nsStyle + "name").Value,
                                    IsBaseStyle = true
                                };

            var xmlAutomaticStyles = document.Descendants(nsOffice + "automatic-styles").Descendants(nsStyle + "style");
            var automaticStyles = from item in xmlAutomaticStyles
                         select new Style
                         {
                             Name = item.Attribute(nsStyle + "name").Value,
                             ParentStyleName = item.Attribute(nsStyle + "parent-style-name") != null ? item.Attribute(nsStyle + "parent-style-name").Value : null,
                             IsBaseStyle = false
                         };

            var styles = definedStyles.Concat(automaticStyles);

            //var styles = (from item in xmlStyles
            //              select new Style
            //              {
            //                  Name = item.Attribute(nsStyle + "name").Value,
            //                  ParentStyleName = item.Attribute(nsStyle + "parent-style-name") != null ? item.Attribute(nsStyle + "parent-style-name").Value : null
            //              }).Where(s => s.ParentStyleName != null);

            return styles.ToList();
        }

        private void assignAutomaticStyles(IEnumerable<Style> styles)
        {
            foreach (var style in styles)
            {
                var parentStyle = styles.FirstOrDefault(s => s.Name == style.ParentStyleName && s.IsBaseStyle);
                if (parentStyle != null)
                {
                    style.ParentStyle = parentStyle;
                }
                else
                {
                    Trace.TraceWarning("ParentStyle '"+style.ParentStyleName+"' used by '"+style.Name+"' not found (or is no base style).");
                }
            }
        }

        private RevisionStatus getRevisionStatus(string stylename)
        {
            if (stylename == "ZZTitelGeprueft")
            {
                return RevisionStatus.Good;
            }
            else if (stylename == "ZZTitelVerbesserungsbeduerftig")
            {
                return RevisionStatus.Improvable;
            }
            else if (stylename == "ZZTitelUngeprueft")
            {
                return RevisionStatus.Unreviewed;
            }
            else if (stylename == "ZZTitelMeilenstein")
            {
                return RevisionStatus.Milestone;
            }

            return RevisionStatus.Unknown;
        }

        private XElement loadXML(string filename)
        {
            XmlWriter xmlWriter = XmlWriter.Create(new StringWriter(), new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Fragment });

            var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            XElement document = XElement.Load(fileStream);
            return document;
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

            return paragraphs.ToList();
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
