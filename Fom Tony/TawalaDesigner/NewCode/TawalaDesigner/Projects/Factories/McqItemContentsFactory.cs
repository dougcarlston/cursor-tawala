using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Common;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace Tawala.Projects.Factories
{
	public static class McqItemContentsFactory
	{
		private static readonly Factory<IFormItemContents> mcqItemContentsFactory = new Factory<IFormItemContents>();

		static McqItemContentsFactory()
		{
			mcqItemContentsFactory.Register("#text", typeof(ChoiceTextContents));
			mcqItemContentsFactory.Register("p", typeof(XhtmlChoice));
			mcqItemContentsFactory.Register("strong", typeof(BoldContents));
			mcqItemContentsFactory.Register("em", typeof(ItalicContents));
			mcqItemContentsFactory.Register("u", typeof(UnderlineContents));
			mcqItemContentsFactory.Register("field", typeof(FieldReference));
		}

		public static IFormItemContents MakeObject(IXmlElement element)
		{
			IFormItemContents formItemContents = mcqItemContentsFactory.MakeObject(element);
			return (formItemContents ?? FormItemContents.NULL);
		}

		private static FormItemContentsCollection MakeChildren(IXmlElement element)
		{
			FormItemContentsCollection contents = new FormItemContentsCollection();

			foreach (IXmlElement childElement in element.GetChildren())
			{
				contents.Add(McqItemContentsFactory.MakeObject(childElement));
			}

			return contents;
		}

		public static IFormItemContents MakeChildrenFromHtml(string html)
		{
			if (html != null)
			{
				string htmlStrippedOfEmptyParagraph = Regex.Replace(html, "<P>&nbsp;</P>", "");
				return MakeChildren(new XhtmlElement(htmlStrippedOfEmptyParagraph, true));
			}

			return null;
		}

		[Serializable]
		private class XhtmlChoice : NewChoice
		{
			public XhtmlChoice(IXmlElement element)
			{
				Contents = MakeChildren(element);
			}
		}

		private class ChoiceTextContents : TextContents
		{
			private Expression choiceExpression;

			public ChoiceTextContents(IXmlElement element)
			{
				choiceExpression = new NonArithmeticExpression(element.Value);
			}

			public override string Text
			{
				get
				{
					return choiceExpression.ToString();
				}
			}

			public override string ToXml()
			{
				return choiceExpression.ToXml();
			}

			public override string ToXhtml(Tawala.Projects.Forms.IFormItem formItem)
			{
				return XMLStringFormatter.EscapeElementText(Text);
			}
		}
	}
}
