// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Projects.Documents;
using Tawala.Projects.Expressions;
using Tawala.Projects.Properties;
using Tawala.RtfSupport;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms
{
    /// <summary>
    /// Class to encapsulate Fill-in-the-blank items on Forms
    /// </summary>
    [Serializable]
    public class FibItem : FormItem, IFibItem, IOperatorDataSource
    {
        private const string defaultLabelPrefix = "Q";
        private const string xmlFibEndTag = "</fib>\r\n";
        private const string xmlFibStartTag = "<fib label=\"{0}\"{1}>";
        private const string xmlFibStartTagWithStyle = "<fib label=\"{0}\"{1} style=\"{2}\">";
        private static readonly FibItemOwnedFactory<IDocumentBlock> blockFactory = new FibItemOwnedFactory<IDocumentBlock>();
        private static readonly FibItemOwnedFactory<IParagraphComponent> componentFactory = new FibItemOwnedFactory<IParagraphComponent>();
        private readonly BlankList blankList = new BlankList();

        #region IDefaultLabel

        public string DefaultLabelPrefix { get { return defaultLabelPrefix; } }

        public string ToXml(string label)
        {
            var xmlString = new StringBuilder();

            if (String.IsNullOrEmpty(Style))
            {
                xmlString.AppendFormat(xmlFibStartTag, label, GetAlternateLabelXml());
            }
            else
            {
                xmlString.AppendFormat(xmlFibStartTagWithStyle, label, GetAlternateLabelXml(), Style);
            }

            xmlString.Append(contentsToXml());

            xmlString.Append(displayConditionsToXml());

            xmlString.Append(xmlFibEndTag);

            return xmlString.ToString();
        }

        #endregion

        static FibItem()
        {
            blockFactory.Register("paragraph", typeof(FormItemParagraph));

            componentFactory.Register("#text", typeof(FormItemFormattedText));
            componentFactory.Register("#whitespace", typeof(FormItemFormattedText));
            componentFactory.Register("blank", typeof(Blank));
            componentFactory.Register("field", "name", typeof(TextItemField));
        }

        public FibItem()
        {
            Rtf = Resources.FibItemDefaultRTF;
            Style = ((Project.Current != null && Project.Current.GlobalFibItemStyle != null)
                         ? Project.Current.GlobalFibItemStyle
                         : "topLabels");

            Project.FieldMapById.AddUnique(this);
        }

        public FibItem(ExternalForm form, string alternateLabel) : base(form, string.Empty)
        {
            var blank = new Blank(this, 1);
            blankList.Add(blank);
            blank.AlternateLabel = alternateLabel;
        }

        public FibItem(IXmlElement element)
        {
            AlternateLabel = element.GetAttribute("alternateLabel");
            Style = element.GetAttribute("style");
            fontTable = getFontTable(element);
            colorTable = getColorTable(element);

            if (element.HasChild("paragraph"))
            {
                contents = makeContentsFromParagraph(element.OuterXml);
            }
            else
            {
                contents = makeContentsFromTextAndBlanks(element.OuterXml);
            }

            getDisplayConditions(element);

            Project.FieldMapById.AddUnique(this);
        }

        public string Rtf
        {
            get { return ToRtf(); }
            set
            {
                parser = new RtfFibParser(value);
                parser.Parse();

                contents = makeContentsFromParagraph(String.Format("<fib{0}>{1}</fib>\r\n", GetAlternateLabelXml(), parser.ToXml()));
            }
        }

        public override string Text
        {
            get
            {
                string textString = "";

                foreach (IDocumentBlock block in contents)
                {
                    textString += block.Text;
                }

                return textString;
            }
            set
            {
                string rtfStartString = @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + @"{\fonttbl" + @"{\f0\fswiss Arial;}" + @"}" + @"{\colortbl;" +
                                        @"\red0\green0\blue0;" + @"\red255\green255\blue255;" + @"}" + @"\pard {\f0\fs18\cf1 ";

                string rtfEndString = @"}\par }";

                Rtf = rtfStartString + value + rtfEndString;
            }
        }

        public BlankList BlankList { get { return blankList; } }

        #region IFibItem Members

        public override bool IsQuestionItem { get { return true; } }

        public override void Eliminate()
        {
            base.Eliminate();

            foreach (Blank blank in blankList)
            {
                blank.Eliminate();
            }
        }

        Collection<IBlank> IFibItem.BlankList
        {
            get
            {
                var blanks = new Collection<IBlank>();

                foreach (IBlank blank in (this).BlankList)
                {
                    blanks.Add(blank);
                }

                return blanks;
            }
        }

        public void InsertBlanksIntoFieldMapByName()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IField Interface

        public override string FieldName
        {
            get
            {
                string fieldName = "";

                if (AlternateLabel.Length != 0)
                {
                    fieldName = AlternateLabel;
                }
                else
                {
                    fieldName = Project.Current.GetDefaultLabel(this);
                }

                return fieldName;
            }
        }

        public override string FieldString { get { return "<<" + FieldName + ">>"; } }

        public override IField this[string name]
        {
            get
            {
                if (name == FieldName)
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
            yield return this;
        }

        #endregion

        #region IRecursiveEnumerable Interface

        public override IEnumerable RecursiveEnumerator
        {
            get
            {
                foreach (IField blank in blankList)
                {
                    yield return blank;
                }
            }
        }

        #endregion

        #region IOperatorDataSource

        public IList OperatorDataSource { get { return HybridOperator.List.DataSource; } }

        #endregion

        #region IFormItem Interface

        public override IForm Form
        {
            set
            {
                base.Form = value;

                if (form != null)
                {
                    foreach (Blank blank in blankList)
                    {
                        Project.FieldMapByName.AddUnique(blank);
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Builds the FIB item's contents from the XML string that starts with a &lt;fibt&gt; element
        /// and contains one or more &lt;paragraph&gt; elements.
        /// </summary>
        private Collection<IDocumentBlock> makeContentsFromParagraph(string xmlString)
        {
            var contents = new Collection<IDocumentBlock>();

            IXmlElement element = new XmlElement(xmlString, true);

            fontTable = getFontTable(element);
            colorTable = getColorTable(element);

            Collection<XmlElement> blockElements = element.GetChildren();

            blankList.BlankIndex = 0;

            foreach (IXmlElement blockElement in blockElements)
            {
                contents.Add(blockFactory.MakeObject(blockElement, this));
            }

            blankList.Truncate(blankList.BlankIndex);

            return contents;
        }

        /// <summary>
        /// Builds the FIB item's contents from the XML string that starts with a &lt;fibt&gt; element
        /// and contains one or more &lt;blank&gt; or text elements.
        /// </summary>
        private Collection<IDocumentBlock> makeContentsFromTextAndBlanks(string xmlString)
        {
            var contents = new Collection<IDocumentBlock>();

            var paragraph = new FormItemParagraph();

            IXmlElement element = new XmlElement(xmlString, true);

            Collection<XmlElement> components = element.GetChildren();

            blankList.BlankIndex = 0;

            foreach (IXmlElement component in components)
            {
                if (component.Name == "blank")
                {
                    paragraph.Add(blankList.MakeBlank(component, this));
                    blankList.BlankIndex++;
                }
                else
                {
                    paragraph.Add(componentFactory.MakeObject(component));
                }
            }

            blankList.Truncate(blankList.BlankIndex);

            contents.Add(paragraph);

            return contents;
        }

        public string GetLabel(int blankIndex)
        {
            if (blankHasAlternateLabel(blankIndex))
            {
                return blankList.GetLabel(blankIndex);
            }

            if (hasAlternateLabel())
            {
                return AlternateLabel + ":" + blankList.GetLabel(blankIndex);
            }

            return form.GetDefaultLabel(this) + ":" + blankList.GetLabel(blankIndex);
        }

        private bool blankHasAlternateLabel(int blankIndex)
        {
            return !blankList[blankIndex].AlternateLabel.Equals(string.Empty);
        }

        private bool hasAlternateLabel()
        {
            return !AlternateLabel.Equals(string.Empty);
        }

        // the list of blanks associated with this item

        private string contentsToXml()
        {
            var xmlString = new StringBuilder();

            foreach (IDocumentBlock block in contents)
            {
                if (block != null)
                {
                    xmlString.Append(block.ToXml());
                }
            }

            return xmlString.ToString();
        }

        private string styleToXml()
        {
            var xmlString = new StringBuilder();

            if (!String.IsNullOrEmpty(Style))
            {
                xmlString.AppendFormat("<style name=\"{0}\" />", Style);
            }

            return xmlString.ToString();
        }

        protected string getStyleName(IXmlElement element)
        {
            if (element.HasChild("style"))
            {
                return element.GetChild("style").GetAttribute("name");
            }

            return "";
        }
    }

    [Serializable]
    public class FormItemFormattedText : FormItemText
    {
        public FormItemFormattedText(IXmlElement element) : base(element)
        {
        }

        public override string ToXml()
        {
            //			return "<font face=\"Arial\" size=\"180\" color=\"000000\">" + text + "</font>";
            return "<font size=\"180\" color=\"000000\">" + text + "</font>";
        }
    }
}