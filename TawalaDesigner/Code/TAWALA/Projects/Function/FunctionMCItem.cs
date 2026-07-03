// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Diagnostics;
using Tawala.Functions.Runtime;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
    [Serializable]
    [TypeConverter(typeof(BindingConverter))]
    public class FunctionMCItem : FunctionFieldWrapper
    {
        public FunctionMCItem()
        {
            Debugger.Break();
        }

        public FunctionMCItem(IPaletteField field)
        {
            Field = field;
        }

        public FunctionMCItem(IXmlElement element)
        {
            Field = new UnresolvedPaletteField(element.Text);
        }

        public static FunctionMCItem Create(IXmlElement element)
        {
            if (string.IsNullOrEmpty(element.Text))
            {
                return null;
            }
            return new FunctionMCItem(element);
        }

        protected override IPaletteField ResolveField(string qualifiedFieldName)
        {
            IPaletteField resolvedField = null;

            string formName = FieldUtil.GetFormName(qualifiedFieldName);
            string fieldName = FieldUtil.GetFieldName(qualifiedFieldName);

            IPaletteField mcItem = null;

            IForm form = Project.Current.AllForms[formName];

            if (form != null)
            {
                mcItem = (IPaletteField)form.ItemList[fieldName];
            }

            if (mcItem != null)
            {
                if (FieldUtil.IsRegularOrExternalRecordField(qualifiedFieldName))
                {
                    string recordName = FieldUtil.GetRecordName(qualifiedFieldName);
                    resolvedField = new RecordField(new Record(recordName), mcItem);
                }
                else
                {
                    resolvedField = mcItem;
                }
            }

            return resolvedField;
        }

        public override string ToString()
        {
            return Field.QualifiedFieldName;
        }

        #region Parse

        public static FunctionMCItem Parse(object o, IParameterInfo parameterInfo)
        {
            var pf = o as IPaletteField;

            if (pf == null || !IsMCItem(pf))
            {
                return null;
            }

            if (FunctionUtil.HasParameterRestrictions(parameterInfo))
            {
                return new FunctionMCItem(FunctionUtil.ApplyParameterRestrictions(parameterInfo, pf));
            }
            if (!FieldUtil.IsRegularOrExternalRecordField(pf.QualifiedFieldName))
            {
                return new FunctionMCItem(FieldUtil.RecordQualifyField(pf));
            }

            return new FunctionMCItem(pf);
        }

        public static bool IsMCItem(IPaletteField pf)
        {
            if (pf is IMcqItem)
            {
                return true;
            }

            var rf = pf as RecordField;
            if (rf != null && rf.ReferenceField is IMcqItem)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}