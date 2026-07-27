// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Projects.Documents;
using Tawala.Projects.Expressions;
using Tawala.Projects.Factories;
using Tawala.Projects.Fields;
using Tawala.Projects.Properties;
using Tawala.RtfSupport;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms
{
    [Serializable]
    public class FileUploadItem : FormItem, IFileUploadItem, IDeserializedField, IAssignableField
    {
        private const string defaultLabelPrefix = "F";
        private const string paragraphClose = "</paragraph>";
        private const string xmlFileNameInputTag = "<fileNameInput/>";
        private const string xmlRootTagName = "file";
        private static readonly Factory<IDocumentBlock> blockFactory = new Factory<IDocumentBlock>();

        #region IDefaultLabel

        public string DefaultLabelPrefix { get { return defaultLabelPrefix; } }

        public string ToXml(string label)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("<{0} label=\"{1}\"", xmlRootTagName, label);
            sb.Append(GetAlternateLabelXml());
            sb.AppendFormat(" required=\"{0}\" style=\"{1}\">", Required.ToString().ToLowerInvariant(), Style);

            foreach (IDocumentBlock block in contents)
            {
                sb.Append(block.ToXml());
            }

            sb.Replace(paragraphClose, xmlFileNameInputTag + paragraphClose);

            sb.AppendFormat("</{0}>\r\n", xmlRootTagName);
            return sb.ToString();
        }

        #endregion

        static FileUploadItem()
        {
            blockFactory.Register("paragraph", typeof(FormItemParagraph));
        }

        public FileUploadItem()
        {
            Project.FieldMapById.AddUnique(this);

            Required = false;
            Style = "topLabels";

            if (contents == null || contents.Count == 0)
            {
                Rtf = Resources.FileUploadItemDefaultRTF;
            }
        }

        public FileUploadItem(ExternalForm form, string alternateLabel) : base(form, alternateLabel)
        {
        }

        /// <summary>
        /// Constructs an MCItem from an &lt;mc&gt; element.
        /// </summary>
        public FileUploadItem(IXmlElement element)
        {
            AlternateLabel = getAttribute(element, "alternateLabel", null);
            Style = getAttribute(element, "style", "topLabels");
            Required = getAttributeAsBool(element, "required", false);

            Project.FieldMapById.AddUnique(this);

            contents = makeContentsFromParagraph(element.OuterXml);
        }

        #region IAssignableField Members

        public string AssignmentName { get { return QualifiedFieldName; } }

        #endregion

        #region IDeserializedField Members

        public IDeserializedField DeserializedFieldReference { get { return (IDeserializedField)Project.FieldMapById[Id]; } }

        #endregion

        #region IFileUploadItem Members

        public override bool IsQuestionItem { get { return true; } }

        public bool Required { get; set; }

        public string Rtf
        {
            get { return ToRtf(); }
            set
            {
                parser = new RtfParser(value);
                parser.Parse();

                contents = makeContentsFromParagraph(String.Format("<file{0}>{1}</file>\r\n", GetAlternateLabelXml(), parser.ToXml()));
            }
        }

        #endregion

        private static string getAttribute(IXmlElement element, string name, string notPresent)
        {
            if (element.HasAttribute(name))
            {
                return element.GetAttribute(name);
            }
            return notPresent;
        }

        private bool getAttributeAsBool(IXmlElement element, string name, bool notPresent)
        {
            if (element.HasAttribute(name))
            {
                return Convert.ToBoolean(element.GetAttribute(name));
            }
            return notPresent;
        }

        /// <summary>
        /// Builds the FIB item's contents from the XML string that starts with a &lt;fibt&gt; element
        /// and contains one or more &lt;paragraph&gt; elements.
        /// </summary>
        private Collection<IDocumentBlock> makeContentsFromParagraph(string xmlString)
        {
            xmlString.Replace(xmlFileNameInputTag, string.Empty);

            var contentsCollection = new Collection<IDocumentBlock>();

            IXmlElement element = new XmlElement(xmlString, true);

            fontTable = getFontTable(element);
            colorTable = getColorTable(element);

            IXmlElement paragraphElement = element.GetChild(0);
            contentsCollection.Add(blockFactory.MakeObject(paragraphElement));
            return contentsCollection;
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
                return AlternateLabel.Length != 0 ? AlternateLabel : Project.Current.GetDefaultLabel(this);
                ;
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

        public IList OperatorDataSource { get { return HybridOperator.List.DataSource; } }

        #endregion
    }
}