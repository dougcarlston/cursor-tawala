// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
{
    [Serializable]
    public sealed class DocumentTab : ParagraphComponent
    {
        public DocumentTab(IXmlElement element)
        {
        }

        public override string Text { get { return "\t"; } }

        public override string ToXml()
        {
            return "<tab/>";
        }

        public override string ToRtf(RtfDocument document)
        {
            return ToRtf();
        }

        public override string ToRtf()
        {
            return @"\tab ";
        }
    }
}