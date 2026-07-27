// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.FormDesigner.FormItemOptions
{
	public interface IMcqOptionsPresenter
	{
		void QuestionLabelChanged(string newLabel);
		void ResponseRequiredChanged(bool required);
		void SelectMoreThanOneChanged(bool selectMoreThanOne);
	}
}
