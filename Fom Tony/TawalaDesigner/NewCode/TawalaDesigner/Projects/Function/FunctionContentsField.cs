using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Tawala.Functions.Runtime;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	[Serializable]
    public class FunctionContentsField : FunctionFieldWrapper, IFunctionParameterXml
	{
		public FunctionContentsField()
		{
		}

		public FunctionContentsField(IPaletteField field)
		{
			this.field = field;
		}

		private IPaletteField field;

		public override IPaletteField Field
		{
			get
			{
				if (field is UnresolvedPaletteField)
				{
					field = resolveField(field);
				}

				return field;
			}

			set { field = value; }
		}

		/// <summary>
		/// Converts an unresolved field to a valid field reference (if one exists).
		/// </summary>
		private IPaletteField resolveField(IPaletteField unresolvedField)
		{
			string formName = FieldUtil.GetFormName(unresolvedField.QualifiedFieldName);
			string fieldName = FieldUtil.GetFieldName(unresolvedField.QualifiedFieldName);

			IPaletteField field = (IPaletteField)Project.Current.AllForms[formName].ItemList[fieldName];
			IPaletteField resolvedField = null;

			if (field != null)
			{
				if (FieldUtil.IsRegularOrExternalRecordField(unresolvedField.QualifiedFieldName))
				{
					string recordName = FieldUtil.GetRecordName(unresolvedField.QualifiedFieldName);
					resolvedField = new RecordField(new Record(recordName), field);
				}
				else
				{
					resolvedField = field;
				}
			}

			return (resolvedField != null ? resolvedField : unresolvedField);
		}

		public override string ToString()
		{
			return string.Format("<field name=\"{0}\" />", Field);
		}

		public static FunctionContentsField Parse(object o)
		{
			RecordField recordField = o as RecordField;

			if (recordField != null && (recordField.ReferenceField is IHiddenField))
			{
				return new FunctionContentsField(recordField);
			}

			IHiddenField hiddenField = o as IHiddenField;

			if (hiddenField != null)
				return new FunctionContentsField(FieldUtil.RecordQualifyField(hiddenField));

			FunctionMCItem mc = FunctionMCItem.Parse(o,null);
			if (mc != null && mc.Field != null)
				return new FunctionContentsField(mc.Field);

			FunctionBlank fb = FunctionBlank.Parse(o);

			if (fb != null && fb.Field != null)
				return new FunctionContentsField(fb.Field);

			return null;
		}

		public static bool AcceptedType(object o)
		{
			if (isContentField(o))
				return true;

			RecordField rf = o as RecordField;

			if (rf != null && isContentField(rf.ReferenceField))
				return true;

			return false;
		}

		private static bool isContentField(object o)
		{
			return o is IMcqItem || o is IHiddenField || o is IBlank;
		}

        #region IFunctionParameterXml Members

        public string ToFunctionParameterXml()
        {
            return ToString();
        }

        #endregion
    }
}
