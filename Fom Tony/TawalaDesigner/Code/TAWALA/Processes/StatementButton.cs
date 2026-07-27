// $Workfile: StatementButton.cs $
// $Revision: 2 $	$Date: 11/25/05 4:35p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Tawala.Processes
{

	public class StatementButton : Button
	{
		private bool active = false;
		private static Color activeBackColor = Color.FromArgb(240, 160, 38);
		private static Color inactiveBackColor = Color.FromArgb(144, 176, 208);

		public bool Active
		{
			get
			{
				return active;
			}
			set
			{
				if (active != value)
				{
					active = value;
					Invalidate();
				}
			}
		}

		public override Color BackColor
		{
			get
			{
				return Enabled ? (active ? activeBackColor : inactiveBackColor) : Color.LightGray;
			}
			set
			{
			}
		}

		protected override void OnCreateControl()
		{
			if (Parent != null)
			{
				Padding m = Margin;
				m.Left = 5;
				m.Right = 0;
				m.Top = 0;
				m.Bottom = 1;
				Margin = m;
				Dock = DockStyle.None;
				Anchor = AnchorStyles.Left;
				Width = Parent.Width - Margin.Left * 2;
			}
			base.OnCreateControl();
		}

		protected override void OnPaint(PaintEventArgs pevent)
		{
			Graphics g = pevent.Graphics;
			Rectangle r = new Rectangle(0, 0, Width, Height);
			Rectangle borderRect = new Rectangle(0, 0, Width - 1, Height - 1);

			using (SolidBrush b = new SolidBrush(BackColor))
			{
				g.FillRectangle(b, r);
			}

			if (Focused && active)
			{
				g.DrawRectangle(Pens.Black, borderRect);
				borderRect.Inflate(-1, -1);
				g.DrawRectangle(Pens.Black, borderRect);
			}
			else
			{
				g.DrawRectangle(Enabled ? Pens.Blue : Pens.DarkGray, borderRect);
			}

			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			sf.LineAlignment = StringAlignment.Center;
			g.DrawString(Text, Font, Enabled ? Brushes.Black : Brushes.Gray, r, sf);
		}

		protected override void OnEnter(EventArgs e)
		{
			base.OnEnter(e);
			FlatAppearance.BorderColor = Color.Black;
			FlatAppearance.BorderSize = 2;
		}

		protected override void OnLeave(EventArgs e)
		{
			base.OnLeave(e);
			FlatAppearance.BorderColor = Enabled ? Color.Blue : Color.DarkGray;
			FlatAppearance.BorderSize = 1;
		}
	}
}
