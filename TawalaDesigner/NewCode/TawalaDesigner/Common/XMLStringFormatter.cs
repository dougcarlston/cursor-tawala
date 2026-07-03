// $Workfile: XMLStringFormatter.cs $
// $Revision: 3 $	$Date: 11/25/05 4:35p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Tawala.Common
{
	/// <summary>
	/// Replaces special characters with XML entity references
	/// </summary>
	public class XMLStringFormatter
	{
		public XMLStringFormatter()
		{
		}

		/// <summary>
		/// constructor that takes an unformatted string as an argument
		/// </summary>
		/// <param name="str"></param>
		public XMLStringFormatter(string str)
		{
			RawString = str;
		}

		/// <summary>
		/// the raw (unformatted) string
		/// </summary>
		private string rawString;
		public string RawString
		{
			get
			{
				return rawString;
			}
			set
			{
				rawString = value;

				// format the string
				format();
			}
		}

		/// <summary>
		/// the formatted string with entity references
		/// </summary>
		private string formattedString;
		public string FormattedString
		{
			get
			{
				return formattedString;
			}
		}

		/// <summary>
		/// replaces special characters with XML entity references
		/// </summary>
		private void format()
		{
			if (rawString != null)
			{
				StringBuilder sb = new StringBuilder(rawString);
				sb.Replace("&", "&amp;");
				sb.Replace("<", "&lt;");
				sb.Replace(">", "&gt;");
				sb.Replace("\"", "&quot;");
				sb.Replace("'", "&apos;");
				formattedString = sb.ToString();
			}
		}

		/// <summary>
		/// Regex.Replace callback function to fixup document text 
		/// Escapes special characters as determined by a RegEx match filter.
		/// </summary>
		static private string matchEvaluator(Match m)
		{
			if (m.Value == "&")
			{
				return "&amp;";
			}
			else if (m.Value == "<")
			{
				return "&lt;";
			}
			else if (m.Value == ">")
			{
				return "&gt;";
			}
			else if (m.Value == "\"")
			{
				return "&quot;";
			}
			else if (m.Value == "'")
			{
				return "&apos;";
			}

			return string.Empty;
		}
		static public string EscapeAttributeText(string s)
		{
			return Regex.Replace(s, "&|<|>|\"|'", new MatchEvaluator(matchEvaluator));
		}

		static public string EscapeElementText(string s)
		{
			return Regex.Replace(s, "&|<|>", new MatchEvaluator(matchEvaluator));
		}
	}

}
