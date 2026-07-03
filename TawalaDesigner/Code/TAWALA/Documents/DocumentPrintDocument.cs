// $Workfile: DocumentPrintDocument.cs $
// $Revision: 2 $	$Date: 3/15/06 1:42p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Tawala.Documents
{
	class DocumentPrintDocument : PrintDocument
	{
		private DocumentEditor te;
		private Font printFont;
		private StreamReader sr;

		internal DocumentPrintDocument(DocumentEditor te)
		{
			this.te = te;
		}

        protected override void OnBeginPrint(PrintEventArgs e) 
        {
			printFont = new Font("Arial", 10);

			MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(te.Text));
			sr = new StreamReader(ms, Encoding.Unicode, true);

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
			string line = null;

			// Calculate the number of lines per page.
			linesPerPage = e.MarginBounds.Height /
			   printFont.GetHeight(e.Graphics);

			// Print each line of the file.
			while (count < linesPerPage &&
			   ((line = sr.ReadLine()) != null))
			{
				yPos = topMargin + (count *
				   printFont.GetHeight(e.Graphics));
				e.Graphics.DrawString(line, printFont, Brushes.Black,
				   leftMargin, yPos, new StringFormat());
				count++;
			}

			e.HasMorePages = line != null;
		}

		protected override void OnEndPrint(PrintEventArgs e)
        {
            base.OnEndPrint(e);

			sr.Close();
			sr = null;

			printFont.Dispose();
			printFont = null;
		}
	}
}
