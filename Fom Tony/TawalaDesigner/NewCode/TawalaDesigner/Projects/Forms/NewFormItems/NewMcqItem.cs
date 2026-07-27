using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Common;
using Tawala.Projects.Factories;
using Tawala.XmlSupport;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Functions.Runtime;

namespace Tawala.Projects.Forms.NewFormItems
{
	[Serializable]
	public class NewMcqItem : FormItem, IMcqItem, IDeserializedField, IAssignableField
	{
		public static int StaticChoices = 0;
		public static int DynamicChoices = 1;

		private int columnCount;
		private bool selectOnlyOne = true;
		private bool requireAtLeastOne;
		private int choiceSourceType;

		public NewMcqItem()
		{
			this.style = ((Project.Current != null && Project.Current.GlobalFibItemStyle != null) ? Project.Current.GlobalMCItemStyle : "vertical");

			string xmlString =
				@"<contents>" +
				@"<question>" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">[Replace this with your question. Use Enter key to add choices below.]</font>" +
				@"</paragraph>" +
				@"</question>" +
				@"<choice label=""a""></choice>" +
				@"</contents>";

			IXmlElement element = new XmlElement(xmlString, true);

			setContents(element);

			Project.FieldMapById.AddUnique(this);
		}

        public NewMcqItem(IExternalForm form, string alternateLabel, int choiceCount, bool onlyOne)
			: base(form, alternateLabel)
		{
			selectOnlyOne = onlyOne;

            var newChoiceList = new NewChoiceList();
			for (int i = 0; i < choiceCount; ++i)
			{
                newChoiceList.Add(new NewChoice(new AlphaLabel(i).ToString(), string.Empty));
			}
		}

		public NewMcqItem(IXmlElement element)
		{
			AlternateLabel = element.GetAttribute("alternateLabel");
			requireAtLeastOne = element.GetAttribute("required") == "true";
			selectOnlyOne = (element.GetAttribute("onlyone") == "true");
			style = element.GetAttribute("style") ?? "vertical";

			if (element.HasAttribute("columnCount"))
			{
				columnCount = Convert.ToInt32(element.GetAttribute("columnCount"));
			}

			setContents(element);

			getDisplayConditions(element);
			
			Project.FieldMapById.AddUnique(this);
		}

		private void setContents(IXmlElement element)
		{
			contents = FormItemContentsFactory.MakeChildren(element);
			
			choiceSourceType = (element.HasChild("data-provider") ? DynamicChoices : StaticChoices);
		}

		public override bool IsQuestionItem
		{
			get { return true; }
		}

		private static readonly string xmlMCItemStartTag = "<mc label=\"{0}\"{1} onlyone=\"{2}\" required=\"{3}\">";
		private static readonly string xmlMCItemStartTagWithStyle = "<mc label=\"{0}\"{1} onlyone=\"{2}\" required=\"{3}\" style=\"{4}\">";
		private static readonly string xmlMCItemStartTagWithColumnCount = "<mc label=\"{0}\"{1} onlyone=\"{2}\" required=\"{3}\" style=\"{4}\" columnCount=\"{5}\">";
		private static readonly string xmlMCItemEndTag = "</mc>\r\n";

		public string ToXml(string label)
		{
			StringBuilder xmlString = new StringBuilder();

			if (String.IsNullOrEmpty(Style))
			{
				xmlString.AppendFormat(xmlMCItemStartTag, label, GetAlternateLabelXml(), selectOnlyOne == true ? "true" : "false", requireAtLeastOne == true ? "true" : "false");
			}
			else
			{
				if (columnCount == 0)
				{
					xmlString.AppendFormat(xmlMCItemStartTagWithStyle, label, GetAlternateLabelXml(), selectOnlyOne == true ? "true" : "false", requireAtLeastOne == true ? "true" : "false", Style);
				}
				else
				{
					xmlString.AppendFormat(xmlMCItemStartTagWithColumnCount, label, GetAlternateLabelXml(), selectOnlyOne == true ? "true" : "false", requireAtLeastOne == true ? "true" : "false", Style, columnCount);
				}
			}

			xmlString.Append(contents.ToXml());

			xmlString.Append(displayConditionsToXml());

			xmlString.Append(xmlMCItemEndTag);

			return xmlString.ToString();
		}

