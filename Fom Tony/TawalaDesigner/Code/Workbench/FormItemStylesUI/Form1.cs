using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FormItemStylesUI
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void styleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			StylesDialog stylesDialog = new StylesDialog();

			if (stylesDialog.ShowDialog() == DialogResult.OK)
			{
				
			}
		}
	}
}