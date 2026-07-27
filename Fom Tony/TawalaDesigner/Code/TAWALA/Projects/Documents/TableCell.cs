// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Projects.Expressions;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
{
    [Serializable]
    public sealed class TableCell : IParagraphComponent
    {
        private const string htmlCellEndTag = "</div></td>";
        private const string htmlCellStartTag = "<td><div style=\"width:{0}pt;\">";
        private const string rtfCellEnd = @"\cell ";
        private const string xmlCellEndTag = "</cell>";
        private const string xmlCellStartTag = "<cell width=\"{0}\">";

        public TableCell(IXmlElement element)
        {
            Divisions = new Collection<Division>();
            Width = Convert.ToInt32(element.GetAttribute("width"));

            var divisionElements = element.GetChildren();

            foreach (IXmlElement divisionElement in divisionElements)
            {
                Divisions.Add(new Division(divisionElement));
            }
        }

        public TableCell(IParagraphComponent component, int width)
        {
            Divisions = new Collection<Division>();
            Width = width;
            Divisions.Add(new Division(component));
        }

        public int Width { get; private set; }

        public Collection<Division> Divisions { get; private set; }

        #region IParagraphComponent Members

        public string Text
        {
            get { return String.Empty; }
        }

        public string ToXml()
        {
            var xmlString = new StringBuilder();

            xmlString.AppendFormat(xmlCellStartTag, Width);

            foreach (var division in Divisions)
            {
                xmlString.Append(division.ToXml());
            }

            xmlString.Append(xmlCellEndTag);

            return xmlString.ToString();
        }

        public string ToHtml()
        {
            var xmlString = new StringBuilder();

            xmlString.AppendFormat(htmlCellStartTag, Width/20);

            foreach (var division in Divisions)
            {
                xmlString.Append(division.ToHtml());
            }

            xmlString.Append(htmlCellEndTag);

            return xmlString.ToString();
        }

        public string ToRtf()
        {
            var rtfString = new StringBuilder();

            int divisionIndex = 0;

            foreach (var division in Divisions)
            {
                rtfString.Append(division.ToRtf());

                if (!isFinalDivision(divisionIndex))
                {
                    rtfString.Append(@"\par ");
                }

                divisionIndex++;
            }

            rtfString.Append(rtfCellEnd);

            return rtfString.ToString();
        }

        #endregion

        private bool isFinalDivision(int divisionIndex)
        {
            return (divisionIndex == Divisions.Count - 1);
        }

        public string ToRtf(RtfDocument document)
        {
            var rtfString = new StringBuilder();

            int divisionIndex = 0;

            foreach (var division in Divisions)
            {
                rtfString.Append(division.ToRtf(document));

                if (!isFinalDivision(divisionIndex))
                {
                    rtfString.Append(@"\par ");
                }

                divisionIndex++;
            }

            rtfString.Append(rtfCellEnd);

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