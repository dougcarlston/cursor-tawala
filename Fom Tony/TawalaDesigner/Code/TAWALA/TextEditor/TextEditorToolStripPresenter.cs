using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Tawala.TextEditor
{
	public class TextEditorToolStripPresenter
	{
		public TextEditorToolStripPresenter(ToolStripSplitButton fcb, ToolStripMenuItem rfcbmi, ToolStripMenuItem rfcmi)
		{
			fontColorButton = fcb;
			recentFontColorButtonMenuItem = rfcbmi;
			recentFontColorMenuItem = rfcmi;
		}

		public void Synchronize()
		{
			UpdateActiveFontColor(activeColor);
			UpdateRecentFontColor(recentColor);
		}

		public void SetTextEditor(ITextEdit textEdit)
		{
			editor = textEdit;
		}

		public void UpdateSelectedTextColor(Color color)
		{
			editor.Selection.SetFontColor(color);
			UpdateActiveFontColor(color);
		}

		public void ChooseFontTextColor()
		{
			ColorDialog cd = new ColorDialog();
			cd.AllowFullOpen = true;
			cd.FullOpen = true;
			cd.AnyColor = true;

			if (!IsThemeColor(activeColor))
			{
				cd.Color = activeColor;
			}

			if (customColors != null)
			{
				cd.CustomColors = customColors;
			}

			if (cd.ShowDialog(Application.OpenForms[0]) == DialogResult.OK)
			{
				customColors = cd.CustomColors;
				UpdateSelectedTextColor(cd.Color);
			}
		}

		public static bool IsThemeColor(Color c)
		{
			return c.Equals(themeColor);
		}

		public void UpdateRecentFontColor(Color c)
		{
			setRecentColor(c, recentFontColorButtonMenuItem);
			setRecentColor(c, recentFontColorMenuItem);
		}

		public void UpdateActiveFontColor(Color c)
		{
			UpdateRecentFontColor(c);
			setActiveColor(c, fontColorButton);
		}

		#region Private

		private void setActiveColor(Color c, ToolStripItem item)
		{
			activeColor = c;
			setColor(c, item, createFontColorBitmap);
		}

		private void setRecentColor(Color c, ToolStripItem item)
		{
			if (IsThemeColor(c))
				return;

			recentColor = c;
			setColor(c, item, createRecentColorBitmap);
		}

		private delegate Bitmap createBitmap(Color c);

		private void setColor(Color c, ToolStripItem item, createBitmap create)
		{
			Image old = item.Image;
			Bitmap bmp = create(c);
			item.Image = bmp;
			if (old != null)
			{
				old.Dispose();
			}
			item.Tag = c;
		}

		private static Bitmap createFontColorBitmap(Color c)
		{
			if (c.Equals(themeColor))
			{
				return new Bitmap(Properties.Resources.FontColorButtonTheme);
			}
			else
			{
				Image image = Properties.Resources.FontColorButton;
				Bitmap bmp = new Bitmap(image);
				using (Graphics g = Graphics.FromImage(bmp))
				{
					using (Brush b = new SolidBrush(c))
					{
						g.FillRectangle(b, 0, 12, 16, 4);
					}
				}
				return bmp;
			}
		}

		private static Bitmap createRecentColorBitmap(Color c)
		{
			Bitmap b = new Bitmap(16, 16);
			using (Graphics g = Graphics.FromImage(b))
			{
				g.FillRectangle(Brushes.Transparent, 0, 0, 16, 16);
				using (SolidBrush sb = new SolidBrush(c))
				{
					g.FillRectangle(sb, 1, 1, 14, 14);
				}
			}

			return b;
		}

		private static int[] customColors;

		private ITextEdit editor;

		private static readonly Color themeColor = Color.FromArgb(0, 0, 1);

		private static Color activeColor = themeColor;
		private static Color recentColor = Color.Black;

		private ToolStripSplitButton fontColorButton;
		private ToolStripMenuItem recentFontColorButtonMenuItem;
		private ToolStripMenuItem recentFontColorMenuItem;

		#endregion
	}
}
