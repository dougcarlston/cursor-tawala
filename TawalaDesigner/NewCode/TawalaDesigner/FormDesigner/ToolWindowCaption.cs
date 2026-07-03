// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Tawala.FormDesigner
{
	public partial class ToolWindowCaption : Label
	{
		private static readonly Color gradBegin = Color.FromArgb(143, 173, 206);
		private static readonly Color gradEnd = Color.FromArgb(189, 211, 239);

		public ToolWindowCaption()
		{
			InitializeComponent();
			Dock = DockStyle.Top;
			Margin = new Padding(0);
			Padding = new Padding(0);
			Font = new Font("Arial", 8.25f);
			TabIndex = 0;
		}

		public override Color ForeColor
		{
			get
			{
				return Color.Black;
			}
			set
			{
				base.ForeColor = Color.Black;
			}
		}

		public override ContentAlignment TextAlign
		{
			get
			{
				return ContentAlignment.MiddleLeft;
			}
			set
			{
				base.TextAlign = ContentAlignment.MiddleLeft;
			}
		}

		protected override void OnLayout(LayoutEventArgs levent)
		{
			Height = SystemInformation.ToolWindowCaptionHeight+1;
			Width = Parent != null ? Parent.Width : 100;
			base.OnLayout(levent);
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			return new Size(Parent != null ? Parent.Width : 100, SystemInformation.ToolWindowCaptionHeight+1);
		}

		public override Size MinimumSize
		{
			get
			{
				return Parent != null ? new Size(Parent.Width, SystemInformation.ToolWindowCaptionHeight+1) : new Size(100, SystemInformation.ToolWindowCaptionHeight+1);
			}
			set
			{
				base.MinimumSize = value;
			}
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			Graphics g = pe.Graphics;
			Rectangle r = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height - 1);
			using (LinearGradientBrush brush = new LinearGradientBrush(r, gradBegin, gradEnd, 90.0f))
			{
				g.FillRectangle(brush, ClientRectangle);
			}
			g.DrawLine(Pens.White, 0, ClientSize.Height-1, ClientSize.Width, ClientSize.Height-1);
			
			base.OnPaint(pe);
		}
	}
}
