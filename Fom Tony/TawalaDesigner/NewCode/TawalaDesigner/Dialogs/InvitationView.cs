// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.ProjectUI;

namespace Tawala.Dialogs
{
    public partial class InvitationView : Form, IInvitationView
    {
        private readonly IInvitationPresenter presenter;

        private string initialFormName;
        private string projectName;

        internal InvitationView(InvitationField invitation)
        {
            InitializeComponent();

            DialogResult = DialogResult.Cancel;

            textBoxKeyExpression.DataBindings.Add("Enabled", checkBoxPrivateInvitation, "Checked", false);
            labelPrivate.DataBindings.Add("Enabled", checkBoxPrivateInvitation, "Checked", false);

            presenter = new InvitationPresenter(this, invitation);

            var projectBinding = new ProjectNameBindingSource(comboBoxInvitationProject, projectName);
            var formBinding = new FormNameBindingSource(comboBoxInvitationStartingPoint, projectBinding, initialFormName);
        }

        #region IInvitationView Members

        public string InitialFormName
        {
            get { return comboBoxInvitationStartingPoint.Text; }
            set { initialFormName = value; }
        }

        public string ProjectName
        {
            get { return comboBoxInvitationProject.Text; }
            set { projectName = value; }
        }

        public string DisplayText
        {
            get { return textBoxDisplayText.Text; }
            set { textBoxDisplayText.Text = value; }
        }

        public bool IsPrivate
        {
            get { return checkBoxPrivateInvitation.Checked; }
            set { checkBoxPrivateInvitation.Checked = value; }
        }

        public CompoundExpression AuthenticationTokenExpression
        {
            get { return new CompoundExpression(textBoxKeyExpression.Text); }
            set
            {
                if (value != null)
                {
                    textBoxKeyExpression.Text = value.ToString();
                }
                else
                {
                    textBoxKeyExpression.Text = string.Empty;
                }
            }
        }

        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            comboBoxInvitationProject.SelectionChangeCommitted += delegate { updateOKButton(); };
            comboBoxInvitationStartingPoint.SelectionChangeCommitted += delegate { updateOKButton(); };
            textBoxDisplayText.TextChanged += delegate { updateOKButton(); };
            textBoxKeyExpression.TextChanged += delegate { updateOKButton(); };
            checkBoxPrivateInvitation.CheckedChanged += delegate { updateOKButton(); };

            updateOKButton();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            presenter.ApplyChanges();
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private bool isText(string text)
        {
            return text != null && text.Trim().Length != 0;
        }

        private bool shouldOkBeEnabled()
        {
            bool bOk = true;
            bOk &= isText(comboBoxInvitationProject.Text);
            bOk &= isText(comboBoxInvitationStartingPoint.Text);
            bOk &= isText(textBoxDisplayText.Text);
            bOk &= (!checkBoxPrivateInvitation.Checked ||
                    (checkBoxPrivateInvitation.Checked && isText(textBoxKeyExpression.Text)));
            return bOk;
        }

        private void updateOKButton()
        {
            bool state = shouldOkBeEnabled();
            if (okButton.Enabled != state)
            {
                okButton.Enabled = state;
            }
        }

        private void textBoxKeyExpression_DragDrop(object sender, DragEventArgs e)
        {
            IField field = textBoxKeyExpression.DraggedField(e.Data);
            textBoxKeyExpression.Paste(field.FieldString);
            updateOKButton();
        }

        private void textBoxKeyExpression_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = (textBoxKeyExpression.AcceptsDropOf(e.Data) ? DragDropEffects.Copy : DragDropEffects.None);
            updateOKButton();
        }

        private void textBoxKeyExpression_Enter(object sender, EventArgs e)
        {
            FieldsPalette.Palette.FieldNodeDoubleClick += palette_FieldNodeDoubleClick;
        }

        private void textBoxKeyExpression_Leave(object sender, EventArgs e)
        {
            FieldsPalette.Palette.FieldNodeDoubleClick -= palette_FieldNodeDoubleClick;
        }

        private void palette_FieldNodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var field = e.Node.Tag as IField;

            if (field != null)
            {
                textBoxKeyExpression.Paste(field.FieldString);
            }
        }
    }
}