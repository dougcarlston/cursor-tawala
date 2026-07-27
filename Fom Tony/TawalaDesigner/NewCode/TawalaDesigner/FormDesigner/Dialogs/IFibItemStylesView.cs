// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System.Windows.Forms;

namespace Tawala.FormDesigner.Dialogs
{
	public interface IFibItemStylesView
	{
		bool FibApplyAllSpecified { get; }

		bool FibFreeformSpecified { get; }
		bool FibLeftLabelsSpecified { get; }
		bool FibRightLabelsSpecified { get; }
		bool FibLeftLabelsJustifiedSpecified { get; }
		bool FibRightLabelsJustifiedSpecified { get; }
		bool FibTopLabelsSpecified { get; }

		DialogResult ShowDialog();
	}
}