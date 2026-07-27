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
	public class NewHeadingItem : FormItem, IHeadingItem
	{
		public NewHeadingItem()
		{
			string xmlString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with heading of your own.]" +
				"</paragraph>";

			contents = FormItemContentsFactory.MakeObject(new XmlElement(xmlString, true));
		}

		public NewHeadingItem(IXmlElement element)
		{
			AlternateLabel = element.GetAttribute("alternateLabel");
			HeadingType = (HeadingType)(Enum.Parse(typeof(HeadingType), element.GetAttribute("type")));

			contents = FormItemContentsFactory.MakeChildren(element);

			getDisplayConditions(element);
		}

		private static readonly string xmlHeadingStartTag = "<heading label=\"{0}\"";
		private static readonly string xmlHeadingEndTag = "</heading>\r\n";

		public string ToXml(string label)
		{
			StringBuilder xmlString = new StringBuilder();

			xmlString.AppendFormat(xmlHeadingStartTag, label, HeadingType);

			if (hasAlternateLabel())
			{
				xmlString.AppendFormat(" alternateLabel=\"{0}\"", alternateLabel);
			}

			xmlString.AppendFormat(" type=\"{0}\"", typeOfHeading);

			xmlString.Append(">");

			xmlString.Append(contents.ToXml());

			xmlString.Append(displayConditionsToXml());

			xmlString.Append(xmlHeadingEndTag);

			return xmlString.ToString();
		}

		private HeadingType typeOfHeading = HeadingType.Main;

		public HeadingType HeadingType
		{
			get { return typeOfHeading; }
			set { typeOfHeading = value; }
		}

		#region IFormField Members

		public override string FieldName
		{
			get
			{
				return (hasAlternateLabel() ? alternateLabel : defaultLabel);
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
			yield break;
		}

		#endregion

		#region IRecursiveEnumerable Interface

		public override IEnumerable RecursiveEnumerator
		{
			get
			{
				yield break;
			}
		}

		#endregion
	}
    public enum HeadingType { Main, Sub };
}
