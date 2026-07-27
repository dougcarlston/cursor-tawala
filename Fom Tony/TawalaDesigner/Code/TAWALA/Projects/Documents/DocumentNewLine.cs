// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
{
    [Serializable]
    public sealed class DocumentNewLine : ParagraphComponent
    {
        public DocumentNewLine(IXmlElement element)
        {
        }

        public override string ToXml()
        {
            return "<newline/>";
        }

        public override string ToRtf(RtfDocument document)
        {
            return ToRtf();
        }

        public override string ToRtf()
        {
            return @"\par ";
        }
    }
}