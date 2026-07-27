// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Forms.Properties;

namespace Tawala.Forms
{
    public partial class McqItemStylesDialog : Form
    {
        private readonly MDIFormView activeForm;
        private bool applyToAll = true;

        protected McqItemStylesDialog()
        {
            InitializeComponent();
        }

        public McqItemStylesDialog(MDIFormView view) : this()
        {
            activeForm = view;
        }

        public bool MCQApplyAllSpecified { get { return applyToAll; } }

        public bool MCQVerticalSpecified { get { return radioButtonMCQVertical.Checked; } }

        public bool MCQHorizontalSpecified { get { return radioButtonMCQHorizontal.Checked; } }

        public bool MCQMultiColumnSpecified { get { return radioButtonMCQMultiColumn.Checked; } }

        public int MCQMultiColumnCount { get { return ((String)comboBoxColumnCount.SelectedItem == "Auto" ? 0 : Convert.ToInt32((String)comboBoxColumnCount.SelectedItem)); } }

        public bool PaddingBottom { get { return !checkBoxNoPaddingBottom.Checked; } }

        private void FormItemStylesDialog_Activated(object sender, EventArgs e)
        {
            Application.Idle += application_Idle;
        }

        private void application_Idle(object sender, EventArgs e)
        {
			// removed Apply to All feature; inadvertant click wreaks havoc in large projects
			//														jdf - 09/09
			//buttonApplyAll.Enabled = anyStyleSelected();
			buttonApplyAll.Enabled = false;
			buttonApplySelected.Enabled = anyStyleSelected();

            comboBoxColumnCount.Enabled = radioButtonMCQMultiColumn.Checked;
        }

        private bool anyStyleSelected()
        {
            return radioButtonMCQVertical.Checked || radioButtonMCQHorizontal.Checked || radioButtonMCQMultiColumn.Checked;
        }

        private bool anyMCQItemSelected()
        {
            if (activeForm != null)
            {
                foreach (Control containerControl in activeForm.FormItemContainer.Controls)
                {
                    var itemView = containerControl as McqItemView;

                    if (itemView != null && itemView.Selected)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool singleMCQItemSelected()
        {
            int count = 0;

            if (activeForm != null)
            {
                foreach (Control containerControl in activeForm.FormItemContainer.Controls)
                {
                    var itemView = containerControl as McqItemView;

                    if (itemView != null && itemView.Selected)
                    {
                        count++;
                    }
                }
            }

            return (count == 1);
        }

        private McqItemView selectedMCQItem()
        {
            if (activeForm != null)
            {
                foreach (Control containerControl in activeForm.FormItemContainer.Controls)
                {
                    var itemView = containerControl as McqItemView;

                    if (itemView != null && itemView.Selected)
                    {
                        return itemView;
                    }
                }
            }

            return null;
        }

        private void FormItemStylesDialog_Load(object sender, EventArgs e)
        {
            ShowApplyOptions();
            CheckStyleButtons();
        }

        private void ShowApplyOptions()
        {
            if (!anyMCQItemSelected())
            {
                buttonApplySelected.Visible = false;
				// removed Apply to All feature; inadvertent click wreaks havoc in large projects
				//														jdf - 09/09
				//labelApplyOptions.Text = Resources.ApplyStyleToAllMCQItems;
            }
        }

        private void CheckStyleButtons()
        {
            if (singleMCQItemSelected())
            {
                checkRadioButton(radioButtonMCQVertical, selectedMCQItem().ProjectMCItem.Style == "vertical");
                checkRadioButton(radioButtonMCQHorizontal, selectedMCQItem().ProjectMCItem.Style == "horizontal");
                checkRadioButton(radioButtonMCQMultiColumn, selectedMCQItem().ProjectMCItem.Style == "multicolumn");
                setMultiColumnCount();

                checkBoxNoPaddingBottom.Checked = !selectedMCQItem().ProjectMCItem.PaddingBottom;
            }
        }

        private void checkRadioButton(RadioButton button, bool check)
        {
            button.Checked = check;
            if (check && button.Parent != null && button.Parent.Parent != null)
            {
                var sc = button.Parent.Parent as ScrollableControl;
                sc.ScrollControlIntoView(button);
            }
        }

        private void setMultiColumnCount()
        {
            comboBoxColumnCount.SelectedIndex = 0;

            if (selectedMCQItem().ProjectMCItem.Style == "multicolumn")
            {
                if (selectedMCQItem().ProjectMCItem.ColumnCount > 0)
                {
                    comboBoxColumnCount.SelectedIndex = selectedMCQItem().ProjectMCItem.ColumnCount - 1;
                }
            }
        }

        private void buttonApplySelected_Click(object sender, EventArgs e)
        {
            applyToAll = false;
        }

        private void buttonApplyAll_Click(object sender, EventArgs e)
        {
            applyToAll = true;
        }
    }
}