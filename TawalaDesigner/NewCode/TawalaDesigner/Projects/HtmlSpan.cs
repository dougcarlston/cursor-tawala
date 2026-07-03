// $Workfile: HtmlSpan.cs $
// $Revision: 7 $	$Date: 6/04/07 2:04p $
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
	/// Implements an HTML span element.
	/// </summary>
	public class HtmlSpan : ParagraphComponent
	{
		public HtmlSpan(IXmlElement element)
		{
			string style = element.GetAttribute("style");

			this.fontFamily = getStringAttribute(style, @"font-family\s?:\s?'([^']+)'", "Arial");
			this.fontSize = getIntegerAttribute(style, @"font-size\s?:\s?(\d+)\s?pt", 12);
			this.text = element.Text;
			this.innerXml = element.InnerXml;
		}

		private string getStringAttribute(string attributeString, string pattern, string defaultString)
		{
			if (Regex.IsMatch(attributeString, pattern))
			{
				return (Regex.Match(attributeString, pattern).Groups[1].Value);
			}
			else
			{
				return defaultString;
			}
		}

		private int getIntegerAttribute(string attributeString, string pattern, int defaultInteger)
		{
			if (Regex.IsMatch(attributeString, pattern))
			{
				return (Convert.ToInt32(Regex.Match(attributeString, pattern).Groups[1].Value));
			}
			else
			{
				return defaultInteger;
			}
		}

		private const string xmlFontStartTag = "<font face=\"{0}\" size=\"{1}\">";
		private const string xmlFontEndTag = "</font>";

		public override string ToXml()
		{
			StringBuilder xmlString = new StringBuilder();

			xmlString.AppendFormat(xmlFontStartTag, fontFamily, fontSize);
			xmlString.Append(innerXml);
			xmlString.Append(xmlFontEndTag);

			return xmlString.ToString();
		}

		private const string htmlSpanStartTag = "<span style=\"font-family:'{0}'; font-size:{1}pt\">";
		private const string htmlSpanEndTag = "</span>";

		public override string ToHtml()
		{
			StringBuilder htmlString = new StringBuilder();

			htmlString.AppendFormat(htmlSpanStartTag, fontFamily, fontSize);
			htmlString.Append(innerXml);
			htmlString.Append(htmlSpanEndTag);

			return htmlString.ToString();
		}

		public override string ToRtf(RtfDocument document)
		{
			return String.Empty;
		}

		/// <summary>
		/// Text within span element
		/// </summary>
		private string text = "";

		public override string Text
		{
			get
			{
				return innerXml;
			}
		}

		/// <summary>
		/// Xml within span element
		/// </summary>
		private string innerXml = "";

		public string InnerXml
		{
			get
			{
				return innerXml;
			}
		}

		/// <summary>
		/// Font face name
		/// </summary>
		private string fontFamily = "";
		
		public string FontFamily
		{
			get
			{
				return fontFamily;
			}
		}

		/// <summary>
		/// Font size in points
		/// </summary>
		private int fontSize;

		public int FontSize
		{
			get
			{
				return fontSize;
			}
		}
	}
}
