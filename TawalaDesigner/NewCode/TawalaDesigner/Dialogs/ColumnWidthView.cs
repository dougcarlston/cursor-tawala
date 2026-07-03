using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Tawala.Dialogs
{
	public partial class ColumnWidthView : Form
	{
		public ColumnWidthView(double columnWidth)
		{
			InitializeComponent();

			numericUpDownColumnWidth.Value = Convert.ToDecimal(columnWidth);
			numericUpDownColumnWidth.Select();
		}

		public double ColumnWidth
		{
			get { return Convert.ToDouble(numericUpDownColumnWidth.Value); }
		}
	}
}