		public override string ToString()
		{
			return QualifiedFieldName;
		}


		#region IMcqItem Members

		/// <summary>
		/// If true, use radio buttons. If false, use check boxes.
		/// </summary>
		public bool SelectOnlyOne
		{
			get { return selectOnlyOne; }
			set { selectOnlyOne = value; }
		}

		/// <summary>
		/// If true, the user must select at least one choice
		/// </summary>
		public bool RequireAtLeastOne
		{
			get { return requireAtLeastOne; }
			set { requireAtLeastOne = value; }
		}

		public int ColumnCount
		{
			get { return columnCount; }
			set { columnCount = value; }
		}

		public int ChoiceSourceType
		{
			get { return choiceSourceType; }
			set { choiceSourceType = value; }
		}

		public Question Question
		{
			get
			{
				FormItemContentsCollection questionDescendants = contents.GetDescendants(typeof(Question));

				return (questionDescendants.Count == 1 ? questionDescendants[0] as Question : null);
			}
		}

		public IFormItemContents QuestionContents
		{
			get
			{
                FormItemContentsCollection questionDescendants = contents.GetDescendants(typeof(Question));

				return (questionDescendants.Count == 1 ? questionDescendants[0] : null);
			}
		}

		public IChoiceList Choices
		{
			get { return getChoices(); }
		}

		public IFormItemContents ChoiceContents
		{
			set
			{
				FormItemContentsCollection mcqContents = new FormItemContentsCollection();

				mcqContents.Add(this.QuestionContents);
				mcqContents.Add(value);

				Contents = mcqContents;
			}
		}

		public IFunction DataSourceFunction
		{
			get
			{
				DataProvider dataProvider = getDataProvider();

				return (dataProvider != null ? dataProvider.Function : null);
			}
		}

		public string ChoicesXhtml
		{
			get
			{
				return (choiceSourceType == DynamicChoices ? getDataProvider().ToXhtml(this) : getChoices().ToXhtml(this));
			}
		}

		private NewChoiceList getChoices()
		{
            FormItemContentsCollection choiceDescendants = contents.GetDescendants(typeof(IChoice));

			NewChoiceList choices = new NewChoiceList();

			foreach (IChoice choice in choiceDescendants)
			{
				choices.Add(choice);
			}

			return choices;
		}

		private DataProvider getDataProvider()
		{
            FormItemContentsCollection dataProviderDescendants = contents.GetDescendants(typeof(DataProvider));

			if (dataProviderDescendants.Count == 1)
			{
				return dataProviderDescendants[0] as DataProvider;
			}

			return null;
		}

		#endregion

		#region IFormField Members

		public override string FieldName
		{
			get
			{
				return (hasAlternateLabel() ? alternateLabel : defaultLabel);
			}
		}

		public override string QualifiedFieldName
		{
			get
			{
				IForm form;

                if ((form = GetContainingForm()) == Tawala.Projects.NullObjects.Form)
				{
					return FieldUtil.UnknownFieldName;
				}
				else
				{
					return form.Name + ":" + FieldName;
				}
			}
		}

		private bool hasAlternateLabel()
		{
			return !string.IsNullOrEmpty(alternateLabel);
		}

		private string defaultLabel
		{
			get
			{
				return Project.Current.GetDefaultLabel(this);
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
				return (selectOnlyOne ? MCOneOperator.List.DataSource : MCManyOperator.List.DataSource);
			}
		}

		#endregion

		#region IDeserializedField Members

		public IDeserializedField DeserializedFieldReference
		{
			get { return Project.FieldMapById[id] as IDeserializedField; }
		}

		#endregion

		#region IAssignableField Members

		public string AssignmentName
		{
			get { return QualifiedFieldName; }
		}

		#endregion
	}
}
