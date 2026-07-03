// $Workfile: ITextSelection.cs $
// $Revision: 9 $	$Date: 10/10/07 9:59a $
// Copyright © 2005-2006 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Tawala.TextEditor
{
	public interface ITextSelection
	{
		Tristate Bold
		{
			get;
			set;
		}

		Tristate Italic
		{
			get;
			set;
		}

		Tristate Underline
		{
			get;
			set;
		}

		bool GetFontColor(out Color c);

		void SetFontColor(Color c);

		string FontName
		{
			get;
			set;
		}

		double FontPointSize
		{
			get;
			set;
		}

		void ResetFormatting();

		HorizontalAlignment ParagraphHAlignment
		{
			get;
			set;
		}

		int Start
		{
			get;
			set;
		}

		int Length
		{
			get;
			set;
		}

		string Text
		{
			get;
			set;
		}
	}
}
