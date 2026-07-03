// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.FormDesigner.FormItemOptions
{
	public interface IFibOptionsView
	{
		string QuestionLabel { get; set; }
		string SelectedBlankLabel { get; set; }
		bool SelectedBlankRequired { get; set; }
		string LabelStatusText { set; }
	}
}
