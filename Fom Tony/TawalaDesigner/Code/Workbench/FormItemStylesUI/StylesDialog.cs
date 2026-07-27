using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FormItemStylesUI
{
	public partial class StylesDialog : Form
	{
		public StylesDialog()
		{
			InitializeComponent();
		}

		private void buttonFIB_Click(object sender, EventArgs e)
		{
			tabControl1.SelectedTab = tabControl1.TabPages["tabPageFib"];
			radioButtonApplyAll.Text = "Apply this style to all fill-in-the-blank questions in the project";
			radioButtonApplySelected.Text = "Apply this style to the selected fill-in-the-blank questions";
		}

		private void buttonMCQ_Click(object sender, EventArgs e)
		{
			tabControl1.SelectedTab = tabControl1.TabPages["tabPageMCQ"];
			radioButtonApplyAll.Text = "Apply this style to all multiple-choice questions in the project";
			radioButtonApplySelected.Text = "Apply this style to the selected multiple-choice questions";
		}

		private void StylesDialog_Load(object sender, EventArgs e)
		{
		}
	}
}