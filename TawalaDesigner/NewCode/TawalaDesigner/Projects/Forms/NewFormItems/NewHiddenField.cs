using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Common;
using Tawala.Projects.Factories;
using Tawala.XmlSupport;
using Tawala.Projects.Forms.FormItemContents;

namespace Tawala.Projects.Forms.NewFormItems
{
	[Serializable]
	public class NewHiddenField : FormItem, IHiddenField, IDeserializedField, IAssignableField
	{
		public NewHiddenField()
		{
			Name = createDefaultName();
			Project.FieldMapById.AddUnique(this);
		}

		public NewHiddenField(IXmlElement element)
		{
			Name = element.GetAttribute("name");
			Project.FieldMapById.AddUnique(this);
		}

		public override string ToXml()
		{
			return string.Format("<field name=\"{0}\"/>" + Environment.NewLine, XMLStringFormatter.EscapeAttributeText(Name));
		}

		#region IHiddenField Members

		public string Name
		{
			get
			{
				return AlternateLabel;
			}
			set
			{
				AlternateLabel = value;
			}
		}

		#endregion

		public override string AlternateLabel
		{
			get
			{
				return base.AlternateLabel;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.AlternateLabel = createDefaultName();
				}
				else
				{
					base.AlternateLabel = value;
				}
			}
		}

		#region IPaletteField Interface

		public override string FieldName
		{
			get
			{
				return Name;
			}
		}

		public override string QualifiedFieldName
		{
			get
			{
				if (!Project.Current.AllForms.ContainsComponent(Form))
				{
					return FieldUtil.UnknownFieldName;
				}
				else
				{
					return Form.Name + ":" + Name;
				}
			}
		}

		public override IField this[string nameParam]
		{
			get
			{
				if (Name == nameParam)
				{
					return this;
				}

				return null;
			}
		}

		#endregion

		#region IEnumerable Interface

		public override IEnumerator GetEnumerator()
		{
			yield return this;
		}

		#endregion

		#region IRecursiveEnumerable Interface

		public override IEnumerable RecursiveEnumerator
		{
			get
			{
				yield return this;
			}
		}

		#endregion

		#region IOperatorDataSource

		public IList OperatorDataSource
		{
			get
			{
				return HybridOperator.List.DataSource;
			}
		}

		#endregion

		private static string createDefaultName()
		{
			Regex regex = new Regex("Field([0-9]+)");

			int num = 1;

			foreach (IForm form in Project.Current.FormList)
			{
				FieldList fieldList = form.GetAllFields();

				foreach (IField field in fieldList.RecursiveEnumerator)
				{
					Match match = regex.Match(field.FieldName);

					if (match.Success)
					{
						Group group = match.Groups[1];

						int value = Convert.ToInt32(group.Value);
						if (value >= num)
						{
							num = value + 1;
						}
					}
				}

			}
			return string.Format("Field{0}", num);
		}

		#region IDeserializedField Members

		public IDeserializedField DeserializedFieldReference
		{
			get
			{
				return (IDeserializedField)Project.FieldMapById[this.Id];
			}
		}

		#endregion

		public override string ToString()
		{
			return QualifiedFieldName;
		}

		#region IAssignableField Members

		public string AssignmentName
		{
			get { return QualifiedFieldName; }
		}

		#endregion
	}
}
