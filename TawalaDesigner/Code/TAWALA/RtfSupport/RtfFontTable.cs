// $Workfile: RtfFontTable.cs $
// $Revision: 8 $	$Date: 11/15/07 11:39a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.XmlSupport;

namespace Tawala.RtfSupport
{
	[Serializable]
	public class RtfFontTable : Collection<RtfFontTableEntry>
	{

		private const string NEWLINE = "\r\n";
		private const string rtfFontTableStart = @"{\fonttbl" + NEWLINE;
		private const string rtfFontTableEnd = @"}" + NEWLINE;

		public void AddUnique(RtfFontTableEntry entry)
		{
			if (IndexMatching(entry.FontName) == -1)
			{
				Add(entry);
			}
		}

		private static readonly string defaultFontSizeTag = @"\fs20";

		public string ToRtf()
		{
			StringBuilder rtfString = new StringBuilder(rtfFontTableStart);

			for (int i = 0; i < Count; i++)
			{
				RtfFontTableEntry entry = this[i];

				if (entry.FontName != null)
				{
					rtfString.Append(entry.ToRtf(i));
				}
			}

			rtfString.Append(rtfFontTableEnd);

			rtfString.Append(defaultFontSizeTag);

			return rtfString.ToString();
		}

		public int IndexMatching(string fontFace)
		{
			for (int i = 0; i < Count; i++)
			{
				RtfFontTableEntry entry = this[i];

				if (entry.FontName == fontFace)
				{
					return i;
				}
			}

			return -1;
		}

	}

	[Serializable]
	public class RtfFontTableEntry
	{
		private string fontFamily;
		private string fontName;

		public RtfFontTableEntry()
		{
		}

		public RtfFontTableEntry(IXmlElement element)
		{
			this.fontFamily = "nil";
			this.fontName = element.GetAttribute("face");
		}

		public RtfFontTableEntry(string fontFamily, string fontName)
		{
			this.fontFamily = fontFamily;
			this.fontName = fontName;
		}

		private const string NEWLINE = "\r\n";
		private const string rtfFontTableEntry = "{{\\f{0}\\f{1} {2};}}" + NEWLINE;

		public string ToRtf(int entryIndex)
		{
			StringBuilder rtfString = new StringBuilder();
			rtfString.AppendFormat(rtfFontTableEntry, entryIndex, fontFamily, fontName);

			return rtfString.ToString();
		}

		public string FontFamily
		{
			get { return fontFamily; }
			set { fontFamily = value; }
		}

		public string FontName
		{
			get { return fontName; }
			set { fontName = value.TrimEnd(';'); }
		}
	}
}
