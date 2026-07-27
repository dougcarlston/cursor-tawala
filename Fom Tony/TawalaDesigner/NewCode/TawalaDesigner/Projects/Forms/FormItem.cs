// $Workfile: FormItem.cs $
// $Revision: 41 $	$Date: 10/26/07 7:03p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.ObjectModel;
using Tawala.Common;
using Tawala.XmlSupport;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;

namespace Tawala.Projects
{
	/// <summary>
	/// Summary description for FormItem.
	/// </summary>
	[Serializable]
	public class FormItem : IPaletteField, IFormItem
	{
		public static readonly FormItem NULL = new FormItem();

		private bool selected;

		public bool Selected
		{
			get { return selected; }
			set { selected = value; }
		}

		public FormItem()
		{
		}

		protected FormItem(IExternalForm form, string altLabel)
		{
			this.alternateLabel = altLabel;
			this.form = form;
			Project.FieldMapById.AddUnique(this);
		}

		protected int id = Project.NextUniqueID;

		/// <summary>
		/// Establish a valid ID for this form item upon deserialization.
		/// </summary>
		/// <param name="context"></param>
		[OnDeserialized]
		internal void onDeserialized(StreamingContext context)
		{
			// if item has no id (because it was copied, for example)
			if (id == 0)
			{
				// assign a new, unique id
				id = Project.NextUniqueID;
				Project.FieldMapById.AddUnique(this);
			}
		}

		public virtual IFormItemContents Contents
		{
			get { return contents; }
			set { contents = value; }
		}

		protected IFormItemContents contents;

		protected string text;

		public virtual string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
			}
		}


		static private string attributeFormat = " alternateLabel=\"{0}\"";

		public string GetAlternateLabelXml()
		{
			if (alternateLabel.Length == 0)
			{
				return string.Empty;
			}
			else
			{
				return string.Format(attributeFormat, XMLStringFormatter.EscapeAttributeText(alternateLabel));
			}
		}

		protected string displayConditionsToXml()
		{
			return DisplayConditions == null ? "" : DisplayConditions.ToXml("displayConditions");
		}

		#region JDFREVISIT

		protected void getDisplayConditions(IXmlElement element)
		{
			if (element.HasChild("displayConditions"))
			{
				IXmlElement displayConditionsElement = getDisplayConditionsElementWithoutWhitespace(element);
				DisplayConditions = new Conditions(displayConditionsElement, buildFieldResolver(displayConditionsElement));
			}
		}

		private static IXmlElement getDisplayConditionsElementWithoutWhitespace(IXmlElement element)
		{
			// if the conditions have whitespace in them, it will wreak havoc elsewhere in the code
			IXmlElement rawDisplayConditionsElement = element.GetChild("displayConditions");
			return new XmlElement(rawDisplayConditionsElement.OuterXml, false);
		}

		// REVISIT: This whole set of methods is duplicated in FunctionConditions. OK for Tawala Classic, but let's address it in the new UI. - jdf 09/098
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

		// REVISIT: This method is duplicated in DocumentPersistedFunctionField. Can it be generalized somewhere?  - jdf 9/08
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

		#endregion

		/// <summary>
		/// Called when the object is deserialized but before the graph is returned.
		/// </summary>
		[OnDeserializing]
		private void onDeserializing(StreamingContext context)
		{
			// we never want this to be null, just empty at worst.
			if (alternateLabel == null)
			{
				alternateLabel = string.Empty;
			}
		}

		public int Id
		{
			get { return id; }
		}

		protected IForm GetContainingForm()
		{
			return Project.Current.GetFormContaining(this);
		}

		/// <summary>
		/// Regex.Replace callback function to escape special characters in text.
		/// </summary>
		protected string EscapeSpecialCharacters(Match m)
		{
			if (m.Value.StartsWith("<<"))
			{
				return "<field name=\"" + XMLStringFormatter.EscapeAttributeText(m.Value.Substring(2, m.Value.Length - 4)) + "\"/>";
			}
			else if (m.Value == "&")
			{
				return "&amp;";
			}
			else if (m.Value == "<")
			{
				return "&lt;";
			}
			else if (m.Value == ">")
			{
				return "&gt;";
			}

			return string.Empty;
		}

		#region IField Interface

		public virtual string FieldName
		{
			get
			{
				return "";
			}
		}

		public virtual string QualifiedFieldName
		{
			get
			{
				return "";
			}
		}

		public virtual string FieldString
		{
			get
			{
				return "<<" + QualifiedFieldName + ">>";
			}
		}

		public virtual IField this[string name]
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

		public virtual IEnumerator GetEnumerator()
		{
			yield return this;
		}

		#endregion

		#region IRecursiveEnumerable Interface

		public virtual IEnumerable RecursiveEnumerator
		{
			get
			{
				yield return this;
			}
		}

		#endregion

		#region IFormItem Members

		public virtual IForm Form
		{
			get { return form; }

			set
			{
				form = value;

				if (form != null)
				{
					Project.FieldMapByName.AddUnique(this);
				}
			}
		}

		protected IForm form;

		public virtual bool IsTextItem
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsQuestionItem
		{
			get
			{
				return false;
			}
		}

		protected string alternateLabel = string.Empty;

		public virtual string AlternateLabel
		{
			get
			{
				return alternateLabel;
			}
			set
			{
				if (value == null)
				{
					Project.FieldMapByName.Remove(this);
					alternateLabel = string.Empty;
					Project.FieldMapByName.AddUnique(this);
				}
				else
				{
					if (alternateLabel != value.Trim())
					{
						Project.FieldMapByName.Remove(this);

						alternateLabel = value.Trim();
						Project.FieldMapByName.AddUnique(this);

						Project.Events.RaiseFormItemChangedEvent(new FormItemEventArgs(null, this, -1));
					}
				}
			}
		}

		/// <summary>
		/// Form Item Style String
		/// </summary>
		public virtual string Style
		{
			get { return style; }
			set { style = value; }
		}

		protected string style;


		private Conditions displayConditions = new Conditions();

		public Conditions DisplayConditions
		{
			get { return displayConditions; }
			set { displayConditions = (value ?? new Conditions()); }
		}

		public bool HasDisplayConditions
		{
			get { return DisplayConditions.Count > 0; }
		}

		public void ClearId()
		{
			id = 0;
		}

		public virtual void Eliminate()
		{
			Project.FieldMapById.Remove(id);
			Project.FieldMapByName.Remove(this);
		}

		public virtual string ToXml()
		{
			Debug.Assert(false);
			return null;
		}

		public void ResolveFieldReferences()
		{
            if (contents != null)
			{
                contents.ResolveFieldReferences();
			}
		}

		public void ResolveFunctionReferences()
		{
            if (contents != null)
			{
                contents.ResolveFunctionReferences();
			}
		}

		#endregion
	}
}
