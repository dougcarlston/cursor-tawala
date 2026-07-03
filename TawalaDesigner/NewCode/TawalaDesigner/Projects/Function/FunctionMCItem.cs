using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Functions.Runtime;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	[Serializable]
	public class FunctionMCItem : FunctionFieldWrapper, IFunctionParameterXml
	{
		public FunctionMCItem()
		{
		}

		public FunctionMCItem(IPaletteField field)
		{
			this.field = field;
		}

		/// <summary>
		/// Creates a FunctionMCItem object from a &ltfield&gt; XML element.
		/// </summary>
		public FunctionMCItem(IXmlElement element)
		{
			string fullFieldName = element.Text;

			string formName = FieldUtil.GetFormName(fullFieldName);
			string fieldName = FieldUtil.GetFieldName(fullFieldName);

			IPaletteField mcItem = null;

			IForm form = Project.Current.AllForms[formName];

			if (form != null)
			{
				mcItem = (IPaletteField)form.ItemList[fieldName];
			}

			if (mcItem == null)
			{
				this.field = new UnresolvedPaletteField(fullFieldName);
			}
			else
			{
				if (FieldUtil.IsRegularOrExternalRecordField(fullFieldName))
				{
					string recordName = FieldUtil.GetRecordName(fullFieldName);
					this.field = new RecordField(new Record(recordName), mcItem);
				}
				else
				{
					this.field = mcItem;
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

			IPaletteField mcItem = null;
			IPaletteField resolvedField = null;

			IForm form = Project.Current.AllForms[formName];

			if (form != null)
			{
				mcItem = (IPaletteField)form.ItemList[fieldName];
			}

			if (mcItem != null)
			{
				if (FieldUtil.IsRegularOrExternalRecordField(unresolvedField.QualifiedFieldName))
				{
					string recordName = FieldUtil.GetRecordName(unresolvedField.QualifiedFieldName);
					resolvedField = new RecordField(new Record(recordName), mcItem);
				}
				else
				{
					resolvedField = mcItem;
				}
			}

			return (resolvedField ?? unresolvedField);
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

		public static FunctionMCItem Parse(object o, IParameterInfo parameterInfo)
		{
			FunctionMCItem fm = o as FunctionMCItem;
			IPaletteField pf = o as IPaletteField;

			if (fm != null) 
				return fm;

			if (pf == null || !IsMCItem(o))
				return null;

			fm = new FunctionMCItem();

			if (FunctionUtil.HasParameterRestrictions(parameterInfo))
			{
				fm.Field = FunctionUtil.ApplyParameterRestrictions(parameterInfo, pf);
			}
			else
			{
				if (!FieldUtil.IsRegularOrExternalRecordField(pf.QualifiedFieldName))
				{
					fm.Field = FieldUtil.RecordQualifyField(pf);
				}
				else
				{
					fm.Field = pf;
				}
			}

			return fm;
		}

		public static bool IsMCItem(object o)
		{
			if (o is IMcqItem)
				return true;

			RecordField rf = o as RecordField;
			if (rf != null && rf.ReferenceField is IMcqItem)
				return true;

			return false;
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
