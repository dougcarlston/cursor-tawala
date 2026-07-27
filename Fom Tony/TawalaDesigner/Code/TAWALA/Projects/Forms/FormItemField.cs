// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Common;
using Tawala.Projects.Documents;
using Tawala.Projects.Fields;
using Tawala.RtfSupport;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms
{
    [Serializable]
    public class FormItemField : ParagraphComponent
    {
        protected IPaletteField field = PaletteField.NULL;

        public IPaletteField Field { get { return field; } }

        public override string Text { get { return field.FieldString; } }

        public override string ToXml()
        {
            if (field is UnresolvedPaletteField)
            {
                field = resolveField(field);
            }

            return String.Format("<field name=\"{0}\"/>", XMLStringFormatter.EscapeAttributeText(field.QualifiedFieldName));
        }

        /// <summary>
        /// Converts an unresolved field to a valid field reference (if one exists).
        /// </summary>
        private static IPaletteField resolveField(IPaletteField unresolvedField)
        {
            string formName = FieldUtil.GetFormName(unresolvedField.QualifiedFieldName);
            string fieldName = FieldUtil.GetFieldName(unresolvedField.QualifiedFieldName);

            var resolvedField = (IPaletteField)Project.Current.GetForm(formName).ItemList[fieldName];

            return (resolvedField ?? unresolvedField);
        }

        public override string ToHtml()
        {
            return String.Format("&lt;&lt;{0}&gt;&gt;", field.QualifiedFieldName);
        }

        public override string ToRtf()
        {
            string rtfString = @"{{\*\txfieldstart\txfieldtype0\txfieldflags" + RtfUtility.NonEditableFieldFlags + @"\txfielddataval{0}" +
                               @"\txfielddata " + RtfUtility.EncodeHexString("TF$", false) + "{1}}}" + @"<<{2}>>{{" + @"\*\txfieldend}}";

            return String.Format(rtfString, field.Id, RtfUtility.EncodeHexString(field.QualifiedFieldName), field.QualifiedFieldName);
        }
    }

    [Serializable]
    public class FormItemIdedField : FormItemField
    {
        public FormItemIdedField(IXmlElement element)
        {
            string fullFieldName = element.GetAttribute("name");
            int id = Convert.ToInt32(element.GetAttribute("id"));

            field = FieldUtil.InitializeIdedField(fullFieldName, id);
        }
    }

    [Serializable]
    public class FormItemNamedField : FormItemField
    {
        public FormItemNamedField(IXmlElement element)
        {
            string fullFieldName = element.GetAttribute("name");
            field = FieldUtil.InitializeNamedField(fullFieldName);
        }
    }
}