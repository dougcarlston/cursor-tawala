// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
{
    [Serializable]
    public class TableRow : IParagraphComponent
    {
        private const string htmlRowEndTag = "</tr>";
        private const string htmlRowStartTag = "<tr>";
        private const string xmlRowEndTag = "</row>";
        private const string xmlRowStartTag = "<row>";
        private readonly Collection<TableCell> cells = new Collection<TableCell>();
        private int indent;

        public TableRow(IXmlElement element)
        {
            var cellElements = element.GetChildren();

            foreach (IXmlElement cellElement in cellElements)
            {
                cells.Add(new TableCell(cellElement));
            }
        }

        public TableRow(Paragraph paragraph)
        {
            int usefulComponentCount = paragraph.Contents.Count;

            while (usefulComponentCount > 0 && paragraph.Contents[usefulComponentCount - 1] is DocumentTab)
            {
                // disregard trailing tabs
                usefulComponentCount--;
            }

            int leftCellEdge = paragraph.Indent;
            int rightCellEdge;
            int cellWidth;
            var currentComponentList = new ParagraphComponentList();

            for (int i = 0; i < usefulComponentCount; i++)
            {
                if (paragraph.Contents[i] is DocumentTab)
                {
                    rightCellEdge = paragraph.TabPositions.PositionGreaterThan(leftCellEdge);
                    cellWidth = rightCellEdge - leftCellEdge;

                    cells.Add(new TableCell(currentComponentList, cellWidth));

                    leftCellEdge = rightCellEdge;
                    currentComponentList = new ParagraphComponentList();
                }
                else
                {
                    currentComponentList.Add(paragraph.Contents[i]);
                }
            }

            rightCellEdge = 10800;
            cellWidth = rightCellEdge - leftCellEdge;

            cells.Add(new TableCell(currentComponentList, cellWidth));
        }

        public Collection<TableCell> Cells
        {
            get { return cells; }
        }

        public int Indent
        {
            get { return indent; }
            set { indent = value; }
        }

        #region IParagraphComponent Members

        public string Text
        {
            get { return String.Empty; }
        }

        public string ToXml()
        {
            var xmlString = new StringBuilder();

            xmlString.AppendFormat(xmlRowStartTag);

            foreach (var cell in cells)
            {
                xmlString.Append(cell.ToXml());
            }

            xmlString.Append(xmlRowEndTag);

            return xmlString.ToString();
        }

        public virtual string ToHtml()
        {
            var xmlString = new StringBuilder();

            xmlString.AppendFormat(htmlRowStartTag);

            foreach (var cell in cells)
            {
                xmlString.Append(cell.ToHtml());
            }

            xmlString.Append(htmlRowEndTag);

            return xmlString.ToString();
        }

        public string ToRtf()
        {
            var rtfString = new StringBuilder();

            int accumulatedWidth = indent;

            foreach (var cell in cells)
            {
                rtfString.AppendFormat(@"\clftsWidth3\clwWidth{0}\cellx{1}", cell.Width, cell.Width + accumulatedWidth);
                accumulatedWidth += cell.Width;
            }

            rtfString.Append(@"\pard\intbl ");

            foreach (var cell in cells)
            {
                rtfString.Append(cell.ToRtf());
            }

            rtfString.Append(@"\row");

            return rtfString.ToString();
        }

        #endregion

        public string ToRtf(RtfDocument document)
        {
            var rtfString = new StringBuilder();

            int accumulatedWidth = indent;

            foreach (var cell in cells)
            {
                rtfString.AppendFormat(@"\clftsWidth3\clwWidth{0}\cellx{1}", cell.Width, cell.Width + accumulatedWidth);
                accumulatedWidth += cell.Width;
            }

            rtfString.Append(@"\pard\intbl ");

            foreach (var cell in cells)
            {
                rtfString.Append(cell.ToRtf(document));
            }

            rtfString.Append(@"\row");

            return rtfString.ToString();
        }

        #region IEnumerable Interface

        public IEnumerator GetEnumerator()
        {
            yield return this;
        }

        #endregion

        #region IRecursiveEnumerable Interface

        public IEnumerable RecursiveEnumerator
        {
            get { yield return this; }
        }

        #endregion
    }
}