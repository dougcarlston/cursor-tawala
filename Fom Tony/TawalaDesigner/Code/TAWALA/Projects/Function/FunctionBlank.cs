// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Diagnostics;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
    [Serializable]
    [TypeConverter(typeof(BindingConverter))]
    public class FunctionBlank : FunctionFieldWrapper
    {
        public FunctionBlank()
        {
            Debugger.Break();
        }

        public FunctionBlank(IPaletteField field)
        {
            Field = field;
        }

        /// <summary>
        /// Creates a FunctionBlank object from a &ltsimple-list-field&gt; XML element.
        /// </summary>
        public FunctionBlank(IXmlElement element)
        {
            Field = new UnresolvedPaletteField(element.Text);
        }

        protected override IPaletteField ResolveField(string qualifiedFieldName)
        {
            IPaletteField resolvedField = null;

            string formName = FieldUtil.GetFormName(qualifiedFieldName);
            string fieldName = FieldUtil.GetFieldName(qualifiedFieldName);

            IPaletteField blank = null;

            IForm form = Project.Current.AllForms[formName];

            if (form != null)
            {
                blank = (IPaletteField)Project.Current.AllForms[formName].ItemList[fieldName];
            }

            if (blank != null)
            {
                if (FieldUtil.IsRecordField(qualifiedFieldName))
                {
                    string recordName = FieldUtil.GetRecordName(qualifiedFieldName);
                    resolvedField = new RecordField(new Record(recordName), blank);
                }
                else
                {
                    resolvedField = blank;
                }
            }

            return resolvedField;
        }

        public override string ToString()
        {
            return Field.QualifiedFieldName;
        }

        #region Parse

        public static FunctionBlank Parse(object o)
        {
            var field = o as IPaletteField;

            if (field == null)
            {
                return null;
            }

            if (isBlank(field))
            {
                return new FunctionBlank(FieldUtil.RecordQualifyField(field));
            }

            if (field is RecordField)
            {
                var recordField = field as RecordField;
                if (isBlank(recordField.ReferenceField as IPaletteField))
                {
                    return new FunctionBlank(field);
                }
            }

            return null;
        }

        private static bool isBlank(object o)
        {
            return o is IHiddenField || o is IBlank || o is IFileUploadItem;
        }

        #endregion
    }
}