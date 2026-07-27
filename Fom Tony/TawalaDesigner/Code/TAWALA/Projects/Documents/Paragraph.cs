// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Functions.Runtime;
using Tawala.Projects.Factories;
using Tawala.Projects.Function;
using Tawala.Projects.Links;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
{
    [Serializable]
    public class Paragraph : IDocumentBlock
    {
        public const string AlignCenter = "center";
        public const string AlignJustify = "justify";
        public const string AlignLeft = "left";
        public const string AlignRight = "right";

        protected const string defaultAlign = AlignLeft;
        protected const int defaultIndent = 0;
        private const string htmlParagraphEndTag = "</p>";
        private const string htmlParagraphStartTag = "<p style=\"margin-left:{0}pt\" align=\"{1}\">";
        protected const string rtfParagraphAlign = @"\q{0} ";
        protected const string rtfParagraphEnd = @"\par ";
        protected const string rtfParagraphIndent = @"\li{0} ";
        protected const string rtfParagraphReset = @"\pard ";
        private const string xmlParagraphEndTag = "</paragraph>";
        private const string xmlParagraphStartTag = "<paragraph indent=\"{0}\" align=\"{1}\">";

        public static Factory<IParagraphComponent> ComponentFactory = new Factory<IParagraphComponent>();

        /// <summary>
        /// Alignment ('left', 'right', 'center', justify')
        /// </summary>
        protected string align = defaultAlign;

        protected ParagraphComponentList contents = new ParagraphComponentList();

        /// <summary>
        /// Indentation in twips
        /// </summary>
        protected int indent = defaultIndent;

        protected TabPositions tabPositions = new TabPositions();

        static Paragraph()
        {
            ComponentFactory.Register("#text", typeof(DocumentText));
            ComponentFactory.Register("b", typeof(BoldText));
            ComponentFactory.Register("i", typeof(ItalicText));
            ComponentFactory.Register("u", typeof(UnderlineText));
            ComponentFactory.Register("font", typeof(FontAttributes));
            ComponentFactory.Register("sp", typeof(DocumentSpace));
            ComponentFactory.Register("tab", typeof(DocumentTab));
            ComponentFactory.Register("field", typeof(DocumentIdedField), "name", "id");
            ComponentFactory.Register("field", "name", typeof(DocumentNamedField));
            ComponentFactory.Register("functionField", typeof(DocumentIdedFunctionField), "instanceId");
            ComponentFactory.Register("invitation", typeof(DocumentIdedInvitationField), "id");
            ComponentFactory.Register("invitation", typeof(DocumentNamedInvitationField), "form", "project");
            ComponentFactory.Register("hyperlink", typeof(IdedHyperlinkField), "id");
            ComponentFactory.Register("link", typeof(NamedHyperlinkField));
            ComponentFactory.Register("image", "id", typeof(GraphicImageReference));
            ComponentFactory.Register("image", typeof(GraphicImage));

            IFunctionRepository functionRepository = FunctionLoader.Repository;
            if (functionRepository != null)
            {
                foreach (IFunctionInfo info in functionRepository.Functions)
                {
                    ComponentFactory.Register(info.Id, typeof(DocumentPersistedFunctionField));
                }
            }
        }

        public Paragraph()
        {
        }

        public Paragraph(int indent, string align)
        {
            this.indent = indent;
            this.align = align;
        }

        public Paragraph(IXmlElement element)
        {
            indent = element.GetAttribute("indent", defaultIndent);
            align = element.GetAttribute("align", defaultAlign);
            tabPositions = new TabPositions(element.GetChild("tabPositions"));

            Collection<XmlElement> childElements = element.GetChildren();

            foreach (XmlElement childElement in childElements)
            {
                if (childElement.Name != "tabPositions")
                {
                    contents.Add(ComponentFactory.MakeObject(childElement));
                }
            }
        }

        public TabPositions TabPositions { get { return tabPositions; } }

        public ParagraphComponentList Contents { get { return contents; } }

        public string Align { get { return align; } set { align = value; } }

        public int Indent { get { return indent; } set { indent = value; } }

        #region IDocumentBlock Members

        public virtual string ToXml()
        {
            var xmlString = new StringBuilder();

            xmlString.AppendFormat(xmlParagraphStartTag, indent, align);

            xmlString.Append(tabPositions.ToXml());

            foreach (IParagraphComponent component in contents)
            {
                xmlString.Append(component.ToXml());
            }

            xmlString.Append(xmlParagraphEndTag);

            return xmlString.ToString();
        }

        public virtual string ToHtml()
        {
            return (containsTabs() ? toTableHtml() : toParagraphHtml());
        }

        public virtual string ToRtf()
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
                rtfString.Append(component.ToRtf());
            }

            rtfString.Append(rtfParagraphEnd);

            return rtfString.ToString();
        }

        public virtual string ToRtf(RtfDocument document)
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
                if (component is FontAttributes)
                {
                    rtfString.Append(((FontAttributes)component).ToRtf(document));
                }
                else
                {
                    rtfString.Append(component.ToRtf());
                }
            }

            rtfString.Append(rtfParagraphEnd);

            return rtfString.ToString();
        }

        public virtual string Text
        {
            get
            {
                var textString = new StringBuilder();

                foreach (IParagraphComponent component in contents)
                {
                    textString.Append(component.Text);
                }

                return textString.ToString();
            }
        }

        #endregion

        public void Add(string text)
        {
            contents.Add(new DocumentText(text));
        }

        public void Add(IParagraphComponent component)
        {
            contents.Add(component);
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
    }
}