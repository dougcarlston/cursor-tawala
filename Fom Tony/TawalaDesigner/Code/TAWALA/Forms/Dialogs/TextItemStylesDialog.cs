// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Forms.Properties;

namespace Tawala.Forms
{
    public partial class TextItemStylesDialog : Form
    {
        private readonly MDIFormView activeForm;
        private bool applyToAll = true;

        protected TextItemStylesDialog()
        {
            InitializeComponent();
        }

        public TextItemStylesDialog(MDIFormView view) : this()
        {
            activeForm = view;
        }

        public bool TextApplyAllSpecified { get { return applyToAll; } }

        public bool TextNormalSpecified { get { return radioButtonTextNormal.Checked; } }

        public bool TextInstructionalSpecified { get { return radioButtonTextInstructional.Checked; } }

        public bool TextErrorSpecified { get { return radioButtonTextError.Checked; } }

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
        }

        private bool anyStyleSelected()
        {
            return radioButtonTextNormal.Checked || radioButtonTextInstructional.Checked || radioButtonTextError.Checked;
        }

        private bool anyTextItemSelected()
        {
            if (activeForm != null)
            {
                foreach (Control containerControl in activeForm.FormItemContainer.Controls)
                {
                    var itemView = containerControl as TextItemView;

                    if (itemView != null && itemView.Selected)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool singleTextItemSelected()
        {
            int count = 0;

            if (activeForm != null)
            {
                foreach (Control containerControl in activeForm.FormItemContainer.Controls)
                {
                    var itemView = containerControl as TextItemView;

                    if (itemView != null && itemView.Selected)
                    {
                        count++;
                    }
                }
            }

            return (count == 1);
        }

        private TextItemView selectedTextItem()
        {
            if (activeForm != null)
            {
                foreach (Control containerControl in activeForm.FormItemContainer.Controls)
                {
                    var itemView = containerControl as TextItemView;

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
            if (!anyTextItemSelected())
            {
                buttonApplySelected.Visible = false;
				// removed Apply to All feature; inadvertent click wreaks havoc in large projects
				//														jdf - 09/09
				//labelApplyOptions.Text = Resources.ApplyStyleToAllTextItems;
            }
        }

        private void CheckStyleButtons()
        {
            if (singleTextItemSelected())
            {
                checkRadioButton(radioButtonTextNormal, selectedTextItem().ProjectTextItem.Style == "normal");
                checkRadioButton(radioButtonTextInstructional, selectedTextItem().ProjectTextItem.Style == "instructional");
                checkRadioButton(radioButtonTextError, selectedTextItem().ProjectTextItem.Style == "error");

                checkBoxNoPaddingBottom.Checked = !selectedTextItem().ProjectTextItem.PaddingBottom;
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