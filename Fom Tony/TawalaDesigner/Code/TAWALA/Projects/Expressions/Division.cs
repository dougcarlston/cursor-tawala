// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text;
using Tawala.Projects.Documents;
using Tawala.XmlSupport;

namespace Tawala.Projects.Expressions
{
    [Serializable]
    public class Division : Paragraph
    {
        private const string htmlDivEndTag = "</div>";
        private const string htmlDivStartTag = "<div style=\"margin-left:{0}pt;text-align:{1}\">";
        private const string xmlDivisionEndTag = "</division>";
        private const string xmlDivisionStartTag = "<division indent=\"{0}\" align=\"{1}\">";

        public Division(IXmlElement element) : base(element)
        {
        }

        public Division(IParagraphComponent component)
        {
            contents.Add(component);
        }

        public override string ToXml()
        {
            var xmlString = new StringBuilder();

            xmlString.AppendFormat(xmlDivisionStartTag, indent, align);

            foreach (var component in contents)
            {
                xmlString.Append(component.ToXml());
            }

            xmlString.Append(xmlDivisionEndTag);

            return xmlString.ToString();
        }

        public override string ToHtml()
        {
            var xmlString = new StringBuilder();

            xmlString.AppendFormat(htmlDivStartTag, indent/20, align);

            foreach (var component in contents)
            {
                xmlString.Append(component.ToHtml());
            }

            xmlString.Append(htmlDivEndTag);

            return xmlString.ToString();
        }

        public override string ToRtf()
        {
            var rtfString = new StringBuilder();

//			if (align != "left")
            {
                rtfString.AppendFormat(rtfParagraphAlign, align.Substring(0, 1));
            }

            if (indent > 0)
            {
                rtfString.AppendFormat(rtfParagraphIndent, indent);
            }

            foreach (var component in contents)
            {
                rtfString.Append(component.ToRtf());
            }

            return rtfString.ToString();
        }

        public override string ToRtf(RtfDocument document)
        {
            var rtfString = new StringBuilder();

//			if (align != "left")
            {
                rtfString.AppendFormat(rtfParagraphAlign, align.Substring(0, 1));
            }

            if (indent > 0)
            {
                rtfString.AppendFormat(rtfParagraphIndent, indent);
            }

            foreach (var component in contents)
            {
                if (component is FontAttributes)
                {
                    rtfString.Append(((FontAttributes)component).ToRtf(document));
                }
                else
                {
                    rtfString.Append(component.ToRtf());
                }
            }

            return rtfString.ToString();
        }
    }
}