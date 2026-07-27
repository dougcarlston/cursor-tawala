// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Forms.Properties;

namespace Tawala.Forms
{
    public partial class FibItemStylesDialog : Form
    {
        private readonly MDIFormView activeForm;
        private bool applyToAll = true;

        protected FibItemStylesDialog()
        {
            InitializeComponent();
        }

        public FibItemStylesDialog(MDIFormView view) : this()
        {
            activeForm = view;
        }

        public bool FibApplyAllSpecified { get { return applyToAll; } }

        public bool FibFreeformSpecified { get { return radioButtonFIBFreeform.Checked; } }

        public bool FibLeftLabelsSpecified { get { return radioButtonFIBLeftLabels.Checked; } }

        public bool FibRightLabelsSpecified { get { return radioButtonFIBRightLabels.Checked; } }

        public bool FibLeftLabelsJustifiedSpecified { get { return radioButtonFIBLeftLabelsJustified.Checked; } }

        public bool FibRightLabelsJustifiedSpecified { get { return radioButtonFIBRightLabelsJustified.Checked; } }

        public bool FibTopLabelsSpecified { get { return radioButtonFIBTopLabels.Checked; } }

        private void FormItemStylesDialog_Activated(object sender, EventArgs e)
        {
            Application.Idle += application_Idle;
        }

        private void application_Idle(object sender, EventArgs e)
        {
            buttonApplyAll.Enabled = anyStyleSelected();
            buttonApplySelected.Enabled = anyStyleSelected();
        }

        private bool anyStyleSelected()
        {
            return radioButtonFIBFreeform.Checked || radioButtonFIBLeftLabels.Checked || radioButtonFIBRightLabels.Checked ||
                   radioButtonFIBLeftLabelsJustified.Checked || radioButtonFIBRightLabelsJustified.Checked ||
                   radioButtonFIBTopLabels.Checked;
        }

        private bool anyFibItemSelected()
        {
            if (activeForm != null)
            {
                foreach (Control control in activeForm.FormItemContainer.Controls)
                {
                    var itemView = control as FibItemView;

                    if (itemView != null && itemView.Selected)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool singleFibItemSelected()
        {
            int count = 0;

            if (activeForm != null)
            {
                foreach (Control control in activeForm.FormItemContainer.Controls)
                {
                    var itemView = control as FibItemView;

                    if (itemView != null && itemView.Selected)
                    {
                        count++;
                    }
                }
            }

            return (count == 1);
        }

        private FibItemView selectedFibItem()
        {
            if (activeForm != null)
            {
                foreach (Control control in activeForm.FormItemContainer.Controls)
                {
                    var itemView = control as FibItemView;

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
            if (!anyFibItemSelected())
            {
                buttonApplySelected.Visible = false;
                labelApplyOptions.Text = Resources.ApplyStyleToAllFIBItems;
            }
        }

        private void CheckStyleButtons()
        {
            if (singleFibItemSelected())
            {
                checkRadioButton(radioButtonFIBFreeform, selectedFibItem().ProjectFibItem.Style == "freeform");
                checkRadioButton(radioButtonFIBLeftLabels, selectedFibItem().ProjectFibItem.Style == "leftAlignLabels");
                checkRadioButton(radioButtonFIBRightLabels, selectedFibItem().ProjectFibItem.Style == "rightAlignLabels");
                checkRadioButton(radioButtonFIBLeftLabelsJustified, selectedFibItem().ProjectFibItem.Style == "leftAlignLabelsJustified");
                checkRadioButton(radioButtonFIBRightLabelsJustified, selectedFibItem().ProjectFibItem.Style == "rightAlignLabelsJustified");
                checkRadioButton(radioButtonFIBTopLabels, selectedFibItem().ProjectFibItem.Style == "topLabels");
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