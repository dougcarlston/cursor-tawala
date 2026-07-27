// $Workfile: ProcessPrintDocument.cs $
// $Revision: 1 $	$Date: 12/02/05 7:23a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

namespace Tawala.Processes
{
	class ProcessPrintDocument : PrintDocument
	{
		private int index;
		private string[] lines;
		private ProcessEditor pe;
		private Font printFont;

		internal ProcessPrintDocument(ProcessEditor pe)
		{
			this.pe = pe;
		}

        protected override void OnBeginPrint(PrintEventArgs e) 
        {
			printFont = new Font("Courier New", 10);
			index = 0;
			lines = pe.ListBoxLines;
			
			base.OnBeginPrint(e);
		}

		protected override void OnPrintPage(PrintPageEventArgs e)
		{
			base.OnPrintPage(e);

			float linesPerPage = 0;
			float yPos = 0;
			int count = 0;
			float leftMargin = e.MarginBounds.Left;
			float topMargin = e.MarginBounds.Top;

			// Calculate the number of lines per page.
			linesPerPage = e.MarginBounds.Height /
			   printFont.GetHeight(e.Graphics);

			// Print each line of the file.
			while (count < linesPerPage && index < lines.Length)
			{
				yPos = topMargin + (count *
				   printFont.GetHeight(e.Graphics));
				e.Graphics.DrawString(lines[index], printFont, Brushes.Black,
				   leftMargin, yPos, new StringFormat());
				count++;
				index++;
			}

			e.HasMorePages = index < lines.Length;
		}

		protected override void OnEndPrint(PrintEventArgs e)
		{
			base.OnEndPrint(e);

			printFont.Dispose();
			printFont = null;

			lines = null;
		}
	}
}
