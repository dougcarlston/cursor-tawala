// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text;
using Tawala.FontSupport;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
{
    [Serializable]
    public class FontAttributes : ParagraphInlineComponent
    {
        public FontAttributes()
        {
        }

        public FontAttributes(IXmlElement element) : base(element)
        {
            Face = (element.HasAttribute("face") ? element.GetAttribute("face") : Fonts.DefaultFontName);

            Size = (element.HasAttribute("size") ? Convert.ToInt32(element.GetAttribute("size")) : 210);

            Color = (element.HasAttribute("color") ? Convert.ToInt32(element.GetAttribute("color"), 16) : 1);
        }

        public string Face { get; protected set; }

        public int Size { get; protected set; }

        public int Color { get; protected set; }

        public override string ToXml()
        {
            var fontStartTag = new StringBuilder("<font");

            if (Face != null && Face != Fonts.DefaultFontName)
            {
                fontStartTag.AppendFormat(" face=\"{0}\"", Face);
            }

            if (Size != 0 && Size != 210)
            {
                fontStartTag.AppendFormat(" size=\"{0}\"", Size);
            }

            if (Color != 1)
            {
                fontStartTag.AppendFormat(" color=\"{0:X6}\"", Color);
            }

            fontStartTag.Append(">");

            return fontStartTag + contents.ToXml() + "</font>";
        }

        public override string ToHtml()
        {
            const string spanStartTag = "<span style=\"font-family:'{0}';font-size:{1}pt;color:#{2:X6}\">";
            return String.Format(spanStartTag, Face, twipsToPoints(Size), Color) + contents.ToHtml() + "</span>";
        }

        private int twipsToPoints(int sizeinTwips)
        {
            return sizeinTwips/20;
        }

        protected int TwipsToHalfPoints(int sizeinTwips)
        {
            return sizeinTwips/10;
        }

        protected int ZeroBasedToOneBased(int zeroBasedIndex)
        {
            return zeroBasedIndex + 1;
        }

        public override string ToRtf(RtfDocument document)
        {
            const string rtfString = @"{{\f{0}\fs{1}\cf{2} {3}}}";

            int fontIndex = document.FontTable.IndexMatching(Face);
            int colorIndex = document.ColorTable.IndexMatching(Color);

            return string.Format(rtfString, fontIndex, TwipsToHalfPoints(Size), ZeroBasedToOneBased(colorIndex), contents.ToRtf());
        }
    }
}