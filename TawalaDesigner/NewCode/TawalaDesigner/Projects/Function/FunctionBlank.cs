using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Functions.Runtime;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	[Serializable]
	public class FunctionBlank : FunctionFieldWrapper, IFunctionParameterXml
	{
		public FunctionBlank()
		{
		}

		public FunctionBlank(IPaletteField field)
		{
			this.field = field;
		}

		/// <summary>
		/// Creates a FunctionBlank object from a &ltsimple-list-field&gt; XML element.
		/// </summary>
		public FunctionBlank(IXmlElement element)
		{
			string fullFieldName = element.Text;

			string formName = FieldUtil.GetFormName(fullFieldName);
			string fieldName = FieldUtil.GetFieldName(fullFieldName);

			IPaletteField blank = null;

			IForm form = Project.Current.AllForms[formName];

			if (form != null)
			{
				blank = (IPaletteField)form.ItemList[fieldName];
			}

			if (blank == null)
			{
				this.field = new UnresolvedPaletteField(fullFieldName);
			}
			else
			{
				if (FieldUtil.IsRegularOrExternalRecordField(fullFieldName))
				{
					string recordName = FieldUtil.GetRecordName(fullFieldName);
					this.field = new RecordField(new Record(recordName), blank);
				}
				else
				{
					this.field = blank;
				}
			}

		}

		/// <summary>
		/// Converts an unresolved field to a valid field reference (if one exists).
		/// </summary>
		private IPaletteField resolveField(IPaletteField unresolvedField)
		{
			string formName = FieldUtil.GetFormName(unresolvedField.QualifiedFieldName);
			string fieldName = FieldUtil.GetFieldName(unresolvedField.QualifiedFieldName);

			IPaletteField blank = null;
			IPaletteField resolvedField = null;

			IForm form = Project.Current.AllForms[formName];

			if (form != null)
			{
				blank = (IPaletteField)Project.Current.AllForms[formName].ItemList[fieldName];
			}

			if (blank != null)
			{
				if (FieldUtil.IsRecordField(unresolvedField.QualifiedFieldName))
				{
					string recordName = FieldUtil.GetRecordName(unresolvedField.QualifiedFieldName);
					resolvedField = new RecordField(new Record(recordName), blank);
				}
				else
				{
					resolvedField = blank;
				}
			}

			return (resolvedField != null ? resolvedField : unresolvedField);
		}

		private IPaletteField field;

		public override IPaletteField Field
		{
			get { return field; }
			set { field = value; }
		}

		public override string ToString()
		{
			if (field is UnresolvedPaletteField)
			{
				field = resolveField(field);
			}

			return field.QualifiedFieldName;
		}

		#region Parse

		public static FunctionBlank Parse(object o)
		{
			FunctionBlank fb = o as FunctionBlank;
			IPaletteField pf = o as IPaletteField;

			if (fb != null)
				return fb;

			if (pf == null || !isBlank(o))
				return null;

			fb = new FunctionBlank();

			if (!FieldUtil.IsRegularOrExternalRecordField(pf.QualifiedFieldName))
			{
				fb.Field = FieldUtil.RecordQualifyField(pf);
			}
			else
			{
				fb.Field = pf;
			}

			return fb;
		}

		private static bool isBlank(object o)
		{
			return o is IHiddenField || o is IBlank;
		}

		private static bool isRecordBlank(object o)
		{
			RecordField rf = o as RecordField;
			if (rf == null) return false;

			return isBlank(rf.ReferenceField);
		}

		#endregion

        #region IFunctionParameterXml Members

        public string ToFunctionParameterXml()
        {
            return ToString();
        }

        #endregion
    }
}
