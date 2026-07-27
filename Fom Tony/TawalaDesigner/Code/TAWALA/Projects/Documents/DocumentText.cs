// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.RtfSupport;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
{
    [Serializable]
    public class DocumentText : ParagraphComponent, IDocumentConversions
    {
        protected string text = "";

        public DocumentText(string text)
        {
            this.text = text;
        }

        public DocumentText(IXmlElement element)
        {
            text = element.OuterXml;
        }

        public override string Text
        {
            get { return text; }
        }

        #region IDocumentConversions Members

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
            return fixUpEscapeCharacters(text);
        }

        public override string ToRtf(RtfDocument document)
        {
            return ToRtf();
        }

        #endregion

        private string fixUpEscapeCharacters(string rawTextString)
        {
            string fixUpString = RtfUtility.EscapeSpecialCharacters(rawTextString);

            return XmlUtility.ConvertEntitiesToText(fixUpString);
        }
    }
}