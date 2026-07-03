using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Tawala.ProjectUI
{
	public static class ImageFileDialog
	{
		public static string Browse(IWin32Window owner)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "GIF (*.gif), JPEG (*.jpg), PNG (*.png) | *.gif; *.jpg; *.png";

			if (dialog.ShowDialog(owner) == DialogResult.OK)
			{
				return dialog.FileName;
			}

			return null;
		}
	}
}
