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
	public class NewTextItem : FormItem, ITextItem
	{
		public NewTextItem()
		{
			this.Style = ((Project.Current != null && Project.Current.GlobalTextItemStyle != null) ? Project.Current.GlobalTextItemStyle : "normal");

			string xmlString =
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with text of your own.]" +
				"</paragraph>";

			contents = FormItemContentsFactory.MakeObject(new XmlElement(xmlString, true));
		}

		public NewTextItem(IXmlElement element)
		{
			AlternateLabel = element.GetAttribute("alternateLabel");
			style = element.GetAttribute("style") ?? "normal";
			contents = FormItemContentsFactory.MakeChildren(element);

			getDisplayConditions(element);
		}

		public override bool IsTextItem
		{
			get { return true; }
		}

		private static readonly string xmlTextStartTag = "<text label=\"{0}\"";
		private static readonly string xmlTextEndTag = "</text>\r\n";

		public string ToXml(string label)
		{
			StringBuilder xmlString = new StringBuilder();

			xmlString.AppendFormat(xmlTextStartTag, label);

			if (hasAlternateLabel())
			{
				xmlString.AppendFormat(" alternateLabel=\"{0}\"", alternateLabel);
			}

			if (hasStyle())
			{
				xmlString.AppendFormat(" style=\"{0}\"", style);
			}

			xmlString.Append(">");

			xmlString.Append(contents.ToXml());

			xmlString.Append(displayConditionsToXml());

			xmlString.Append(xmlTextEndTag);

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

		private bool hasStyle()
		{
			return !string.IsNullOrEmpty(style);
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
}
