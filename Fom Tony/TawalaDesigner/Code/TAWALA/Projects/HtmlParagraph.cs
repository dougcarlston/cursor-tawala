// $Workfile: HtmlParagraph.cs $
// $Revision: 5 $	$Date: 7/06/06 11:02a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
using System;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.XmlSupport;

namespace Tawala.Proj
{
	/// <summary>
	/// Implements an HTML paragraph element.
	/// </summary>
	public class HtmlParagraph : Paragraph
	{
		private static readonly int TWIPS_PER_POINT = 20;

		private IXmlElement element;

		private static Factory<IParagraphComponent> componentFactory = new Factory<IParagraphComponent>();

		static HtmlParagraph()
		{
			componentFactory.Register("span", typeof(HtmlSpan));
			componentFactory.Register("#text", typeof(HtmlText));
			componentFactory.Register("b", typeof(HtmlText));
			componentFactory.Register("i", typeof(HtmlText));
			componentFactory.Register("u", typeof(HtmlText));
		}

		public HtmlParagraph(string htmlString) : this(new Tawala.XmlSupport.XmlElement(htmlString))
		{
		}

		public HtmlParagraph(IXmlElement element)
		{
			this.element = element;

			if (element.HasAttribute("style"))
			{
				string style = element.GetAttribute("style");

				this.marginTop = getIntegerAttribute(style, @"margin-top\s?:\s?(\d+)\s?pt", 0);
				this.marginBottom = getIntegerAttribute(style, @"margin-bottom\s?:\s?(\d+)\s?pt", 0);
				this.marginLeft = getIntegerAttribute(style, @"margin-left\s?:\s?(\d+)\s?pt", 0);
			}

			this.align = element.GetAttribute("align", defaultAlign);
			this.indent = marginLeft * TWIPS_PER_POINT;

			Collection<Tawala.XmlSupport.XmlElement> childElements = element.GetChildren();

			foreach (Tawala.XmlSupport.XmlElement childElement in childElements)
			{
				this.contents.Add(componentFactory.MakeObject(childElement));
			}
		}

		private string getStringAttribute(string attributeName, string pattern, string defaultString)
		{
			if (Regex.IsMatch(attributeName, pattern))
			{
				return (Regex.Match(attributeName, pattern).Groups[1].Value);
			}
			else
			{
				return defaultString;
			}
		}

		private int getIntegerAttribute(string attributeName, string pattern, int defaultInteger)
		{
			if (Regex.IsMatch(attributeName, pattern))
			{
				return (Convert.ToInt32(Regex.Match(attributeName, pattern).Groups[1].Value));
			}
			else
			{
				return defaultInteger;
			}
		}


		private string tableStartTag()
		{
			return "<table width=\"700\">";
		}

		private string tdStartTag()
		{
			return "<td width=\"" + 100 / ColumnCount + "%\">";
		}

		private string toTable()
		{
			StringBuilder tableString = new StringBuilder() ;

			tableString.Append(tableStartTag() + "<tr>" + tdStartTag());
			tableString.Append(Regex.Replace(Text, "\t", "</td>" + tdStartTag()));
			tableString.Append("</td>" + "</tr>" + "</table>");

			return tableString.ToString();
		}

		public override string ToHtml()
		{
			return (Text.Contains("\t") ? toTable() : element.OuterXml);
		}

		public int ColumnCount
		{
			get
			{
				return Regex.Matches(Text, "\t").Count + 1;
			}
		}

		public override string Text
		{
			get
			{
				StringBuilder text = new StringBuilder();

				foreach (IParagraphComponent component in contents)
				{
					text.Append(component.Text);
				}

				return text.ToString();
			}
		}

		/// <summary>
		/// Top margin in points
		/// </summary>
		private int marginTop;

		public int MarginTop
		{
			get
			{
				return marginTop;
			}
		}

		/// <summary>
		/// Bottom margin in points
		/// </summary>
		private int marginBottom;

		public int MarginBottom
		{
			get
			{
				return marginBottom;
			}
		}

		/// <summary>
		/// Left margin in points
		/// </summary>
		private int marginLeft;

		public int MarginLeft
		{
			get
			{
				return marginLeft;
			}
		}

	}
}
