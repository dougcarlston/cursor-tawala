// $Workfile: RtfTextAttribute.cs $
// $Revision: 2 $	$Date: 4/25/06 10:46a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Text;

namespace Tawala.RtfSupport
{
	public class RtfTextAttribute
	{
		public RtfTextAttribute(string startTag, string endTag)
		{
			this.startTag = startTag;
			this.endTag = endTag;
		}

		protected RtfTextAttribute()
		{
		}

		private string startTag = "";
		public virtual string StartTag
		{
			get
			{
				return startTag;
			}
		}

		private string endTag = "";
		public virtual string EndTag
		{
			get
			{
				return endTag;
			}
		}

		public static RtfTextAttribute Bold = new RtfTextAttribute("<b>", "</b>");
		public static RtfTextAttribute Italic = new RtfTextAttribute("<i>", "</i>");
		public static RtfTextAttribute Underline = new RtfTextAttribute("<u>", "</u>");
	};

	public class RtfFontAttribute : RtfTextAttribute
	{
		private bool faceSet = false;
		private bool sizeSet = false;
		private bool colorSet = false;

		private string face = string.Empty;
		private int size = 0;
		private int r=0, g=0, b=0;

		public RtfFontAttribute(string face)
		{
			this.face = face;
			faceSet = true;
		}

		public RtfFontAttribute(int size)
		{
			this.size = size;
			sizeSet = true;
		}

		public RtfFontAttribute(int r, int g, int b)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			colorSet = true;
		}

		public override string StartTag
		{
			get
			{
				StringBuilder sb = new StringBuilder("<font");
				if (faceSet)
				{
					sb.AppendFormat(" face=\"{0}\"", face);
				}
				if (sizeSet)
				{
					sb.AppendFormat(" size=\"{0}\"", size);
				}
				if (colorSet)
				{
					sb.AppendFormat(" color=\"{0},{1},{2}\"", r, g, b);
				}
				sb.Append(">");
				return sb.ToString();
			}
		}

		public override string EndTag
		{
			get
			{
				return "</font>";
			}
		}
	}
}