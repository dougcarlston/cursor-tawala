// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Controls;
using Tawala.Projects;
using Tawala.Projects.Links;
using Tawala.ProjectUI;
using Form=System.Windows.Forms.Form;

namespace Tawala.Dialogs
{
    public partial class InsertHyperlinkDialog : Form
    {
        private readonly ConditionGroupCollection groups;
        private EventHandler callbackOnSuccess;
        private Hyperlink hyperLink;

        protected InsertHyperlinkDialog()
        {
            InitializeComponent();

            textBoxUrl.TextChanged += text_Changed;
            textBoxDisplayText.TextChanged += text_Changed;

            bindUI(groupBoxConditions, "Enabled", checkBoxConditionalDisplay, "Checked");
            bindUI(labelWhen1, "Enabled", checkBoxConditionalDisplay, "Checked");
            bindUI(comboBoxAndOr, "Enabled", checkBoxConditionalDisplay, "Checked");
            bindUI(labelWhen2, "Enabled", checkBoxConditionalDisplay, "Checked");

            groups = new ConditionGroupCollection(groupBoxConditions, comboBoxAndOr, true);
        }

        public ILink Hyperlink
        {
            get
            {
                return hyperLink;
            }
        }

        private static void bindUI(Control target, string targetProperty, Control controlSource, string sourceProperty)
        {
            var binding = new Binding(targetProperty, controlSource, sourceProperty);
            binding.ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;
            binding.DataSourceUpdateMode = DataSourceUpdateMode.Never;
            target.DataBindings.Add(binding);
        }

        public static void New(EventHandler callbackSuccess)
        {
            var dialog = new InsertHyperlinkDialog();
            dialog.callbackOnSuccess = callbackSuccess;
            dialog.Show(Application.OpenForms[0]);
        }

        public static void Edit(Hyperlink existing, EventHandler callbackSuccess)
        {
            var dialog = new InsertHyperlinkDialog();
            dialog.callbackOnSuccess = callbackSuccess;
            dialog.hyperLink = existing;
            dialog.Show(Application.OpenForms[0]);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (hyperLink != null)
            {
                textBoxDisplayText.Text = hyperLink.DisplayText;
                textBoxUrl.Text = hyperLink.Url;
                checkBoxNewWindow.Checked = hyperLink.OpenNewWindow;

                if (hyperLink.Conditions != null)
                {
                    checkBoxConditionalDisplay.Checked = true;
                    groups.Conditions = hyperLink.Conditions;
                }
            }

            base.OnLoad(e);
        }

        private void text_Changed(object sender, EventArgs e)
        {
            buttonOK.Enabled = textBoxUrl.Text.Length > 0;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (hyperLink == null)
            {
                hyperLink = new Hyperlink();
            }

            hyperLink.DisplayText = textBoxDisplayText.Text;
            hyperLink.Url = textBoxUrl.Text;
            hyperLink.OpenNewWindow = checkBoxNewWindow.Checked;
            hyperLink.Conditions = (!checkBoxConditionalDisplay.Checked || groups.Conditions.Count == 0) ? null : groups.Conditions;

            Close();

            callbackOnSuccess(this, EventArgs.Empty);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBoxExpression_DragDrop(object sender, DragEventArgs e)
        {
            var textBoxExpression = sender as ComplexExpressionTextBox;

            var field = textBoxExpression.DraggedField(e.Data);

            textBoxExpression.Insert(field);
            textBoxExpression.Focus();
        }

        private void textBoxExpression_DragEnter(object sender, DragEventArgs e)
        {
            var textBoxExpression = sender as ComplexExpressionTextBox;
            e.Effect = (textBoxExpression.AcceptsDropOf(e.Data) ? DragDropEffects.Copy : DragDropEffects.None);
        }

        private void textBox_Enter(object sender, EventArgs e)
        {
            FieldsPalette.Palette.FieldNodeDoubleClick += palette_FieldNodeDoubleClick;
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            FieldsPalette.Palette.FieldNodeDoubleClick -= palette_FieldNodeDoubleClick;
        }

        private static void palette_FieldNodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var textBoxExpression = sender as ComplexExpressionTextBox;
            var field = e.Node.Tag as IField;

            if (field != null)
            {
                textBoxExpression.Paste(field.FieldString);
            }
        }

        private void groupBoxConditions_Layout(object sender, LayoutEventArgs e)
        {
            int top = 14;
            int height = 14;

            foreach (Control c in groupBoxConditions.Controls)
            {
                c.Top = top;
                c.Left = 2;
                c.Width = groupBoxConditions.Width - 4;
                c.PerformLayout();
                top = c.Bottom;
                height += c.Height;
            }

            groupBoxConditions.Height = height + 10;

            Height = groupBoxConditions.Bottom + SystemInformation.CaptionHeight + buttonOK.Height + 30;
        }

        private void comboBoxAndOr_VisibleChanged(object sender, EventArgs e)
        {
            if (comboBoxAndOr.Visible)
            {
                labelWhen2.Visible = true;
                if (comboBoxAndOr.SelectedIndex < 0)
                {
                    comboBoxAndOr.SelectedIndex = 0;
                }
                PerformLayout();
            }
            else
            {
                labelWhen2.Visible = false;
            }
        }
    }
}