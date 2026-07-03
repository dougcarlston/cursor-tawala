// $Workfile: GradientLabel.cs $
// $Revision: 7 $	$Date: 11/25/05 4:35p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Tawala.Common
{
	/// <summary>
	/// Standard label header for various Designer panels.  For now the control uses hard-coded colors
	/// rather than supporting assignable properties.
	/// </summary>
	public class GradientLabel : System.Windows.Forms.Label
	{
		private static readonly Color gradBegin = Color.FromArgb(89, 135, 214);
		private static readonly Color gradEnd = Color.FromArgb(4, 57, 148);

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public GradientLabel()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			base.ForeColor = System.Drawing.Color.White;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			Margin = new Padding(0);
			Padding = new Padding(0);
		}
		#endregion

		protected override void OnPaint(PaintEventArgs pe)
		{
			LinearGradientBrush brush = new LinearGradientBrush(ClientRectangle, gradBegin, gradEnd, 90.0f);
			Graphics g = pe.Graphics;
			g.FillRectangle(brush, ClientRectangle);
			base.OnPaint(pe);
		}
	
		public override Color ForeColor
		{
			get
			{
				return base.ForeColor;
			}
		}
	
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			base.SetBoundsCore (x, y, width, 20, specified | BoundsSpecified.Height);
		}
	}
}
