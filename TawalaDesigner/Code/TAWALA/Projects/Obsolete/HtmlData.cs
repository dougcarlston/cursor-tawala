// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
    /// <summary>
    /// Class to handle HTML data inside a CDATA section
    /// </summary>
    public class HtmlData
    {
        private readonly string text;

        public HtmlData(IXmlElement element)
        {
            text = element.GetChild("#cdata-section").Value == null ? "" : element.GetChild("#cdata-section").Value;
        }

        public string Text { get { return text; } }

        public static string ExtractText(IXmlElement element)
        {
            string extractedString = element.GetChild("#cdata-section").Value == null ? "" : element.GetChild("#cdata-section").Value;

            // remove unneeded style tag which creates problematic style info with RTF output
            extractedString = Regex.Replace(extractedString, "<style type=.*?</style>", "", RegexOptions.Singleline);
            return extractedString;
        }

        public static string ConvertToPlainTextRtf(IXmlElement element)
        {
            string extractedString = element.GetChild("#cdata-section").Value == null ? "" : element.GetChild("#cdata-section").Value;

            var rtfString = new StringBuilder();

            // extract paragraphs
            MatchCollection matches = Regex.Matches(extractedString, @"<p[^>]*>.*?</p>");
            foreach (Match m in matches)
            {
                string plainText = Regex.Replace(m.ToString(), "<[^>]+>", "");
                if (!plainText.Equals("&nbsp;")) // TX control throws these in; ignore
                {
                    rtfString.Append(plainText);
                    rtfString.Replace("&lt;", "<");
                    rtfString.Replace("&gt;", ">");
                    rtfString.Replace("&amp;", "&");
                    rtfString.Append(@"\par ");
                }
            }

            return rtfString.ToString();
        }
    }
}