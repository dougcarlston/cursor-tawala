using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Tawala.ProjectUI
{
	public partial class IntermediateColorDialog : Form
	{
		public IntermediateColorDialog()
		{
			InitializeComponent();
		}

		public bool UseProjectThemeColor
		{
			get
			{
				return (radioButtonUseThemeColor.Checked == true);
			}
		}
	}
}