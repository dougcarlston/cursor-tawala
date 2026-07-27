using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Projects.Factories;
using Tawala.XmlSupport;
using Tawala.Projects.Forms.FormItemContents;

namespace Tawala.Projects.Forms.NewFormItems
{
	[Serializable]
	public class NewFibItem : FormItem, IFibItem
	{
		public NewFibItem()
		{
			this.style = ((Project.Current != null && Project.Current.GlobalFibItemStyle != null) ? Project.Current.GlobalFibItemStyle : "topLabels");

			string xmlString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with your question. Underscores create blanks.] " +
				"<blank label=\"a\" length=\"20\" required=\"false\"/>" +
				"</paragraph>";

			contents = FormItemContentsFactory.MakeObject(new XmlElement(xmlString, true));

			Project.FieldMapById.AddUnique((IField)this);
		}

		public NewFibItem(IXmlElement element)
		{
			AlternateLabel = element.GetAttribute("alternateLabel");
			style = element.GetAttribute("style") ?? "topLabels";
			contents = FormItemContentsFactory.MakeChildren(element);

			getDisplayConditions(element);

			Project.FieldMapById.AddUnique(this);
		}

		public NewFibItem(IExternalForm form, string alternateLabel) : base(form, string.Empty)
		{
            var blanks = new FormItemContentsCollection();
            blanks.Add(new ExternalBlank(this, alternateLabel));
            contents = blanks;
            Project.FieldMapById.AddUnique(this);
            InsertBlanksIntoFieldMapByName();
		}

		public void InsertBlanksIntoFieldMapByName()
		{
			foreach (IBlank blank in BlankList)
			{
				Project.FieldMapByName.AddUnique(blank);
			}
		}

		public override bool IsQuestionItem
		{
			get { return true; }
		}

		public override IFormItemContents Contents
		{
			set
			{
				contents = value;
				blankCacheIsValid = false;
			}
		}

		private static readonly string xmlFibStartTag = "<fib label=\"{0}\" style=\"{1}\">";
		private static readonly string xmlFibStartWithAlternateLabelTag = "<fib label=\"{0}\" alternateLabel=\"{1}\" style=\"{2}\">";
		private static readonly string xmlFibEndTag = "</fib>\r\n";

		public string ToXml(string label)
		{
			StringBuilder xmlString = new StringBuilder();

			if (hasAlternateLabel())
			{
				xmlString.AppendFormat(xmlFibStartWithAlternateLabelTag, label, alternateLabel, style);
			}
			else
			{
				xmlString.AppendFormat(xmlFibStartTag, label, style);
			}

			xmlString.Append(contents.ToXml());

			xmlString.Append(displayConditionsToXml());

			xmlString.Append(xmlFibEndTag);

			return xmlString.ToString();
		}

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

		#region IFibItem Members

		private Collection<IBlank> blankCache;
		private bool blankCacheIsValid = false;

		public Collection<IBlank> BlankList
		{
			get
			{
				if (!blankCacheIsValid)
				{
					refreshBlankCache();
				}

				return blankCache;
			}
		}

		private void refreshBlankCache()
		{
			blankCache = new Collection<IBlank>();
			FormItemContentsCollection descendants = contents.GetDescendants(typeof(IBlank));

			foreach (IFormItemContents item in descendants)
			{
				blankCache.Add(item as IBlank);
			}

			blankCacheIsValid = true;
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
				foreach (IField blank in BlankList)
				{
					yield return blank;
				}
			}
		}

		#endregion
	}
}
