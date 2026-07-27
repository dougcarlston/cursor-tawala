// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
{
    [Serializable]
    public class TabPositions : Collection<TabStop>, IDocumentConversions
    {
        public TabPositions()
        {
        }

        public TabPositions(IXmlElement element)
        {
            Collection<XmlElement> childElements = element.GetChildren("tabStop");

            foreach (XmlElement childElement in childElements)
            {
                Add(new TabStop(childElement));
            }
        }

        #region IDocumentConversions Members

        public virtual string ToXml()
        {
            var xmlString = new StringBuilder();

            if (Count > 0)
            {
                xmlString.Append("<tabPositions>");

                for (int i = 0; i < Count; i++)
                {
                    xmlString.Append(this[i].ToXml());
                }

                xmlString.Append("</tabPositions>");
            }

            return xmlString.ToString();
        }

        public virtual string ToHtml()
        {
            return "";
        }

        public virtual string ToRtf()
        {
            var rtfString = new StringBuilder();

            for (int i = 0; i < Count; i++)
            {
                rtfString.Append(this[i].ToRtf());
            }

            return rtfString.ToString();
        }

        public virtual string ToRtf(RtfDocument document)
        {
            var rtfString = new StringBuilder();

            for (int i = 0; i < Count; i++)
            {
                rtfString.Append(this[i].ToRtf());
            }

            return rtfString.ToString();
        }

        #endregion

        /// <summary>
        /// Returns the first tabstop position that is greater than the specified position.
        /// </summary>
        public int PositionGreaterThan(int specifiedPosition)
        {
            foreach (TabStop tabStop in this)
            {
                if (tabStop.Position > specifiedPosition)
                {
                    return tabStop.Position;
                }
            }

            return 10800;
        }
    }

    [Serializable]
    public class TabStop
    {
        /// <summary>
        /// tab stop position in TWIPS
        /// </summary>
        private readonly int position;

        public TabStop(IXmlElement element)
        {
            position = Convert.ToInt32(element.GetAttribute("position"));
        }

        public int Position { get { return position; } }

        public string ToXml()
        {
            return String.Format("<tabStop position=\"{0}\"/>", position);
        }

        public string ToRtf()
        {
            return String.Format(@"\tx{0}", position);
        }
    }
}