// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Tawala.FormDesigner
{
//	[Designer(typeof(ParentControlDesigner), typeof(IDesigner))]
	public partial class OptionsPanel : UserControl
	{
		public OptionsPanel()
		{
			InitializeComponent();
			Title = "Options";
		}

		public void SetOptions(Control c)
		{
			panel1.Controls.Clear();

			if (c != null)
			{
				c.Dock = DockStyle.Fill;
				panel1.Controls.Add(c);
				c.PerformLayout();
			}
		}

		protected override void OnControlAdded(ControlEventArgs e)
		{
			if (e.Control != toolWindowCaption1)
			{
				Controls.SetChildIndex(toolWindowCaption1, Controls.Count - 1);
			}
			base.OnControlAdded(e);
		}

		public string Title
		{
			get { return toolWindowCaption1.Text; }
			set { toolWindowCaption1.Text = value; }
		}
	}
}
