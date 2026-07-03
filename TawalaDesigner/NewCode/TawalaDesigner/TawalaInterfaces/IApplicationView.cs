// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System.Windows.Forms;
using System.Drawing;

namespace Tawala.Interfaces
{
	public interface IApplicationView
	{
		FontFamily[] GetFontList();
		Control ComponentPalette { get; }
		void SetProjectNameInTitleBar(string projectName);
		SaveFileDialog CreateSaveFileDialog();
	}
}
