// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.FormDesigner.FormItemOptions
{
	public interface IFibOptionsPresenter
	{
		void QuestionLabelChanged(string newLabel);
		void BlankLabelChanged(string newLabel);
		void BlankResponseRequiredChanged(bool required);
	}
}
