// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Diagnostics;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;

namespace Tawala.Projects
{
    [Serializable]
    [TypeConverter(typeof(BindingConverter))]
    public class FunctionContentsField : FunctionFieldWrapper
    {
        public FunctionContentsField()
        {
            Debugger.Break();
        }

        public FunctionContentsField(IPaletteField field)
        {
            Field = field;
        }

        protected override IPaletteField ResolveField(string qualifiedFieldName)
        {
            IPaletteField resolvedField = null;

            string formName = FieldUtil.GetFormName(qualifiedFieldName);
            string fieldName = FieldUtil.GetFieldName(qualifiedFieldName);

            var fieldLookup = (IPaletteField)Project.Current.AllForms[formName].ItemList[fieldName];

            if (fieldLookup != null)
            {
                if (FieldUtil.IsRegularOrExternalRecordField(qualifiedFieldName))
                {
                    string recordName = FieldUtil.GetRecordName(qualifiedFieldName);
                    resolvedField = new RecordField(new Record(recordName), fieldLookup);
                }
                else
                {
                    resolvedField = fieldLookup;
                }
            }

            return resolvedField;
        }

        public override string ToString()
        {
            return string.Format("<field name=\"{0}\" />", Field);
        }

        public static FunctionContentsField Parse(object o)
        {
            if (!AcceptedType(o))
            {
                return null;
            }

            var recordField = o as RecordField;

            if (recordField != null)
            {
                return new FunctionContentsField(recordField);
            }

            var field = o as IPaletteField;
            return new FunctionContentsField(field);
        }

        public static bool AcceptedType(object o)
        {
            if (isContentField(o))
            {
                return true;
            }

            var rf = o as RecordField;

            if (rf != null && isContentField(rf.ReferenceField))
            {
                return true;
            }

            return false;
        }

        private static bool isContentField(object o)
        {
            return o is IMcqItem || o is Blank || o is IHiddenField || o is IBlank || o is IFileUploadItem;
        }
    }
}