// $Workfile: FunctionConditions.cs $
// $Revision: 22 $	$Date: 10/15/07 4:03p $
// Copyright © 2005 - 2007 Tawala Systems, Inc. All rights reserved.

using System;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Tawala.Functions.Runtime;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	[Serializable]
    [TypeConverter("Tawala.Functions.Controls.FunctionConditionsBinder+BindingConverter")]
	public class FunctionConditions : IFunctionParameterXml
	{
		private FunctionFormCollection forms = new FunctionFormCollection();
		private Conditions conditions;


		public FunctionConditions()
		{
		}

		public FunctionConditions(IXmlElement element)
		{
			// the outtermost node of the element is function-and-parameter specific;
			// we're only interested in the child elements
			if (element.HasChild("form"))
			{
				forms = new FunctionFormCollection(element.GetChildren("form"));
			}

			if (element.HasChild("conditions"))
			{
				IXmlElement conditionsElement = element.GetChild("conditions");

				conditions = new Conditions(conditionsElement, buildFieldResolver(conditionsElement));
			}
		}

		private FieldList buildFieldResolver(IXmlElement conditionsElement)
		{
			FieldList allFields = new FieldList();

			foreach (IForm form in Project.Current.AllForms)
			{
				allFields.Add(form.GetAllFields());
			}

			allFields.Add(Project.Current.AllVariables);

			allFields.Add(getAllRecordFields(conditionsElement, new Record(FieldUtil.DefaultRecordQualifierName)));

			return allFields;
		}

		private IField getAllRecordFields(IXmlElement element, Record record)
		{
			FieldList recordFields = new FieldList();

			foreach (IXmlElement childElement in element.GetChildren())
			{
				if (childElement.HasAttribute("field"))
				{
					addRecordField(recordFields, record, childElement);
				}

				recordFields.Add(getAllRecordFields(childElement, record));
			}

			return recordFields;
		}

		private void addRecordField(FieldList fieldList, Record record, IXmlElement childElement)
		{
			string fullFieldName = childElement.GetAttribute("field");

			if (FieldUtil.IsRecordField(fullFieldName) || FieldUtil.IsExternalRecordField(fullFieldName))
			{
				RecordField recordField = new RecordField(record, getFormField(fullFieldName));
				fieldList.AddUnique(recordField);
			}
		}

		// REVISIT: This method is duplicated in DocumentPersistedFunctionField. Can it be generalized somewhere?  - jdf 3/07
		private IPaletteField getFormField(string recordAndFormQualifiedName)
		{
			string formName = FieldUtil.GetFormName(recordAndFormQualifiedName);
			string qualifiedFieldName = formName + ":" + FieldUtil.GetFieldName(recordAndFormQualifiedName);

			IPaletteField field = Project.Current.AllForms[formName].GetAllFields()[qualifiedFieldName] as IPaletteField;
			if (field == null)
			{
				field = PaletteField.NULL;
			}

			return field;
		}

		public FunctionFormCollection Forms
		{
			get { return forms; }
			set { forms = value; }
		}

		public Conditions Conditions
		{
			get { return conditions; }
			set { conditions = value; }
		}

		public string ToXml()
		{
			StringBuilder xmlString = new StringBuilder();
			
			xmlString.AppendFormat(forms.ToXml());

			if (conditions != null)
			{
				xmlString.Append(conditions.ToXml());
			}

			return xmlString.ToString();
		}

		public override string ToString()
		{
			return conditions == null ? string.Empty : conditions.ToString();
		}
	
        #region IFunctionParameterXml Members

        public string ToFunctionParameterXml()
        {
            return ToXml();
        }

        #endregion
    }
}
