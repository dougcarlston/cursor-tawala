using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Common;

namespace Tawala.RtfSupport
{
	public class RtfFibParser : RtfParser
	{
		public RtfFibParser(string rtfString) : base(rtfString)
		{
		}

		protected override RtfToken getToken(string tokenString)
		{
			if (containsUnderscores(tokenString))
			{
				return new RtfToken(tokenString, accumulateUnderscoreText); 
			}

			return base.getToken(tokenString);
		}

		private bool containsUnderscores(string tokenString)
		{
			return Regex.IsMatch(tokenString, "_+");
		}

		private void accumulateUnderscoreText(string tokenString)
		{
			if (rtfState.InContent)
			{
				string textPattern = "[^_]+";
				string underscorePattern = "_+";
				string textOrUnderscorePattern = "(" + textPattern + "|" + underscorePattern + ")";

				MatchCollection matches = Regex.Matches(tokenString, textOrUnderscorePattern);

				int blankIndex = 0;

				foreach (Match match in matches)
				{
					if (Regex.IsMatch(match.Value, underscorePattern))
					{
                        string xmlBlankTag = "<blank label=\"{0}\" length=\"{1}\"></blank>";
						accumulateLiteralText(string.Format(xmlBlankTag, new AlphaLabel(blankIndex++), match.Value.Length));
					}
					else
					{
						accumulateLiteralText(getCurrentFontStartTag(newFontIndex, newFontSize, newColorIndex));
						accumulateLiteralText(match.Value);
						accumulateLiteralText(fontEndTag);
					}
				}
			}
		}
	}
}
