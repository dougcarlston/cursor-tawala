// $Workfile: FieldsPanel.cs $
// $Revision: 1 $	$Date: 6/01/06 2:07p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Tawala.Common;
using Tawala.Projects;

///<summary>
///FieldsPanel is a container for the contents of FieldsPalette and also
///provides the resize capability.
///It was originally an internal class of FieldsPalette.cs but the VSN Form Designer had issues with
///that some times.  It would remove the construct/assignment statement for it.
///</summary>
namespace Tawala.ProjectUI
{
	public class FieldsPanel : Panel
	{
		private bool isDrag = false;
		private Rectangle theRectangle;
		private Point lastMousePos;

		public FieldsPanel()
		{
			DoubleBuffered = true;
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			if (Cursor != Cursors.Default)
			{
				Cursor = Cursors.Default;
			}
		}

		private bool isMouseOverResizeBar()
		{
			if (MouseButtons == MouseButtons.None || MouseButtons == MouseButtons.Left)
			{
				Point ptClient = PointToClient(MousePosition);
				return ptClient.X >= Left && ptClient.X < Left + Padding.Left - 1;
			}
			return false;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (isMouseOverResizeBar())
			{
				isDrag = true;
				Cursor = Cursors.SizeWE;
				theRectangle = RectangleToScreen(new Rectangle(Left, 0, Padding.Left, Height));
				Invalidate(RectangleToClient(theRectangle));
				lastMousePos = MousePosition;
			}
			else
			{
				isDrag = false;
				base.OnMouseDown(e);
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (isMouseOverResizeBar())
			{
				Cursor = Cursors.SizeWE;
			}

			if (isDrag)
			{
				Point mp = MousePosition;
				int desiredWidth = Width + (lastMousePos.X - mp.X);

				if (desiredWidth < 60)
				{
					desiredWidth = 60;
				}
				else if (desiredWidth > Parent.Parent.Width - 50)
				{
					desiredWidth = Parent.Parent.Width - 50;
				}

				Parent.Width = desiredWidth;
				theRectangle = RectangleToScreen(new Rectangle(Left, 0, Padding.Left, Height));
				Refresh();

				lastMousePos = mp;
			}
			else
			{
				base.OnMouseMove(e);
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (isDrag)
			{
				isDrag = false;
				Cursor = Cursors.Default;
				Invalidate(RectangleToClient(theRectangle));
			}
			else
			{
				base.OnMouseUp(e);
			}
		}

		/// <summary>
		/// OnPaint - Draw border on left side
		/// </summary>
		protected override void OnPaint(PaintEventArgs e)
		{
			Pen border = isDrag ? Pens.DarkGray : Pens.Gray;
			Pen interior = isDrag ? Pens.Gray : Pens.LightGray;

			// Draw left border for sizing and separation

			e.Graphics.DrawLine(border, Left, 0, Left, Height);
			for (int i = 1; i < Padding.Left-1; ++i)
			{
				e.Graphics.DrawLine(interior, Left + i, 0, Left + i, Height);
			}
			e.Graphics.DrawLine(border, Left + Padding.Left-1, 0, Left + Padding.Left-1, Height);
		}
	}
}