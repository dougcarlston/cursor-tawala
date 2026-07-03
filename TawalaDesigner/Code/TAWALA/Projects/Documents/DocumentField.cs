// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Tawala.Common;
using Tawala.Projects.Fields;
using Tawala.RtfSupport;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
{
    [Serializable]
    public abstract class DocumentField : ParagraphComponent, IDocumentConversions
    {
        private const string encodedFieldPrefix = "540046002400"; // "TF$"

        [NonSerialized]
        protected IPaletteField field = PaletteField.NULL;

        public IPaletteField Field
        {
            get { return field; }
        }

        #region IDocumentConversions Members

        public override string ToXml()
        {
            if (field is UnresolvedPaletteField)
            {
                field = resolveField(field);
            }

            return String.Format("<field name=\"{0}\"/>", XMLStringFormatter.EscapeAttributeText(field.QualifiedFieldName));
        }

        public override string ToHtml()
        {
            return String.Format("&lt;&lt;{0}&gt;&gt;", field.QualifiedFieldName);
        }

        public override string ToRtf()
        {
            string rtfString =
                @"{{\*\txfieldstart\txfieldtype0\txfieldflags" + RtfUtility.NonEditableFieldFlags +
                @"\txfielddataval{0}" +
                @"\txfielddata " + encodedFieldPrefix + "{1}}}" +
                @"<<{2}>>{{" +
                @"\*\txfieldend}}";

            return String.Format(rtfString, field.Id, RtfUtility.EncodeHexString(field.FieldName), field.QualifiedFieldName);
        }

        public override string ToRtf(RtfDocument document)
        {
            return ToRtf();
        }

        #endregion

        /// <summary>
        /// Converts an unresolved field to a valid field reference (if one exists).
        /// </summary>
        private IPaletteField resolveField(IPaletteField unresolvedField)
        {
            string formName = FieldUtil.GetFormName(unresolvedField.QualifiedFieldName);
            string fieldName = FieldUtil.GetFieldName(unresolvedField.QualifiedFieldName);

            var resolvedField = (IPaletteField)Project.Current.GetForm(formName).ItemList[fieldName];

            return (resolvedField ?? unresolvedField);
        }
    }

    [Serializable]
    public class DocumentIdedField : DocumentField
    {
        private readonly int id = -1;

        public DocumentIdedField(IXmlElement element)
        {
            string fullFieldName = element.GetAttribute("name");
            int id = Convert.ToInt32(element.GetAttribute("id"));

            field = FieldUtil.InitializeIdedField(fullFieldName, id);
        }

        [OnDeserialized]
        private void onDeserialized(StreamingContext context)
        {
            field = (Project.FieldMapById.ContainsKey(id) ? (IPaletteField)Project.FieldMapById[id] : PaletteField.NULL);
        }
    }

    [Serializable]
    public class DocumentNamedField : DocumentField
    {
        private string serializedFieldName;

        public DocumentNamedField(IXmlElement element)
        {
            string fullFieldName = element.GetAttribute("name");
            field = FieldUtil.InitializeNamedField(fullFieldName);
        }

        private static bool isFormQualified(string fullFieldName)
        {
            foreach (var form in Project.Current.FormList)
            {
                if (fullFieldName.StartsWith(form.Name + ":"))
                {
                    return true;
                }
            }

            return false;
        }

        private static string getFormName(string fullFieldName)
        {
            return Regex.Match(fullFieldName, @"([^:]+):.*").Groups[1].Value;
        }

        private static string getFieldName(string fullFieldName)
        {
            return Regex.Match(fullFieldName, @"[^:]+:(.*)").Groups[1].Value;
        }

        [OnSerializing]
        private void onSerializing(StreamingContext context)
        {
            serializedFieldName = field.QualifiedFieldName;
        }

        [OnDeserialized]
        private void onDeserialized(StreamingContext context)
        {
            field = FieldUtil.InitializeNamedField(serializedFieldName);
            serializedFieldName = null;
        }
    }
}