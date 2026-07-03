// Copyright © 2005 - 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Tawala.XmlSupport
{
	public static class XmlUtility
	{
		public static string ConvertEntitiesToText(string xmlString)
		{
			StringBuilder textString = new StringBuilder(xmlString);

			textString.Replace("&lt;", "<");
			textString.Replace("&gt;", ">");
			textString.Replace("&amp;", "&");
			textString.Replace("&quot;", "\"");
			textString.Replace("&apos;", "'");

			return textString.ToString();
		}

		public static string ToXhtml(string htmlString)
		{
			string xhtmlString = htmlString.Replace("\r\n","");

			xhtmlString = removeImportDirectives(xhtmlString);
			xhtmlString = removeNamespaceQualifiers(xhtmlString);
			xhtmlString = lowerCaseElementNames(xhtmlString);
			xhtmlString = closeInputTags(xhtmlString);
			xhtmlString = closeBreakTags(xhtmlString);
			xhtmlString = closeImageTags(xhtmlString);
			xhtmlString = quoteAttributes(xhtmlString);
			xhtmlString = replaceNonBreakingSpaces(xhtmlString);
			xhtmlString = new ParagraphNormalizer().CloseAllParagraphTags(xhtmlString);
			xhtmlString = SpanNormalizer.FixUnbalancedSpanTags(xhtmlString);

			return xhtmlString;
		}

		private static string removeImportDirectives(string htmlString)
		{
			return Regex.Replace(htmlString, @"<\?import[^>]*>", "");
		}

		private static string removeNamespaceQualifiers(string htmlString)
		{
			return Regex.Replace(htmlString, @"(</?)[Tt]:", "$1");
		}

		private static string lowerCaseElementNames(string htmlString)
		{
			return Regex.Replace(htmlString, @"(</?[A-Za-z]+)", toLower);
		}

		private static string toLower(Match m)
		{
			return (m.Groups[1].ToString().ToLower());
		}

		private static string closeInputTags(string htmlString)
		{
			return Regex.Replace(htmlString, @"(<input[^>]*)>", "$1 />");
		}

		private static string closeBreakTags(string htmlString)
		{
			return Regex.Replace(htmlString, @"(<br[^>]*)>", "$1 />");
		}

		private static string closeImageTags(string htmlString)
		{
			return Regex.Replace(htmlString, @"(<img[^>]*)>", "$1 />");
		}

		private static string quoteAttributes(string htmlString)
		{
			return Regex.Replace(htmlString, @"=([^"" >]*)([ >])", @"=""$1""$2");
		}

		private static string replaceNonBreakingSpaces(string htmlString)
		{
			return Regex.Replace(htmlString, @"&nbsp;", " ");
		}

		private class ParagraphNormalizer
		{
			public string CloseAllParagraphTags(string htmlString)
			{
				htmlString = Regex.Replace(htmlString, "<p [^>]*>|<p>|</p>", matchEvaluator);

				if (open > 0)
				{
					if (firstParagraphIndex == 0)
					{
						htmlString += "</p>";
					}
					else
					{
						htmlString = htmlString.Insert(htmlString.Length -1 - firstParagraphIndex, "</p>");
					}
				}

				return htmlString;
			}

			private string matchEvaluator(Match m)
			{
				if (m.Value.StartsWith("<p"))
				{
					if (firstParagraphIndex == -1)
					{
						firstParagraphIndex = m.Index;
					}

					++open;

					if (open > 1)
					{
						open--;
						return "</p>" + m.Value;
					}
				}
				else
				{
					open--;
				}

				return m.Value;
			}

			private int open = 0;
			private int firstParagraphIndex = -1;
		}

		private static string closeOpenParagraphTags(string htmlString)
		{
			int indexFirstOpen = htmlString.IndexOf("<p");
			int indexOpen = indexFirstOpen;
			int indexNextOpen = htmlString.IndexOf("<p", indexOpen + 1);

			while (indexNextOpen != -1)
			{
				int indexClose = htmlString.IndexOf("</p>", indexOpen+1);

				if (indexNextOpen > indexOpen && indexClose > indexOpen && indexClose < indexNextOpen)
				{
					indexOpen = indexNextOpen;
					continue;
				}

				if (indexNextOpen > indexOpen && indexClose > indexNextOpen)
				{
					htmlString = htmlString.Insert(indexNextOpen, "</p>");
					indexOpen = htmlString.IndexOf("<p", indexNextOpen);
				}

				if (indexNextOpen == -1)
				{
					if (indexClose == -1)
					{
						htmlString = htmlString.Insert(htmlString.Length - indexFirstOpen, "</p>");
					}
					break;
				}
			}
			return htmlString;
		}

		private static class SpanNormalizer
		{
			private static Regex regexParagraphs = new Regex("<p.*?</p>", RegexOptions.Compiled);
			private static Regex regexSpanOpen = new Regex("<span", RegexOptions.Compiled);
			private static Regex regexSpanClose = new Regex("</span>", RegexOptions.Compiled);
			private static Regex regexSpanTags = new Regex("</span>|<span", RegexOptions.Compiled);

			public static string FixUnbalancedSpanTags(string htmlString)
			{
				htmlString = fixExcessSpanOpenTags(htmlString);
				htmlString = fixExcessSpanCloseTags(htmlString);

				return htmlString;
			}

			private static string fixExcessSpanOpenTags(string htmlString)
			{
				var paragraphs = regexParagraphs.Matches(htmlString);

				foreach (Match p in paragraphs)
				{
					if (regexSpanOpen.IsMatch(p.Value))
					{
						var openingSpanTags = regexSpanOpen.Matches(p.Value);
						var closingSpanTags = regexSpanClose.Matches(p.Value);

						if (openingSpanTags.Count > closingSpanTags.Count)
						{
							for (int i = 0; i < openingSpanTags.Count - closingSpanTags.Count; i++)
							{
								int insertionPoint = p.Value.IndexOf("</p") + p.Index;
								htmlString = htmlString.Insert(insertionPoint, "</span>");
							}
						}
					}
				}

				return htmlString;
			}

			private static string fixExcessSpanCloseTags(string htmlString)
			{
				var sb = new StringBuilder(htmlString);

				while (removeExcessSpanCloseInParagraphs(sb)) { }

				return sb.ToString();
			}

			private static bool removeExcessSpanCloseInParagraphs(StringBuilder sb)
			{
				var paragraphs = regexParagraphs.Matches(sb.ToString());

				foreach (Match p in paragraphs)
				{
					var allSpanTags = regexSpanTags.Matches(p.Value);
					int open = 0;

					foreach (Match tag in allSpanTags)
					{
						if (tag.Value == "<span")
						{
							open++;
						}
						else if (open == 0)
						{
							sb.Remove(p.Index + tag.Index, 7);
							return true;
						}
						else
						{
							open--;
						}
					}
				}

				return false;
			}
		}
	}
}
