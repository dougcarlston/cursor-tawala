// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System.Windows.Forms;

namespace Tawala.FormDesigner.Dialogs
{
	public interface IMcqItemStylesView
	{
		bool McqApplyAllSpecified { get; }
		
		bool MCQVerticalSpecified { get; }
		bool MCQHorizontalSpecified { get; }
		bool MCQMultiColumnSpecified { get; }
		int MCQMultiColumnCount { get; }
		DialogResult ShowDialog();
	}
}