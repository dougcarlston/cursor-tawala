// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System.Windows.Forms;

namespace Tawala.FormDesigner.Dialogs
{
	public interface ITextItemStylesView
	{
		bool TextApplyAllSpecified { get; }

		bool TextNormalSpecified { get; }
		bool TextInstructionalSpecified { get; }
		bool TextErrorSpecified { get; }

		DialogResult ShowDialog();
	}
}