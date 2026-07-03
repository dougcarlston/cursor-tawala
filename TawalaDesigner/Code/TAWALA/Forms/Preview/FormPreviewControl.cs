// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Form=Tawala.Projects.Form;

namespace Tawala.Forms.Preview
{
    public partial class FormPreviewControl : UserControl
    {
        private Form form;

        public FormPreviewControl()
        {
            InitializeComponent();
        }

        internal bool CanPrint { get { return !browser.IsBusy; } }

        internal void SetPreviewForm(Form previewForm)
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

        internal void ShowPrintPreviewDialog()
        {
            browser.ShowPrintPreviewDialog();
        }

        internal void ShowPrintDialog()
        {
            browser.ShowPrintDialog();
        }
    }
}