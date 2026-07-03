// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Controls;
using Tawala.Dialogs.Properties;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Links;
using Tawala.ProjectUI;
using Form=System.Windows.Forms.Form;

namespace Tawala.Dialogs
{
    public partial class InsertInvitationDialog : Form
    {
        private static InsertInvitationDialog singleInstance = null;

        private readonly EventHandler callbackOnSuccess;
        private InvitationField draggedField;
        private FormNameBindingSource formBindingSource;
        private ProjectNameBindingSource projectBindingSource;

        private InsertInvitationDialog(InvitationField field, EventHandler callbackOnSuccess)
        {
            this.draggedField = field;
            this.callbackOnSuccess = callbackOnSuccess;

            InitializeComponent();

            textBoxKeyExpression.DataBindings.Add("Enabled", checkBoxPrivateInvitation, "Checked", false);
            labelPrivate.DataBindings.Add("Enabled", checkBoxPrivateInvitation, "Checked", false);
            checkBoxOneTimeOnly.DataBindings.Add("Enabled", checkBoxPrivateInvitation, "Checked", false);

            textBoxDisplayText.DragEnter += textBoxExpression_DragEnter;
            textBoxDisplayText.DragDrop += textBoxExpression_DragDrop;
            textBoxDisplayText.Enter += textBoxExpression_Enter;
            textBoxDisplayText.Leave += textBoxExpression_Leave;

            textBoxKeyExpression.DragEnter += textBoxExpression_DragEnter;
            textBoxKeyExpression.DragDrop += textBoxExpression_DragDrop;
            textBoxKeyExpression.Enter += textBoxExpression_Enter;
            textBoxKeyExpression.Leave += textBoxExpression_Leave;

            checkBoxOneTimeOnly.Visible = false;
        }

        public static InvitationField Result
        {
            get { return singleInstance != null ? singleInstance.draggedField : null; }
        }

        public InvitationField Invitation
        {
            get { return draggedField; }
        }

        public static void New(EventHandler callbackOnSuccess)
        {
            showModeless(null, callbackOnSuccess);
        }

        public static void Edit(InvitationField invitationField, EventHandler callbackOnSuccess)
        {
            showModeless(invitationField, callbackOnSuccess);
        }

        private static void showModeless(InvitationField field, EventHandler callbackOnSuccess)
        {
            singleInstance = new InsertInvitationDialog(field, callbackOnSuccess);
            singleInstance.Show(Application.OpenForms[0]);
        }

        private void InsertInvitation_Load(object sender, EventArgs e)
        {
            string findProj = null;
            string findForm = null;

            if (draggedField != null)
            {
                findForm = draggedField.FormName;
                findProj = draggedField.ProjectName;
                textBoxDisplayText.Text = draggedField.DisplayText;
                textBoxKeyExpression.Text = draggedField.IsPrivate ? draggedField.AuthenticationTokenExpression.ToString() : string.Empty;
                checkBoxPrivateInvitation.Checked = draggedField.IsPrivate;
                Text = Resources.EditInvDialogCaption;
            }
            else
            {
                Text = Resources.InsertInvDialogCaption;
            }

            projectBindingSource = new ProjectNameBindingSource(comboBoxInvitationProject, findProj);
            formBindingSource = new FormNameBindingSource(comboBoxInvitationStartingPoint, projectBindingSource, findForm);

            comboBoxInvitationProject.SelectionChangeCommitted += delegate { updateOKButton(); };
            comboBoxInvitationStartingPoint.SelectionChangeCommitted += delegate { updateOKButton(); };
            textBoxDisplayText.TextChanged += delegate { updateOKButton(); };
            textBoxKeyExpression.TextChanged += delegate { updateOKButton(); };
            checkBoxPrivateInvitation.CheckedChanged += delegate { updateOKButton(); };

            updateOKButton();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            singleInstance = null;
            base.OnHandleDestroyed(e);
        }

        private bool isCurrentProject()
        {
            return comboBoxInvitationProject.Text.CompareTo("(Current Project)") == 0;
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
            bOk &= (!checkBoxPrivateInvitation.Checked || (checkBoxPrivateInvitation.Checked && isText(textBoxKeyExpression.Text)));
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

        private void textBoxExpression_DragDrop(object sender, DragEventArgs e)
        {
            var textBoxExpression = sender as ExpressionTextBox;
            IField field = textBoxExpression.DraggedField(e.Data);
            textBoxExpression.Paste(field.FieldString);
            updateOKButton();
        }

        private void textBoxExpression_DragEnter(object sender, DragEventArgs e)
        {
            var textBoxExpression = sender as ExpressionTextBox;
            e.Effect = (textBoxExpression.AcceptsDropOf(e.Data) ? DragDropEffects.Copy : DragDropEffects.None);
            updateOKButton();
        }

        private void textBoxExpression_Enter(object sender, EventArgs e)
        {
            FieldsPalette.Palette.FieldNodeDoubleClick += Palette_FieldNodeDoubleClick;
        }

        private void textBoxExpression_Leave(object sender, EventArgs e)
        {
            FieldsPalette.Palette.FieldNodeDoubleClick -= Palette_FieldNodeDoubleClick;
        }

        private void Palette_FieldNodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var field = e.Node.Tag as IField;

            if (field != null)
            {
                textBoxKeyExpression.Paste(field.FieldString);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (draggedField == null)
            {
                draggedField = new InvitationField();
            }

            draggedField.DisplayText = textBoxDisplayText.Text;
            draggedField.InitialFormName = comboBoxInvitationStartingPoint.Text;

            if (isCurrentProject())
            {
                draggedField.ProjectName = string.Empty;
                draggedField.Form = Project.Current.GetForm(comboBoxInvitationStartingPoint.Text);
            }
            else
            {
                draggedField.ProjectName = comboBoxInvitationProject.Text;
                draggedField.Form = null;
            }

            draggedField.IsPrivate = checkBoxPrivateInvitation.Checked;

            if (draggedField.IsPrivate)
            {
                draggedField.AuthenticationTokenExpression = new FieldsAndLiteralsExpression(textBoxKeyExpression.Text);
            }
            else
            {
                draggedField.AuthenticationTokenExpression = null;
            }

            callbackOnSuccess(sender, e);
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}