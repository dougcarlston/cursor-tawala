// $Workfile: ComponentPalette.cs $
// $Revision: 3 $	$Date: 11/25/05 5:00p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Tawala.ProjectUI
{
	public partial class ComponentPalette : UserControl
	{
		public ComponentPalette()
		{
			InitializeComponent();
			Dock = DockStyle.Left;
		}

		protected override void OnControlAdded(ControlEventArgs e)
		{
			base.OnControlAdded(e);
			Width = e.Control.Width;
			e.Control.Dock = DockStyle.Fill;
		}
	}
}
