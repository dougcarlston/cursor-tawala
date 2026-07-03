using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Tawala.Projects.Forms;

namespace Tawala.FormDesigner.Preview
{
	public partial class FormPreviewControl : UserControl
	{
		public FormPreviewControl()
		{
			InitializeComponent();
		}

		internal void SetPreviewForm(IForm previewForm)
		{
			form = previewForm;
			browser.SetPreviewForm(form);
		}

		internal void Activate(string anchor)
		{
			browser.Activate(anchor);
		}

		internal void Deactivate()
		{
			browser.Deactivate();
		}

		internal bool CanPrint
		{
			get { return !browser.IsBusy; }
		}

		internal void ShowPrintPreviewDialog()
		{
			browser.ShowPrintPreviewDialog();
		}

		internal void ShowPrintDialog()
		{
			browser.ShowPrintDialog();
		}

		private IForm form = null;
	}
}
