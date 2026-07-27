// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Projects.Factories;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms.FormItemContents
{
    [Serializable]
    public abstract class NewFont : FormItemContents
    {
        public const int DefaultFontSize = 0;
        protected string fontColor;

        protected string fontFace;
        protected int fontSizeInPoints;

        protected NewFont(IXmlElement element)
        {
            Contents = FormItemContentsFactory.MakeChildren(element);
        }

        public string FontFace
        {
            get { return fontFace; }
            set
            {
                fontFace = value;

                Contents.ApplyFontStyle(new FontStyle(this));
            }
        }

        public int FontSizeInPoints
        {
            get { return fontSizeInPoints; }

            set
            {
                fontSizeInPoints = value;

                Contents.ApplyFontStyle(new FontStyle(this));
            }
        }

        public string FontColor
        {
            get { return fontColor; }

            set
            {
                fontColor = value;

                Contents.ApplyFontStyle(new FontStyle(this));
            }
        }

        public override string ToXml()
        {
            removeRedundantFontStyles();

            var xmlString = new StringBuilder();

            if (AnyFontAttributeSpecified())
            {
                xmlString.Append("<font");
            }

            if (fontFaceSpecified())
            {
                xmlString.AppendFormat(" face=\"{0}\"", fontFace);
            }

            if (fontSizeSpecified())
            {
                xmlString.AppendFormat(" size=\"{0}\"", fontSizeInPoints*20);
            }

            if (fontColorSpecified())
            {
                xmlString.AppendFormat(" color=\"{0}\"", fontColor);
            }

            if (AnyFontAttributeSpecified())
            {
                xmlString.Append(">");
            }

            xmlString.Append(Contents.ToXml());

            if (AnyFontAttributeSpecified())
            {
                xmlString.Append("</font>");
            }

            return xmlString.ToString();
        }

        private void removeRedundantFontStyles()
        {
            FontStyle innermostStyle = Contents.GetInnermostFontStyle();
            ApplyFontStyle(innermostStyle);
            setFontStyle(innermostStyle);
        }

        private void setFontStyle(FontStyle style)
        {
            if (style != null)
            {
                if (style.HasFace)
                {
                    fontFace = style.Face;
                }

                if (style.HasSize)
                {
                    fontSizeInPoints = style.Size;
                }

                if (style.HasColor)
                {
                    fontColor = style.Color;
                }
            }
        }

        public override string ToXhtml(IFormItem formItem)
        {
            var xmlString = new StringBuilder();

            if (AnyFontAttributeSpecified())
            {
                xmlString.Append("<span style=\"");

                if (fontFaceSpecified())
                {
                    xmlString.AppendFormat("font-family: {0};", fontFace);
                }

                if (fontSizeSpecified())
                {
                    xmlString.AppendFormat("font-size: {0}pt;", fontSizeInPoints);
                }

                if (fontColorSpecified())
                {
                    xmlString.AppendFormat("color: #{0};", fontColor);
                }

                xmlString.Append("\">");
                xmlString.Append(Contents.ToXhtml(formItem));
                xmlString.Append("</span>");
            }
            else
            {
                xmlString.Append(Contents.ToXhtml(formItem));
            }

            return xmlString.ToString();
        }

        public override void ApplyFontStyle(FontStyle style)
        {
            if (style != null)
            {
                if (style.HasFace)
                {
                    fontFace = string.Empty;
                }

                if (style.HasSize)
                {
                    fontSizeInPoints = DefaultFontSize;
                }

                if (style.HasColor)
                {
                    fontColor = string.Empty;
                }

                Contents.ApplyFontStyle(style);
            }
        }

        public override FontStyle GetInnermostFontStyle()
        {
            FontStyle style = null;

            if (Contents != null)
            {
                style = Contents.GetInnermostFontStyle();
            }

            if (style == null)
            {
                style = new FontStyle(this);
            }

            return style;
        }

        public bool AnyFontAttributeSpecified()
        {
            return fontFaceSpecified() || fontSizeSpecified() || fontColorSpecified();
        }

        private bool fontSizeSpecified()
        {
            return fontSizeInPoints != DefaultFontSize;
        }

        private bool fontFaceSpecified()
        {
            return !String.IsNullOrEmpty(fontFace) && !defaultFontSpecified();
        }

        private bool defaultFontSpecified()
        {
            return fontFace == "Default Font";
        }

        private bool fontColorSpecified()
        {
            return !String.IsNullOrEmpty(fontColor);
        }
    }

    [Serializable]
    public class NewXmlFont : NewFont
    {
        public NewXmlFont(IXmlElement element)
            : base(element)
        {
            fontFace = element.GetAttribute("face");
            fontSizeInPoints = Convert.ToInt32(element.GetAttribute("size"))/20;
            fontColor = element.GetAttribute("color");
        }
    }

    [Serializable]
    public class NewXhtmlFont : NewFont
    {
        public NewXhtmlFont(IXmlElement element)
            : base(element)
        {
            string style = element.GetAttribute("style");

            if (style != null)
            {
                Match sizeMatch = Regex.Match(style, @"FONT-SIZE: (\d+)pt", RegexOptions.IgnoreCase);

                if (sizeMatch.Success)
                {
                    fontSizeInPoints = Convert.ToInt32(sizeMatch.Groups[1].Value);
                }

                Match faceMatch = Regex.Match(style, @"FONT-FAMILY: ([^;]+);?", RegexOptions.IgnoreCase);

                if (faceMatch.Success)
                {
                    fontFace = faceMatch.Groups[1].Value;
                }

                Match colorMatch = Regex.Match(style, @"COLOR: #([0-9A-Fa-f]+)", RegexOptions.IgnoreCase);

                if (colorMatch.Success)
                {
                    fontColor = colorMatch.Groups[1].Value;
                }
            }
        }
    }
}