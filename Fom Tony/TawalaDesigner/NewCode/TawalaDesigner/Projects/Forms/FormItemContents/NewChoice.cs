// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Projects;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Factories;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms.FormItemContents
{
	[Serializable]
	public class NewChoice : FormItemContents, IChoice
	{
		protected string choiceLabel;

		protected NewChoice()
		{
		}

		public NewChoice(string choiceLabel, string choiceText)
		{
			Expression choiceExpression = new NonArithmeticExpression(choiceText);

			Contents = FormItemContentsFactory.MakeChildren(new XmlElement(string.Format(@"<choice label=""{0}"">{1}</choice>", choiceLabel, choiceExpression.ToXml())));

			this.choiceLabel = choiceLabel;
		}

		public override string ToXml()
		{
			return string.Format("<choice label=\"{0}\">{1}</choice>", choiceLabel, Contents.ToXml());
		}

		public override string ToXhtml(IFormItem formItem)
		{
			IMcqItem mcqItem = formItem as IMcqItem;
			string inputType = mcqItem.SelectOnlyOne ? "radio" : "checkbox";
			return string.Format("<t:Choice><span class=\"choice\">{0}</span><input type=\"{1}\" /><span>{2}</span><br /></t:Choice>", choiceLabel, inputType, Contents.ToXhtml(formItem));
		}

		#region IChoice Members

		public string ContentsXhtml(IFormItem formItem)
		{
			return Contents.ToXhtml(formItem);
		}

		private const string xmlChoiceStartTag = "<choice label=\"{0}\">";
		private const string xmlChoiceEndTag = "</choice>";

		public string ToXml(string label)
		{
			StringBuilder xmlString = new StringBuilder();

			xmlString.AppendFormat(xmlChoiceStartTag, label);
			xmlString.Append(Contents.ToXml());
			xmlString.Append(xmlChoiceEndTag);

			return xmlString.ToString();
		}

		public string ChoiceLabel
		{
			get { return choiceLabel; }
			set { choiceLabel = value; }
		}

		public new string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		#region IField Members

		public string FieldName
		{
			get { throw new NotImplementedException(); }
		}

		public string FieldString
		{
			get { throw new NotImplementedException(); }
		}

		public IField this[string name]
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IEnumerable Members

		public System.Collections.IEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IRecursiveEnumerable Members

		public System.Collections.IEnumerable RecursiveEnumerator
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IAnyField Members

		public int Id
		{
			get { throw new NotImplementedException(); }
		}

		public string QualifiedFieldName
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}

	[Serializable]
	public class NewXmlChoice : NewChoice
	{
		public NewXmlChoice(IXmlElement element)
		{
			setContentsWhileStrippingExtraParagraphElements(element);

			choiceLabel = element.GetAttribute("label");
		}

		private void setContentsWhileStrippingExtraParagraphElements(IXmlElement element)
		{
			if (element.HasDescendants("font"))
			{
				Contents = FormItemContentsFactory.MakeChildrenFromDescendants(element, "font");
			}
			else
			{
				//Contents = FormItemContentsFactory.MakeChildrenFromDescendants(element, "#text", "field");

				string elementXml = Regex.Replace(element.OuterXml, "<paragraph[^>]+>([^<]*)</paragraph>", "$1");
				Contents = FormItemContentsFactory.MakeChildren(new XmlElement(elementXml));
			}
		}

		public new string Text
		{
			get { return base.Text; }
		}
	}

	[Serializable]
	public class NewXhtmlChoice : NewChoice
	{
		public NewXhtmlChoice(IXmlElement element)
		{
			Contents = FormItemContentsFactory.MakeChildren(element);
			choiceLabel = element.GetChildren("span")[0].Text;
		}
	}
}
