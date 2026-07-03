// $Workfile: RtfColorTable.cs $
// $Revision: 5 $	$Date: 10/06/06 4:06p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.XmlSupport;

namespace Tawala.RtfSupport
{
    [Serializable]
    public class RtfColorTable : Collection<RtfColorTableEntry>
	{

		private const string NEWLINE = "\r\n";
		private const string rtfColorTableStart = @"{\colortbl;" + NEWLINE;
		private const string rtfColorTableEnd = @"}" + NEWLINE;

		public void AddUnique(RtfColorTableEntry entry)
		{
			if (IndexMatching(entry.ToRgbColor()) == -1)
			{
				Add(entry);
			}
		}

		public string ToRtf()
		{
			StringBuilder rtfString = new StringBuilder(rtfColorTableStart);

			for (int i = 0; i < Count; i++)
			{
				RtfColorTableEntry entry = this[i];
				rtfString.Append(entry.ToRtf());
			}

			rtfString.Append(rtfColorTableEnd);

			return rtfString.ToString();
		}

		public int IndexMatching(int rgbColor)
		{
			for (int i = 0; i < Count; i++)
			{
				RtfColorTableEntry entry = this[i];
				int entryColor = (entry.Red << 16) + (entry.Green << 8) + entry.Blue;

				if (entryColor == rgbColor)
				{
					return i;
				}
			}

			return -1;
		}
	}

    [Serializable]
    public class RtfColorTableEntry
	{
		private int red;
		private int green;
		private int blue;

		public RtfColorTableEntry()
		{
		}

		public RtfColorTableEntry(int red, int green, int blue)
		{
			this.red = red;
			this.green = green;
			this.blue = blue;
		}

		public RtfColorTableEntry(IXmlElement element)
		{
			int rgbColor = Convert.ToInt32(element.GetAttribute("color"), 16);
			
			this.red = (rgbColor >> 16) & 0xff;
			this.green = (rgbColor >> 8) & 0xff;
			this.blue = rgbColor & 0xff;
		}

		private const string NEWLINE = "\r\n";

		public string ToRtf()
		{
			string rtfColorTableEntry = "\\red{0}\\green{1}\\blue{2};" + NEWLINE;

			return String.Format(rtfColorTableEntry, red, green, blue);
		}

		public string ToHexString()
		{
			return String.Format("{0:X6}", (red << 16) + (green << 8) + blue);
		}

		public int ToRgbColor()
		{
			return ((red << 16) + (green << 8) + blue);
		}

		public int Red
		{
			get { return red; }
			set { red = value; }
		}

		public int Green
		{
			get { return green; }
			set { green = value; }
		}

		public int Blue
		{
			get { return blue; }
			set { blue = value; }
		}
	}
}
