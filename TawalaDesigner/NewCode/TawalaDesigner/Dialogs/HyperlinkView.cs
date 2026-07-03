// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.ProjectUI;

namespace Tawala.Dialogs
{
    public partial class HyperlinkView : Form, IHyperlinkView
    {
        private readonly IHyperlinkPresenter presenter;

        internal HyperlinkView(Hyperlink hyperlink)
        {
            InitializeComponent();

            DialogResult = DialogResult.Cancel;

            presenter = new HyperlinkPresenter(this, hyperlink);
        }

        #region IHyperlinkView Members

        public string DisplayText
        {
            get { return textBoxDisplayText.Text; }
            set { textBoxDisplayText.Text = value; }
        }

        public string Url
        {
            get { return textBoxUrl.Text; }
            set { textBoxUrl.Text = value; }
        }

        public bool NewWindow
        {
            get { return checkBoxNewWindow.Checked; }
            set { checkBoxNewWindow.Checked = value; }
        }

        #endregion

        private void buttonOK_Click(object sender, EventArgs e)
        {
            presenter.ApplyChanges();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void updateUI()
        {
            buttonOK.Enabled = textBoxDisplayText.Text.Length > 0 && textBoxUrl.Text.Length > 0;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBoxDisplayText_TextChanged(object sender, EventArgs e)
        {
            updateUI();
        }

        private void textBoxUrl_TextChanged(object sender, EventArgs e)
        {
            updateUI();
        }

        private void textBoxUrl_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = (textBoxUrl.AcceptsDropOf(e.Data) ? DragDropEffects.Copy : DragDropEffects.None);
        }

        private void textBoxUrl_DragDrop(object sender, DragEventArgs e)
        {
            IField field = textBoxUrl.DraggedField(e.Data);

            textBoxUrl.Insert(field);
            textBoxUrl.Focus();
        }

        private void textBoxUrl_Enter(object sender, EventArgs e)
        {
            FieldsPalette.Palette.FieldNodeDoubleClick += palette_FieldNodeDoubleClick;
        }

        private void textBoxUrl_Leave(object sender, EventArgs e)
        {
            FieldsPalette.Palette.FieldNodeDoubleClick -= palette_FieldNodeDoubleClick;
        }

        private void palette_FieldNodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var field = e.Node.Tag as IField;

            if (field != null)
            {
                textBoxUrl.Paste(field.FieldString);
            }
        }
    }
}