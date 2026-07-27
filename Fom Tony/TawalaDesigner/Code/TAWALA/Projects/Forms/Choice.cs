// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Projects.Documents;
using Tawala.Projects.Expressions;
using Tawala.Projects.Factories;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms
{
    /// <summary>
    /// Class to contain a single choice in a multiple choice question
    /// </summary>
    [Serializable]
    public class Choice : IPaletteField, IDeserializedField, IChoice
    {
        private const string xmlChoiceEndTag = "</choice>";
        private const string xmlChoiceStartTag = "<choice label=\"$LABEL\">";
        private static readonly ChoiceBlockFactory<IDocumentBlock> blockFactory = new ChoiceBlockFactory<IDocumentBlock>();
        private readonly string label = "";

        protected ChoiceBlockCollection contents = new ChoiceBlockCollection();

        static Choice()
        {
            blockFactory.Register("paragraph", typeof(ChoiceParagraph));
        }

        public Choice()
        {
            Project.FieldMapById.AddUnique(this);
        }

        public Choice(string initialText) : this()
        {
            Text = initialText;
        }

        public Choice(IXmlElement element) : this()
        {
            label = element.GetAttribute("label");

            if (element.HasChild("paragraph"))
            {
                contents = makeContentsFromParagraph(element.OuterXml);
            }
            else
            {
                contents = makeContentsFromText(element.Text);
            }
        }

        #region IChoice Members

        /// <summary>
        /// Choice Text
        /// </summary>
        public string Text
        {
            get
            {
                var itemText = new StringBuilder();

                foreach (IDocumentBlock block in contents)
                {
                    itemText.Append(block.Text);
                }

                return itemText.ToString();
            }
            set { contents = makeContentsFromText(value); }
        }

        public string ToXml(string label)
        {
            var xmlString = new StringBuilder(xmlChoiceStartTag);
            xmlString.Replace("$LABEL", label);

            foreach (IDocumentBlock block in contents)
            {
                xmlString.Append(block.ToXml());
            }

            xmlString.Append(xmlChoiceEndTag);

            return xmlString.ToString();
        }

        public string ToRtf(string label, RtfDocument document)
        {
            var rtfString = new StringBuilder();

            foreach (IDocumentBlock block in contents)
            {
                rtfString.Append(block.ToRtf(document));
            }

            return rtfString.ToString();
        }

        public string ContentsXhtml(IFormItem formItem)
        {
            throw new NotImplementedException();
        }

        public string ToXml()
        {
            throw new NotImplementedException();
        }

        public string ToXhtml(IFormItem formItem)
        {
            throw new NotImplementedException();
        }

        public FormItemContentsCollection GetDescendants(Type descendantType)
        {
            throw new NotImplementedException();
        }

        public IFormItemContents Contents { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public void ApplyFontStyle(FontStyle style)
        {
            throw new NotImplementedException();
        }

        public FontStyle GetInnermostFontStyle()
        {
            throw new NotImplementedException();
        }

        public void ResolveFieldReferences()
        {
            throw new NotImplementedException();
        }

        public void ResolveFunctionReferences()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDeserializedField Members

        public IDeserializedField DeserializedFieldReference { get { return this; } }

        #endregion

        #region IPaletteField Members

        public string QualifiedFieldName { get { return FieldName; } }

        #endregion

        #region IEnumerable Interface

        public IEnumerator GetEnumerator()
        {
            yield break;
        }

        #endregion

        #region IRecursiveEnumerable Interface

        public IEnumerable RecursiveEnumerator { get { yield break; } }

        #endregion

        /// <summary>
        /// Builds the choice's contents from the XML string that starts with a &lt;choice&gt; element
        /// and contains one or more <paragraph> elements.
        /// </summary>
        private ChoiceBlockCollection makeContentsFromParagraph(string xmlString)
        {
            var contents = new ChoiceBlockCollection();

            IXmlElement element = new XmlElement(xmlString, true);

            foreach (IXmlElement blockElement in element.GetChildren())
            {
                contents.Add(blockFactory.MakeObject(blockElement, label));
            }

            return contents;
        }

        /// <summary>
        /// Builds the choice's contents from the specified text.
        /// </summary>
        private ChoiceBlockCollection makeContentsFromText(string text)
        {
            string choiceFormat =
                "<choice label=\"a\">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "{0}" +
                "</paragraph>" +
                "</choice>";

            string xmlString = String.Format(choiceFormat, makeFontsXmlString(text));
            return makeContentsFromParagraph(xmlString);
        }

        private string makeFontsXmlString(string text)
        {
            var fontsXmlString = new StringBuilder();

            string fontFormat =
                "<font face=\"Arial\" size=\"200\" color=\"000000\">" +
                "{0}" +
                "</font>";

            var expression = new NonArithmeticExpression(nullToEmptyString(text));

            if (expression.IsEmpty)
            {
                fontsXmlString.AppendFormat(fontFormat, String.Empty);
            }
            else
            {
                foreach (ExpressionElement expressionElement in expression.Elements)
                {
                    fontsXmlString.AppendFormat(fontFormat, expressionElement.ToXml());
                }
            }

            return fontsXmlString.ToString();
        }

        private string nullToEmptyString(string text)
        {
            return (text == null ? String.Empty : text);
        }

        public string ToHtml(string name, string type)
        {
            return contents.ToHtml(name, type);
        }

        #region IField Interface

        private readonly int id = Project.NextUniqueID;
        public string FieldName { get { return Text.Replace("<<", "").Replace(">>", ""); } }

        public string FieldString { get { return "<<" + FieldName + ">>"; } }

        public IField this[string name]
        {
            get
            {
                if (FieldName == name)
                {
                    return this;
                }

                return null;
            }
        }

        public int Id { get { return id; } }

        public override string ToString()
        {
            return FieldName;
        }

        #endregion
    }

    [Serializable]
    public class ChoiceBlockCollection : Collection<IDocumentBlock>
    {
        public string ToHtml(string name, string type)
        {
            var htmlString = new StringBuilder();

            htmlString.Append("<div class=\"answer\">");

            foreach (IDocumentBlock block in this)
            {
                if (block is ChoiceParagraph)
                {
                    var paragraph = block as ChoiceParagraph;

                    htmlString.Append(paragraph.ToHtml(name, type));
                }
            }

            htmlString.Append("</div>");

            return htmlString.ToString();
        }
    }

    /// <summary>
    /// Class to represent a paragraph that occurs within a Choice object.
    /// </summary>
    [Serializable]
    public class ChoiceParagraph : FormItemParagraph
    {
        private readonly ChoiceFontAttributes font;
        private readonly string label = "";

        /// <summary>
        /// Constructs a ChoiceParagraph object from an XML &lt;paragraph&gt; element.
        /// </summary>
        public ChoiceParagraph(IXmlElement element, string label)
        {
            this.label = label;
            indent = element.GetAttribute("indent", defaultIndent);
            align = element.GetAttribute("align", defaultAlign);
            tabPositions = new TabPositions(element.GetChild("tabPositions"));

            contents.Add(new ChoiceLabel(label, "Arial", 200, 0x000000));

            foreach (XmlElement childElement in element.GetChildren())
            {
                if (childElement.Name == "font")
                {
                    if (!containsOnlyChoiceLabel(childElement))
                    {
                        IXmlElement modifiedElement = new XmlElement(removeLabelPrefixes(childElement.OuterXml));
                        font = new ChoiceFontAttributes(modifiedElement, label);
                        contents.Add(font);
                    }
                }
                else if (childElement.Name == "field" || childElement.Name == "sp")
                {
                    contents.Add(componentFactory.MakeObject(childElement));
                }
            }
        }

        /// <summary>
        /// Indicates whether the specified element contains only a choice label (e.g. "   a) ").
        /// </summary>
        private bool containsOnlyChoiceLabel(IXmlElement element)
        {
			return Regex.IsMatch(element.OuterXml, @">( +)([a-z]+)\) <");
        }

        /// <summary>
        /// Removes all label prefixes (e.g. "   a) " from the text portions of the specified XML string.
        /// </summary>
        private string removeLabelPrefixes(string xmlString)
        {
            return Regex.Replace(xmlString, @">( +)([a-z]+)\) ([^<]*)<", ">$3<");
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

        public string ToHtml(string name, string type)
        {
            var htmlString = new StringBuilder();

            htmlString.Append("<p>");
            htmlString.AppendFormat("<input name=\"{0}\" type=\"{1}\" value=\"{2}\" />", name, type, label);
            htmlString.AppendFormat(font.ToHtml());
            htmlString.Append("</p>");

            return htmlString.ToString();
        }
    }

    /// <summary>
    /// Class to represent a font that occurs within a ChoiceParagraph object.
    /// </summary>
    [Serializable]
    public class ChoiceFontAttributes : FormItemFontAttributes
    {
        private static readonly Factory<IParagraphComponent> componentFactory = new Factory<IParagraphComponent>();
        protected string label = "";

        static ChoiceFontAttributes()
        {
            componentFactory.Register("#text", typeof(ChoiceText));
            componentFactory.Register("#whitespace", typeof(ChoiceText));
            componentFactory.Register("field", "name", typeof(FormItemNamedField));
            componentFactory.Register("b", typeof(BoldText));
            componentFactory.Register("i", typeof(ItalicText));
            componentFactory.Register("u", typeof(UnderlineText));
        }

        public ChoiceFontAttributes()
        {
        }

        public ChoiceFontAttributes(IXmlElement element, string label) : base(element)
        {
            this.label = label;

            IXmlElement childElement = element.GetChild(0);

            if (childElement == XmlElement.NULL)
            {
                contents = new ChoiceText();
            }
            else
            {
                contents = componentFactory.MakeObject(childElement);
            }
        }

        public override string ToRtf(RtfDocument document)
        {
            string rtfString = @"{{\f{0}\fs{1}\cf{2} {3}}}";

            int fontIndex = document.FontTable.IndexMatching(Face);
            int colorIndex = document.ColorTable.IndexMatching(Color);

            return string.Format(rtfString, fontIndex, TwipsToHalfPoints(Size), ZeroBasedToOneBased(colorIndex), contentsToRtf(document));
        }

        private string contentsToRtf(RtfDocument document)
        {
            if (contents is ChoiceText)
            {
                return ((ChoiceText)contents).ToRtf(document);
            }
            else
            {
                return contents.ToRtf();
            }
        }

        public override string ToHtml()
        {
            var htmlString = new StringBuilder();

            htmlString.AppendFormat("<span style=\"font-family: {0}; font-size: {1}pt; color:#{2:X6};\">", Face, Size/20, Color);
            htmlString.AppendFormat(contents.ToHtml());
            htmlString.AppendFormat("</span>");

            return htmlString.ToString();
        }
    }

    /// <summary>
    /// Class to represent a label that occurs within a Choice object.
    /// </summary>
    [Serializable]
    public class ChoiceLabel : ChoiceFontAttributes
    {
        public ChoiceLabel(string label, string face, int size, int color)
        {
            this.label = label;
            this.Face = face;
            this.Size = size;
            this.Color = color;
        }

        public override string ToRtf(RtfDocument document)
        {
            string rtfString = @"{{\f{0}\fs{1}\cf{2} {3}}}";

            int fontIndex = document.FontTable.IndexMatching(Face);
            int colorIndex = document.ColorTable.IndexMatching(Color);

            return string.Format(rtfString, fontIndex, TwipsToHalfPoints(Size), ZeroBasedToOneBased(colorIndex), labelToRtf());
        }

        private string labelToRtf()
        {
			var labelString = new StringBuilder();
			labelString.Append(' ', calculateNumberOfLeadingSpaces());
			labelString.Append(label);
			labelString.Append(") ");

			return labelString.ToString();
        }

    	private int calculateNumberOfLeadingSpaces()
    	{
    		return 4 - label.Length;
    	}

    	public override string ToXml()
        {
            return "";
        }
    }

    /// <summary>
    /// Class to represent the text that occurs within a ChoiceFontAttributes object.
    /// </summary>
    [Serializable]
    public class ChoiceText : FormItemText
    {
        public ChoiceText() : base("")
        {
        }

        public ChoiceText(IXmlElement element) : base(element)
        {
        }

        public ChoiceText(IXmlElement element, string label) : base(element)
        {
        }

        public override string ToRtf(RtfDocument document)
        {
            return ToRtf();
        }
    }
}