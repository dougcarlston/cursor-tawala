// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
{
    [Serializable]
    public sealed class DocumentSpace : ParagraphComponent
    {
        public DocumentSpace(IXmlElement element)
        {
        }

        public override string ToXml()
        {
            return "<sp/>";
        }

        public override string ToHtml()
        {
            return " ";
        }

        public override string ToRtf()
        {
            return " ";
        }

        public override string ToRtf(RtfDocument document)
        {
            return ToRtf();
        }
    }
}