// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
{
    [Serializable]
    public sealed class ItalicText : ParagraphInlineComponent
    {
        public ItalicText(IXmlElement element) : base(element)
        {
        }

        public override string ToXml()
        {
            return "<i>" + contents.ToXml() + "</i>";
        }

        public override string ToHtml()
        {
            return "<i>" + contents.ToHtml() + "</i>";
        }

        public override string ToRtf()
        {
            return @"\i " + contents.ToRtf() + @"\i0 ";
        }

        public override string ToRtf(RtfDocument document)
        {
            return ToRtf();
        }
    }
}