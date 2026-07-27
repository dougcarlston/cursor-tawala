// $Workfile: RtfTokenizer.cs $
// $Revision: 5 $	$Date: 1/26/07 6:54p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.XmlSupport;

namespace Tawala.RtfSupport
{
	public class RtfTokenizer
	{
		public static StringCollection Tokenize(string rtfString)
		{
			return tokenize(rtfString);
		}

		private static StringCollection tokenize(string rtfString)
		{
			StringCollection tokenStrings = new StringCollection();

			string newlinePattern = "[\r\n]+";
			string startGroupPattern = @"{";
			string endGroupPattern = @"}";
			string escapedTextPattern = @"\\[{}\\]";
			string commandPattern = @"(\\[a-zA-Z*]+(-?[0-9]+)?) ?";
			string textPattern = @"[^\\{}]+";

			string tokenPattern =
				newlinePattern + "|" +
				startGroupPattern + "|" +
				endGroupPattern + "|" +
				escapedTextPattern + "|" +
				commandPattern + "|" +
				textPattern;

			MatchCollection matches = Regex.Matches(rtfString, tokenPattern);

			foreach (Match match in matches)
			{
				string tokenString = (match.Groups[1].Value == "" ? match.Value : match.Groups[1].Value);

				if (!tokenString.StartsWith("\r") && !tokenString.StartsWith("\n"))
				{
					tokenStrings.Add(tokenString);
				}
			}

			return tokenStrings;
		}


	}
}
