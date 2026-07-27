// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.Projects.Forms.FormItemContents
{
	public class FontStyle
	{
		private int size;
		private string color;
		private string face;

		public FontStyle()
		{
			this.face = string.Empty;
			this.size = NewFont.DefaultFontSize;
			this.color = string.Empty;
		}

		public FontStyle(NewFont font)
		{
			this.face = font.FontFace;
			this.size = font.FontSizeInPoints;
			this.color = font.FontColor;
		}

		public bool HasSize
		{
			get { return (size != 0); }
		}

		public int Size
		{
			get { return size; }
			set { size = value; }
		}

		public bool HasColor
		{
			get { return (!string.IsNullOrEmpty(color)); }
		}

		public string Color
		{
			get { return color; }
			set { color = value; }
		}

		public bool HasFace
		{
			get { return (!string.IsNullOrEmpty(face)); }
		}

		public string Face
		{
			get { return face; }
			set { face = value; }
		}
	}
}
