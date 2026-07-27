using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Tawala.ProjectUI
{
	public partial class InsertTableDialog : Form
	{
		public InsertTableDialog()
		{
			InitializeComponent();
		}

		public double TableWidth
		{
			get
			{
				return Convert.ToDouble(widthNumericUpDown.Value);
			}
		}

		public int Columns
		{
			get
			{
				return Convert.ToInt32(columnsNumericUpDown.Value);
			}
		}

		public int Rows
		{
			get
			{
				return Convert.ToInt32(rowsNumericUpDown.Value);
			}
		}
	}
}