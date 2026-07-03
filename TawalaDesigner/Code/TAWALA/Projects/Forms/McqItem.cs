// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Common;
using Tawala.Functions.Runtime;
using Tawala.Projects.Documents;
using Tawala.Projects.Expressions;
using Tawala.Projects.Factories;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Properties;
using Tawala.RtfSupport;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms
{
    [Serializable]
    public class McqItem : FormItem, IDeserializedField, IAssignableField, IMcqItem
    {
        private static readonly Factory<IDocumentBlock> blockFactory = new Factory<IDocumentBlock>();
        private readonly ChoiceList choiceList = new ChoiceList();

        /// <summary>
        /// The question (first line of text) associated with this item 
        /// </summary>
        private readonly MCItemQuestion question;

        private int columnCount;

        [NonSerialized]
        private IFunction dataSourceFunction;

        [NonSerialized]
        private IXmlElement dynamicMcqElement;

        private string dynamicMcqFunctionXml;
        private bool requireAtLeastOne;
        private bool selectOnlyOne = true;

        static McqItem()
        {
            blockFactory.Register("question", typeof(MCItemQuestion));
        }

        public McqItem()
        {
            Project.FieldMapById.AddUnique(this);

            Rtf = Resources.MCItemDefaultRTF;
            Style = ((Project.Current != null && Project.Current.GlobalMCItemStyle != null) ? Project.Current.GlobalMCItemStyle : "vertical");
            PaddingBottom = true;
        }

        public McqItem(ExternalForm form, string alternateLabel, int choiceCount, bool onlyOne) : base(form, alternateLabel)
        {
            selectOnlyOne = onlyOne;

            for (int i = 0; i < choiceCount; ++i)
            {
                choiceList.Add(new Choice());
            }
        }

        /// <summary>
        /// Constructs an MCItem from an &lt;mc&gt; element.
        /// </summary>
        public McqItem(IXmlElement element) : this()
        {
            AlternateLabel = element.GetAttribute("alternateLabel");
            requireAtLeastOne = element.GetAttribute("required") == "true";
            selectOnlyOne = (element.GetAttribute("onlyone") == "true");
            Style = element.GetAttribute("style");
            if (string.IsNullOrEmpty(Style))
            {
                Style = "vertical";
            }
            columnCount = Convert.ToInt32(element.GetAttribute("columnCount"));
            PaddingBottom = element.HasAttribute("paddingBottom") ? (element.GetAttribute("paddingBottom") == "true") : true;

            fontTable = getFontTable(element);
            colorTable = getColorTable(element);

            question = new MCItemQuestion(element.GetChild("question"));

            contents.Clear();
            contents.Add(question);

            if (choicesComeFromDataProvider(element))
            {
                Project.Events.ProjectOpened += events_ProjectOpened;
                setDataProvider(element.GetChild("data-provider"));
            }
            else
            {
                setChoiceList(element);
            }

            getDisplayConditions(element);
        }

        public string Rtf
        {
            get { return ToRtf(); }
            set
            {
                parser = new RtfParser(value);
                parser.Parse();

                contents = makeContentsFromParagraphs(String.Format("<mc{0}>{1}</mc>\r\n", GetAlternateLabelXml(), parser.ToXml()));
            }
        }

        public override string Text
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
            set
            {
                // replace fields and special characters
                string newText = Regex.Replace(value, @"<<.+?>>|&|<|>", new MatchEvaluator(EscapeSpecialCharacters));

                text = Regex.Replace(newText, @"<<([^>]+)>>", "<field name=\"$1\"/>");
                contents = makeContentsFromText(text);

                choiceList.Clear();
                choiceList.Add(new Choice(string.Empty));
            }
        }

        /// <summary>
        /// The zero-based index corresponding to the final choice in the choice list.
        /// </summary>
        public int MaximumChoiceIndex { get { return (choiceList.Count - 1); } }

        #region IAssignableField Members

        public string AssignmentName { get { return QualifiedFieldName; } }

        #endregion

        #region IDeserializedField Members

        public IDeserializedField DeserializedFieldReference { get { return (IDeserializedField)Project.FieldMapById[Id]; } }

        #endregion

        #region IDefaultLabel

        private const string defaultLabelPrefix = "Q";

        public string DefaultLabelPrefix { get { return defaultLabelPrefix; } }

        public string ToXml(string label)
        {
            var xmlString = new StringBuilder();

            xmlString.AppendFormat("<mc {0}>", GetXmlAttributes(label));

            foreach (IDocumentBlock block in contents)
            {
                xmlString.Append(block.ToXml());
            }

            if (dataSourceFunction == null)
            {
                xmlString.Append(choiceList.ToXml());
            }
            else
            {
                xmlString.Append("<data-provider>");
                xmlString.Append(dataSourceFunction.ToXml());
                xmlString.Append("</data-provider>");
            }

            xmlString.Append(displayConditionsToXml());

            xmlString.Append("</mc>\r\n");

            return xmlString.ToString();
        }

        #endregion

        #region IMcqItem Members

        public override bool IsQuestionItem { get { return true; } }

        public IFunction DataSourceFunction { get { return dataSourceFunction; } set { dataSourceFunction = value; } }

        /// <summary>
        /// If true, use radio buttons. If false, use check boxes.
        /// </summary>
        public bool SelectOnlyOne { get { return selectOnlyOne; } set { selectOnlyOne = value; } }

        /// <summary>
        /// If true, the user must select at least one choice
        /// </summary>
        public bool RequireAtLeastOne { get { return requireAtLeastOne; } set { requireAtLeastOne = value; } }

        public int ColumnCount { get { return columnCount; } set { columnCount = value; } }

        public IChoiceList Choices { get { return choiceList; } }

        public int ChoiceSourceIndex { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public string ChoicesXhtml { get { throw new NotImplementedException(); } }

        public Question Question { get { throw new NotImplementedException(); } }

        public IFormItemContents QuestionContents { get { throw new NotImplementedException(); } }

        public IFormItemContents ChoiceContents { set { throw new NotImplementedException(); } }

        public bool PaddingBottom { get; set; }

        #endregion

        private static bool choicesComeFromDataProvider(IXmlElement element)
        {
            return element.HasChild("data-provider");
        }

        /// <summary>
        /// Builds the MC item's contents from the XML string that starts with an &lt;mc&gt; element
        /// and contains one &lt;paragraph&gt; element for each question or choice.
        /// </summary>
        private Collection<IDocumentBlock> makeContentsFromParagraphs(string xmlString)
        {
            var contents = new Collection<IDocumentBlock>();

            IXmlElement element = new XmlElement(xmlString, true);

            fontTable = getFontTable(element);
            colorTable = getColorTable(element);

            IXmlElement paragraphElement = element.GetChild(0);
            string questionElementXmlString = String.Format("<question>{0}</question>", paragraphElement.OuterXml);
            IXmlElement questionElement = new XmlElement(questionElementXmlString);
            contents.Add(new MCItemQuestion(questionElement));

            choiceList.Clear();

            for (int i = 1; i < element.GetChildren().Count; i++)
            {
                paragraphElement = element.GetChild(i);
                string choiceElementXmlString = String.Format("<choice label=\"{0}\">{1}</choice>", new AlphaLabel(i - 1),
                                                              paragraphElement.OuterXml);
                IXmlElement choiceElement = new XmlElement(choiceElementXmlString);
                choiceList.Add(new Choice(choiceElement));
            }

            return contents;
        }

        /// <summary>
        /// Builds the MC item's contents from the specified text.
        /// </summary>
        private static Collection<IDocumentBlock> makeContentsFromText(string text)
        {
            var contents = new Collection<IDocumentBlock>();

            contents.Add(new MCItemQuestion(text));

            return contents;
        }

        private void setChoiceList(IXmlElement element)
        {
            Collection<XmlElement> choiceElements = element.GetDescendants("choice");

            choiceList.Clear();

            foreach (XmlElement choiceElement in choiceElements)
            {
                choiceList.Add(new Choice(choiceElement));
            }
        }

        [OnSerializing]
        private void onSerializingMcq(StreamingContext context)
        {
            if (DataSourceFunction != null)
            {
                dynamicMcqFunctionXml = DataSourceFunction.ToXml();
            }
        }

        [OnDeserialized]
        private void onDeserializedMcq(StreamingContext context)
        {
            regenerateDynamicMcqFunction();
        }

        private void setDataProvider(IXmlElement dataProviderElement)
        {
            dynamicMcqElement = dataProviderElement.GetChild("dynamic-mcq");
        }

        private void events_ProjectOpened(object sender, ProjectEventArgs e)
        {
            regenerateDynamicMcqFunction();
        }

        private void regenerateDynamicMcqFunction()
        {
            if (dynamicMcqFunctionXml != null)
            {
                dynamicMcqElement = new XmlElement(dynamicMcqFunctionXml);
            }

            if (dynamicMcqElement != null)
            {
                var converter = new XmlToFunctionConverter();
                dataSourceFunction = converter.ConvertFrom(dynamicMcqElement);
            }

            dynamicMcqFunctionXml = null;
            dynamicMcqElement = null;
        }

        protected string GetXmlAttributes(string label)
        {
            var attributeXml = new StringBuilder();

            if (columnCount == 0)
            {
                attributeXml.AppendFormat("label=\"{0}\"{1} onlyone=\"{2}\" required=\"{3}\" style=\"{4}\"", label, GetAlternateLabelXml(),
                                          selectOnlyOne ? "true" : "false", requireAtLeastOne ? "true" : "false", Style);
            }
            else
            {
                attributeXml.AppendFormat("label=\"{0}\"{1} onlyone=\"{2}\" required=\"{3}\" style=\"{4}\" columnCount=\"{5}\"", label,
                                          GetAlternateLabelXml(), selectOnlyOne ? "true" : "false", requireAtLeastOne ? "true" : "false",
                                          Style, columnCount);
            }
            if (!PaddingBottom)
            {
                attributeXml.Append(" paddingBottom=\"false\"");
            }

            return attributeXml.ToString();
        }

        public override string ToRtf()
        {
            var rtfString = new StringBuilder(rtfProlog);

            RtfDocument emptyDocument = getSkeletalRtfDocument();

            rtfString.Append(emptyDocument.FontTable.ToRtf());
            rtfString.Append(emptyDocument.ColorTable.ToRtf());

            rtfString.Append(rtfDefaultTabs);

            foreach (IDocumentBlock block in contents)
            {
                if (block != null)
                {
                    rtfString.Append(block.ToRtf(emptyDocument));
                }
            }

            rtfString.Append(choiceList.ToRtf(emptyDocument));

            rtfString.Append(rtfEnd);

            return rtfString.ToString();
        }

        public string ToHtml(string label)
        {
            var htmlString = new StringBuilder();

            htmlString.Append("<div class=\"mcCheckbox\">");
            htmlString.Append("<div class=\"question\">");

            foreach (IDocumentBlock block in contents)
            {
                htmlString.Append(block.ToHtml());
            }

            return htmlString.ToString();
        }

        public override string ToString()
        {
            return QualifiedFieldName;
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

        public override string QualifiedFieldName
        {
            get
            {
                IForm form;

                if ((form = GetContainingForm()) == Projects.Form.NULL)
                {
                    return FieldUtil.UnknownFieldName;
                }
                return form.Name + ":" + FieldName;
            }
        }

        public override string FieldString { get { return "<<" + QualifiedFieldName + ">>"; } }

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
            yield return this;
        }

        #endregion

        #region IRecursiveEnumerable Interface

        public override IEnumerable RecursiveEnumerator { get { yield return this; } }

        #endregion

        #region IOperatorDataSource

        public IList OperatorDataSource { get { return (selectOnlyOne ? MCOneOperator.List.DataSource : MCManyOperator.List.DataSource); } }

        #endregion
    }
}