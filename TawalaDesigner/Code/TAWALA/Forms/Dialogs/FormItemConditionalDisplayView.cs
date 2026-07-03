// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Controls;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Form=System.Windows.Forms.Form;

namespace Tawala.Forms.Dialogs
{
    public partial class FormItemConditionalDisplayView : Form, IFormItemConditionalDisplayView
    {
        private readonly ConditionGroupCollection groups;
        private readonly IFormItemConditionalDisplayPresenter presenter;

        public FormItemConditionalDisplayView(IFormItem item)
        {
            InitializeComponent();

            groups = new ConditionGroupCollection(groupBoxConditions, comboBoxAndOr, true);

            presenter = new FormItemConditionalDisplayPresenter(this, item);
        }

        #region IFormItemConditionalDisplayView Members

        public void SetDisplayConditions(Conditions conditions)
        {
            groups.Conditions = conditions;
            checkBoxConditionalDisplay.Checked = conditions.Count > 0;
        }

        public Conditions GetDisplayConditions()
        {
            return checkBoxConditionalDisplay.Checked ? groups.Conditions : new Conditions();
        }

        #endregion

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            presenter.ConditionsDefined();
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
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

            Height = groupBoxConditions.Bottom + SystemInformation.CaptionHeight + okButton.Height + 30;
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

        private void checkBoxConditionalDisplay_CheckedChanged(object sender, EventArgs e)
        {
            labelWhen1.Enabled =
                comboBoxAndOr.Enabled = labelWhen2.Enabled = groupBoxConditions.Enabled = checkBoxConditionalDisplay.Checked;
        }
    }
}