// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace Tawala.Forms
{
    internal class FormPrintDocument : PrintDocument
    {
        private readonly FormItemContainer ic;
        private int item;

        internal FormPrintDocument(FormItemContainer ic)
        {
            this.ic = ic;
        }

        protected override void OnBeginPrint(PrintEventArgs e)
        {
            base.OnBeginPrint(e);
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            base.OnPrintPage(e);

            int left = e.MarginBounds.Left;
            int top = e.MarginBounds.Top;
            int width = e.MarginBounds.Width;

            while (item < ic.Controls.Count)
            {
                Control c = ic.Controls[item];
                var b = new Bitmap(c.Width, c.Height);
                c.DrawToBitmap(b, new Rectangle(0, 0, c.Width, c.Height));
                var h = (int)(b.Height*100/b.VerticalResolution);
                if (top + h >= e.MarginBounds.Bottom)
                {
                    break;
                }

                e.Graphics.DrawImage(b, left, top);
                top += h;
                ++item;
            }

            e.HasMorePages = item < ic.Controls.Count;
        }

        protected override void OnEndPrint(PrintEventArgs e)
        {
            base.OnEndPrint(e);
        }
    }
}