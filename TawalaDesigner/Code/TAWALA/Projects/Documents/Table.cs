// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
{
    [Serializable]
    public sealed class Table : IDocumentBlock
    {
        private const string htmlTableEndTag = "</table>";
        private const string htmlTableStartTag = "<table style=\"margin-left:{0}pt;\">";
        private const string xmlTableEndTag = "</table>";
        private const string xmlTableStartTag = "<table indent=\"{0}\">";
        private static readonly int defaultIndent;
        private readonly Collection<TableRow> rows = new Collection<TableRow>();

        /// <summary>
        /// Indentation in twips
        /// </summary>
        protected int indent = defaultIndent;

        public Table(IXmlElement element)
        {
            indent = Convert.ToInt32(element.GetAttribute("indent"));

            Collection<XmlElement> rowElements = element.GetChildren();

            foreach (IXmlElement rowElement in rowElements)
            {
                rows.Add(new TableRow(rowElement));
            }
        }

        public Table(Paragraph paragraph)
        {
            indent = paragraph.Indent;
            rows.Add(new TableRow(paragraph));
        }

        public Collection<TableRow> Rows { get { return rows; } }

        public int Indent { get { return indent; } set { indent = value; } }

        #region IDocumentBlock Members

        public string ToXml()
        {
            var xmlString = new StringBuilder();

            xmlString.AppendFormat(xmlTableStartTag, indent);

            foreach (TableRow row in rows)
            {
                xmlString.Append(row.ToXml());
            }

            xmlString.Append(xmlTableEndTag);

            return xmlString.ToString();
        }

        public string ToHtml()
        {
            var xmlString = new StringBuilder();

            xmlString.AppendFormat(htmlTableStartTag, indent/20);

            foreach (TableRow row in rows)
            {
                xmlString.Append(row.ToHtml());
            }

            xmlString.Append(htmlTableEndTag);

            return xmlString.ToString();
        }

        public string ToRtf()
        {
            var rtfString = new StringBuilder();

            foreach (TableRow row in rows)
            {
                rtfString.AppendFormat(@"\trowd\trleft{0}", indent);
                row.Indent = indent;
                rtfString.Append(row.ToRtf());
            }

            return rtfString.ToString();
        }

        public string ToRtf(RtfDocument document)
        {
            var rtfString = new StringBuilder();

            foreach (TableRow row in rows)
            {
                rtfString.AppendFormat(@"\trowd\trleft{0}", indent);
                row.Indent = indent;
                rtfString.Append(row.ToRtf(document));
            }

            return rtfString.ToString();
        }

        public string Text { get { return ""; } }

        #endregion
    }
}