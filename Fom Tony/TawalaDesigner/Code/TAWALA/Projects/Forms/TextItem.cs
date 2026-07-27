// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.FontSupport;
using Tawala.Projects.Documents;
using Tawala.Projects.Factories;
using Tawala.Projects.Properties;
using Tawala.RtfSupport;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms
{
    /// <summary>
    /// Class to encapsulate Text items on Forms
    /// </summary>
    [Serializable]
    public class TextItem : FormItem, ITextItem
    {
        private const string tagName = "text";
        private static readonly Factory<IDocumentBlock> blockFactory = new Factory<IDocumentBlock>();

        static TextItem()
        {
            blockFactory.Register("paragraph", typeof(FormItemParagraph));
            blockFactory.Register("table", typeof(Table));
        }

        public TextItem()
        {
            Rtf = Resources.TextItemDefaultRTF;
            Style = ((Project.Current != null && Project.Current.GlobalTextItemStyle != null)
                         ? Project.Current.GlobalTextItemStyle
                         : "normal");
            PaddingBottom = true;
        }

        public TextItem(IXmlElement element)
        {
            AlternateLabel = element.GetAttribute("alternateLabel");
            Style = element.GetAttribute("style");
            if (string.IsNullOrEmpty(Style))
            {
                Style = "normal";
            }
            PaddingBottom = element.HasAttribute("paddingBottom") ? (element.GetAttribute("paddingBottom") == "true") : true;
            fontTable = getFontTable(element);
            colorTable = getColorTable(element);

            if (element.HasChild("paragraph") || element.HasChild("table"))
            {
                contents = makeContentsFromParagraph(element.OuterXml);
            }
            else
            {
                contents = makeContentsFromText(element.InnerXml);
            }

            getDisplayConditions(element);
        }

        protected TextItem(string rtf)
        {
            Rtf = rtf;
        }

        public string Rtf
        {
            get { return ToRtf(); }
            set
            {
                parser = new RtfParser(value);
                parser.Parse();

                contents =
                    makeContentsFromParagraph(String.Format("<{0}{1}>{2}</{0}>\r\n", XmlTagName, GetAlternateLabelXml(), parser.ToXml()));
            }
        }

        protected virtual string XmlTagName
        {
            get { return tagName; }
        }

        public override string Text
        {
            get
            {
                var itemText = new StringBuilder();

                foreach (var block in contents)
                {
                    itemText.Append(block.Text);
                }

                return itemText.ToString();
            }
            set
            {
                // replace fields and special characters
                string newText = Regex.Replace(value, @"<<.+?>>|&|<|>", new MatchEvaluator(EscapeSpecialCharacters));

                text = Regex.Replace(newText, @"<<([^>]+)>>", "<field name=\"$1\"/>");
                contents = makeContentsFromText(text);
            }
        }

        #region IDefaultLabel 

        private const string defaultLabelPrefix = "T";

        public virtual string DefaultLabelPrefix
        {
            get { return defaultLabelPrefix; }
        }

        public string ToXml(string label)
        {
            var xmlString = new StringBuilder();

            if (contents.Count > 0)
            {
                xmlString.AppendFormat("<{0}{1}>", XmlTagName, GetXmlAttributes(label));

                foreach (var block in contents)
                {
                    if (block != null)
                    {
                        xmlString.Append(block.ToXml());
                    }
                }

                xmlString.Append(displayConditionsToXml());

                xmlString.AppendFormat("</{0}>\r\n", XmlTagName);
            }
            else
            {
                string newText = Regex.Replace(Text, @"<<.+?>>|&|<|>", new MatchEvaluator(EscapeSpecialCharacters));

                xmlString.AppendFormat("<{0} label=\"{1}\"{2}>{3}</{0}>\r\n", XmlTagName, label, GetAlternateLabelXml(), newText);
            }

            return xmlString.ToString();
        }

        #endregion

        #region ITextItem Members

        public override bool IsTextItem
        {
            get { return true; }
        }

        public bool PaddingBottom { get; set; }

        #endregion

        protected virtual string GetXmlAttributes(string label)
        {
            var attributeXml = new StringBuilder();

            attributeXml.AppendFormat(" label=\"{0}\"{1} style=\"{2}\"", label, GetAlternateLabelXml(), Style);
            if (!PaddingBottom)
            {
                attributeXml.Append(" paddingBottom=\"false\"");
            }

            return attributeXml.ToString();
        }

        /// <summary>
        /// Builds the text item's contents from the XML string that starts with a &lt;text&gt; element
        /// and contains one or more &lt;paragraph&gt; or &lt;table&gt; elements.
        /// </summary>
        private Collection<IDocumentBlock> makeContentsFromParagraph(string xmlString)
        {
            var contents = new Collection<IDocumentBlock>();

            IXmlElement element = new XmlElement(xmlString, true);

            fontTable = getFontTable(element);
            colorTable = getColorTable(element);

            var blockElements = element.GetChildren();

            foreach (IXmlElement blockElement in blockElements)
            {
                contents.Add(blockFactory.MakeObject(blockElement));
            }

            return contents;
        }

        /// <summary>
        /// Builds the text item's contents from the specified text.
        /// </summary>
        private Collection<IDocumentBlock> makeContentsFromText(string text)
        {
            const string xmlParagraphFormat = "<{0}>" + "<paragraph>" + "{1}" + "</paragraph>" + "</{0}>";

            string xmlString = String.Format(xmlParagraphFormat, XmlTagName,
                                             text.Replace("\t", "<tab/>").Replace("\n", "</paragraph><paragraph>"));
            return makeContentsFromParagraph(xmlString);
        }

        /// <summary>
        /// Returns a font table built from the &lt;font&gt; elements that are descendants of the specified element. 
        /// </summary>
        protected override RtfFontTable getFontTable(IXmlElement element)
        {
            var fontTable = new RtfFontTable();

            var fontElements = element.GetDescendants("font");

            fontTable.AddUnique(new RtfFontTableEntry("swiss", "Arial"));
            fontTable.AddUnique(new RtfFontTableEntry("nil", Fonts.DefaultFontName));

            foreach (var fontElement in fontElements)
            {
                fontTable.AddUnique(new RtfFontTableEntry(fontElement));
            }

            return fontTable;
        }

        /// <summary>
        /// Returns a color table built from the &lt;font&gt; elements that are descendants of the specified element. 
        /// </summary>
        protected override RtfColorTable getColorTable(IXmlElement element)
        {
            var colorTable = new RtfColorTable();

            var fontElements = element.GetDescendants("font");

            colorTable.AddUnique(new RtfColorTableEntry(0, 0, 0));
            colorTable.AddUnique(new RtfColorTableEntry(255, 255, 255));
            colorTable.AddUnique(new RtfColorTableEntry(Fonts.DefaultFontRGB[0], Fonts.DefaultFontRGB[1], Fonts.DefaultFontRGB[2]));

            foreach (var fontElement in fontElements)
            {
                colorTable.AddUnique(new RtfColorTableEntry(fontElement));
            }

            return colorTable;
        }

        #region IField Interface

        public override string FieldName
        {
            get
            {
                string fieldName = AlternateLabel.Length != 0 ? AlternateLabel : Project.Current.GetDefaultLabel(this);
                return fieldName;
            }
        }

        public override string FieldString
        {
            get { return "<<" + FieldName + ">>"; }
        }

        public override IField this[string name]
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

        #endregion

        #region IEnumerable Interface

        public override IEnumerator GetEnumerator()
        {
            yield break;
        }

        #endregion

        #region IRecursiveEnumerable Interface

        public override IEnumerable RecursiveEnumerator
        {
            get
            {
                //				yield return this;
                yield break;
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents either a text string or a field in a TextItem.
    /// </summary>
    public interface ITextItemComponent
    {
        /// <summary>
        /// Gets a text string representation of the component. For a text component, this is the text itself.
        /// For a field component, this is a string representing the field, inside double angle brackets (e.g. "<<Form 1:Q1:a>>").
        /// </summary>
        string Text { get; }
    }

    public class TextItemText : ITextItemComponent
    {
        private readonly string text;

        public TextItemText(IXmlElement element)
        {
            text = element.Value;
        }

        #region ITextItemComponent Members

        public string Text
        {
            get { return text; }
        }

        #endregion
    }

    public class TextItemField : ITextItemComponent
    {
        private readonly string text;

        public TextItemField(IXmlElement element)
        {
            text = element.GetAttribute("name");
        }

        #region ITextItemComponent Members

        public string Text
        {
            get { return "<<" + text + ">>"; }
        }

        #endregion
    }
}