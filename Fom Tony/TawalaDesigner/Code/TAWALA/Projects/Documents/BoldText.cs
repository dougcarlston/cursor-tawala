// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
{
    [Serializable]
    public sealed class BoldText : ParagraphInlineComponent
    {
        public BoldText(IXmlElement element) : base(element)
        {
        }

        public override string ToXml()
        {
            return "<b>" + contents.ToXml() + "</b>";
        }

        public override string ToHtml()
        {
            return "<b>" + contents.ToHtml() + "</b>";
        }

        public override string ToRtf()
        {
            return @"\b " + contents.ToRtf() + @"\b0 ";
        }

        public override string ToRtf(RtfDocument document)
        {
            return ToRtf();
        }
    }
}