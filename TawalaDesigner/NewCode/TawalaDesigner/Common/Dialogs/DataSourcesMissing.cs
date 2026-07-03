using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Tawala.Common
{
	public partial class DataSourcesMissing : Form
	{
		public DataSourcesMissing()
		{
			InitializeComponent();
		}

		public DataSourcesMissing(List<string> missing)
			: this()
		{
			flowLayoutPanel.SuspendLayout();

			StringBuilder names = new StringBuilder();

			foreach (string ds in missing)
			{
				if (names.Length > 0)
				{
					names.Append(", ");
				}
				names.Append(ds);
			}

			Label label = new Label();
			label.AutoSize = true;
			label.Text = names.ToString();
			label.Margin = new Padding(12, 0, 3, 9);
			flowLayoutPanel.Controls.Add(label);
			flowLayoutPanel.SetFlowBreak(label, true);

			flowLayoutPanel.ResumeLayout();
			PerformLayout();
		}
	}
}