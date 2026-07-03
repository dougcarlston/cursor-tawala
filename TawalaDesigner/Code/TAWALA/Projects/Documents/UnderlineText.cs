// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
{
    [Serializable]
    public class UnderlineText : ParagraphInlineComponent
    {
        public UnderlineText(IXmlElement element) : base(element)
        {
        }

        public override string ToXml()
        {
            return "<u>" + contents.ToXml() + "</u>";
        }

        public override string ToHtml()
        {
            return "<u>" + contents.ToHtml() + "</u>";
        }

        public override string ToRtf()
        {
            return @"\ul " + contents.ToRtf() + @"\ul0 ";
        }

        public override string ToRtf(RtfDocument document)
        {
            return ToRtf();
        }
    }
}