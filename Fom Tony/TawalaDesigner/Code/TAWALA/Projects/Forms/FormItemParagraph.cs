// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Projects.Documents;
using Tawala.Projects.Function;
using Tawala.Projects.Links;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms
{
    [Serializable]
    public class FormItemParagraph : Paragraph
    {
        private const string htmlParagraphEndTag = "</p>";
        private const string htmlParagraphStartTag = "<p style=\"margin-left:{0}pt\" align=\"{1}\">";
        private const string xmlParagraphEndTag = "</paragraph>";
        private const string xmlParagraphStartTag = "<paragraph indent=\"{0}\" align=\"{1}\">";
        protected static FibItemOwnedFactory<IParagraphComponent> componentFactory = new FibItemOwnedFactory<IParagraphComponent>();

        static FormItemParagraph()
        {
            componentFactory.Register("#text", typeof(FormItemText));
            componentFactory.Register("#whitespace", typeof(FormItemText));
            componentFactory.Register("sp", typeof(FormItemSpace));
            componentFactory.Register("field", "name", typeof(FormItemNamedField));
            componentFactory.Register("font", typeof(FormItemFontAttributes));
            componentFactory.Register("tab", typeof(DocumentTab));
            componentFactory.Register("newline", typeof(DocumentNewLine));
            componentFactory.Register("image", "id", typeof(GraphicImageReference));
            componentFactory.Register("image", typeof(GraphicImage));
            componentFactory.Register("blank", typeof(Blank));
            componentFactory.Register("functionField", typeof(DocumentIdedFunctionField), "instanceId");
            componentFactory.Register("hyperlink", typeof(IdedHyperlinkField), "id");
            componentFactory.Register("link", typeof(NamedHyperlinkField));
            componentFactory.Register("b", typeof(BoldText));
            componentFactory.Register("i", typeof(ItalicText));
            componentFactory.Register("u", typeof(UnderlineText));
        }

        public FormItemParagraph()
        {
        }

        /// <summary>
        /// Constructs a FormItemParagraph object from an XML &lt;paragraph&gt; element.
        /// </summary>
        public FormItemParagraph(IXmlElement element)
        {
            indent = element.GetAttribute("indent", defaultIndent);
            align = element.GetAttribute("align", defaultAlign);
            tabPositions = new TabPositions(element.GetChild("tabPositions"));

            Collection<XmlElement> childElements = element.GetChildren();

            foreach (XmlElement childElement in childElements)
            {
                if (childElement.Name != "tabPositions")
                {
                    contents.Add(componentFactory.MakeObject(childElement));
                }
            }
        }

        /// <summary>
        /// Constructs a FormItemParagraph object from an XML &lt;paragraph&gt; element.
        /// </summary>
        public FormItemParagraph(IXmlElement element, FibItem owner)
        {
            indent = element.GetAttribute("indent", defaultIndent);
            align = element.GetAttribute("align", defaultAlign);
            tabPositions = new TabPositions(element.GetChild("tabPositions"));

            Collection<XmlElement> childElements = element.GetChildren();

            foreach (XmlElement childElement in childElements)
            {
                if (childElement.Name != "tabPositions")
                {
                    if (childElement.Name == "blank")
                    {
                        contents.Add(owner.BlankList.MakeBlank(childElement, owner));
                        owner.BlankList.BlankIndex++;
                    }
                    else
                    {
                        if (childElement.Name == "font")
                        {
                            contents.Add(componentFactory.MakeObject(childElement, owner));
                        }
                        else
                        {
                            if (childElement.Name != "#whitespace")
                            {
                                contents.Add(componentFactory.MakeObject(childElement));
                            }
                        }
                    }
                }
            }
        }

        public override string Text
        {
            get
            {
                var textString = new StringBuilder();

                foreach (IParagraphComponent component in contents)
                {
                    if (component != null)
                    {
                        textString.Append(component.Text);
                    }
                }

                return textString.ToString();
            }
        }

        public override string ToXml()
        {
            var xmlString = new StringBuilder();

            xmlString.AppendFormat(xmlParagraphStartTag, indent, align);

            xmlString.AppendFormat(tabPositions.ToXml());

            foreach (IParagraphComponent component in contents)
            {
                if (component != null)
                {
                    xmlString.Append(component.ToXml());
                }
            }

            xmlString.Append(xmlParagraphEndTag);

            return xmlString.ToString();
        }

        private string toParagraphHtml()
        {
            var htmlString = new StringBuilder();

            htmlString.AppendFormat(htmlParagraphStartTag, indent/20, align);

            if (contents.Count > 0)
            {
                foreach (IParagraphComponent component in contents)
                {
                    htmlString.Append(component.ToHtml());
                }
            }
            else
            {
                htmlString.Append("&nbsp;");
            }

            htmlString.Append(htmlParagraphEndTag);

            return Regex.Replace(htmlString.ToString(), "  ", " &nbsp;"); // to display as multiple spaces
        }

        private string toTableHtml()
        {
            var table = new Table(this);

            return table.ToHtml();
        }

        public override string ToHtml()
        {
            return (containsTabs() ? toTableHtml() : toParagraphHtml());
        }

        private bool containsTabs()
        {
            foreach (ParagraphComponent component in contents)
            {
                if (component is DocumentTab)
                {
                    return true;
                }
            }

            return false;
        }

        public override string ToRtf()
        {
            var rtfString = new StringBuilder(rtfParagraphReset);

            if (align != "left")
            {
                rtfString.AppendFormat(rtfParagraphAlign, align.Substring(0, 1));
            }

            if (indent > 0)
            {
                rtfString.AppendFormat(rtfParagraphIndent, indent);
            }

            foreach (IParagraphComponent component in contents)
            {
                rtfString.Append(component.ToRtf());
            }

            rtfString.Append(rtfParagraphEnd);

            return rtfString.ToString();
        }

        public override string ToRtf(RtfDocument document)
        {
            var rtfString = new StringBuilder(rtfParagraphReset);

            if (align != "left")
            {
                rtfString.AppendFormat(rtfParagraphAlign, align.Substring(0, 1));
            }

            if (indent > 0)
            {
                rtfString.AppendFormat(rtfParagraphIndent, indent);
            }

            rtfString.Append(tabPositions.ToRtf());

            foreach (IParagraphComponent component in contents)
            {
                if (component != null)
                {
                    if (component is FormItemFontAttributes)
                    {
                        rtfString.Append(((FormItemFontAttributes)component).ToRtf(document));
                    }
                    else
                    {
                        rtfString.Append(component.ToRtf());
                    }
                }
            }

            rtfString.Append(rtfParagraphEnd);

            return rtfString.ToString();
        }
    }

    [Serializable]
    public class FormItemSpace : ParagraphComponent
    {
        public FormItemSpace(IXmlElement element)
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