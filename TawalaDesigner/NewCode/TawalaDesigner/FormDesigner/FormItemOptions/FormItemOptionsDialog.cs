using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Tawala.FormDesigner.FormItemOptions
{
	public partial class FormItemOptionsDialog : Form
	{
		public FormItemOptionsDialog()
		{
			InitializeComponent();
		}

		private void FormItemOptionsDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (userClickedCloseBox(e))
			{
				hideInsteadOfClosing(e);
			}
		}

		private static bool userClickedCloseBox(FormClosingEventArgs e)
		{
			return e.CloseReason == CloseReason.UserClosing;
		}

		private void hideInsteadOfClosing(FormClosingEventArgs e)
		{
			e.Cancel = true;
			Hide();
		}
	}
}
