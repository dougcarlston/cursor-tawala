// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Documents;
using Tawala.RtfSupport;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
    [Serializable]
    public class FormItemText : ParagraphComponent
    {
        protected string text = "";

        public FormItemText(string text)
        {
            this.text = text;
        }

        public FormItemText(IXmlElement element)
        {
            text = element.OuterXml;
        }

        public override string Text { get { return XmlUtility.ConvertEntitiesToText(text); } }

        public override string ToXml()
        {
            return text;
        }

        public override string ToHtml()
        {
            return text;
        }

        public override string ToRtf()
        {
            return fixUpEscapeCharacters(text).Replace("\n", @"\par ");
        }

        public override string ToRtf(RtfDocument document)
        {
            return ToRtf();
        }

        private string fixUpEscapeCharacters(string rawTextString)
        {
            string fixUpString = RtfUtility.EscapeSpecialCharacters(rawTextString);

            return XmlUtility.ConvertEntitiesToText(fixUpString);
        }
    }
}