// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Tawala.Forms.Properties;

namespace Tawala.Forms
{
    public partial class FibStylesDialog : Form
    {
        private readonly MDIFormView activeForm;
        private bool applyToAll = true;

        protected FibStylesDialog()
        {
            InitializeComponent();
        }

        public FibStylesDialog(MDIFormView view) : this()
        {
            activeForm = view;
        }

        public bool FibApplyAllSpecified { get { return applyToAll; } }

        public bool FibFreeformSpecified { get { return radioButtonFreeform.Checked; } }

        public bool FibLeftLabelsSpecified { get { return radioButtonLeftJustified.Checked && !checkBoxAlignRight.Checked; } }

        public bool FibRightLabelsSpecified { get { return radioButtonRightJustified.Checked && !checkBoxAlignRight.Checked; } }

        public bool FibLeftLabelsJustifiedSpecified { get { return radioButtonLeftJustified.Checked && checkBoxAlignRight.Checked; } }

        public bool FibRightLabelsJustifiedSpecified { get { return radioButtonRightJustified.Checked && checkBoxAlignRight.Checked; } }

        public bool FibTopLabelsSpecified { get { return radioButtonAbove.Checked; } }

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
            checkBoxAlignRight.Enabled = radioButtonLeftJustified.Checked || radioButtonRightJustified.Checked;
        }

        private bool anyStyleSelected()
        {
            return radioButtonFreeform.Checked || radioButtonLeftJustified.Checked || radioButtonRightJustified.Checked ||
                   radioButtonAbove.Checked;
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

        private bool identicallyStyledFibItemsSelected()
        {
            var styleOccurrences = new Dictionary<string, int>();

            if (activeForm != null)
            {
                foreach (Control control in activeForm.FormItemContainer.Controls)
                {
                    var itemView = control as FibItemView;

                    if (itemView != null && itemView.Selected)
                    {
                        incrementStyleOccurrenceCount(styleOccurrences, itemView.ProjectFibItem.Style);
                    }
                }

                if (styleOccurrences.Count == 1)
                {
                    foreach (var keyValuePair in styleOccurrences)
                    {
                        return (styleOccurrences[keyValuePair.Key] > 1);
                    }
                }
            }

            return false;
        }

        private static void incrementStyleOccurrenceCount(Dictionary<string, int> styleOccurrences, string styleString)
        {
            if (!styleOccurrences.ContainsKey(styleString))
            {
                styleOccurrences.Add(styleString, 0);
            }

            styleOccurrences[styleString]++;
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

        protected override void OnLoad(EventArgs e)
        {
            showApplyOptions();
            checkStyleButtons();
            base.OnLoad(e);
        }

        private void showApplyOptions()
        {
            if (!anyFibItemSelected())
            {
                buttonApplySelected.Visible = false;
				// removed Apply to All feature; inadvertent click wreaks havoc in large projects
				//														jdf - 09/09
				//labelApplyOptions.Text = Resources.ApplyStyleToAllFIBItems;
            }
        }

        private void checkStyleButtons()
        {
            string style = null;

            if (singleFibItemSelected() || identicallyStyledFibItemsSelected())
            {
                style = selectedFibItem().ProjectFibItem.Style;
            }

            if (style == null)
            {
                style = string.Empty;
            }

            radioButtonFreeform.Checked = style == "freeform";
            radioButtonLeftJustified.Checked = style.Contains("leftAlignLabels");
            radioButtonRightJustified.Checked = style.Contains("rightAlignLabels");
            radioButtonAbove.Checked = style.Contains("topLabels");

            checkBoxAlignRight.Checked = style.Contains("Justified");
        }

        private void buttonApplySelected_Click(object sender, EventArgs e)
        {
            applyToAll = false;
        }

        private void buttonApplyAll_Click(object sender, EventArgs e)
        {
            applyToAll = true;
        }

        private void updatePreview(Control sample)
        {
            panelPreview.Controls.Clear();
            sample.Size = panelPreview.ClientSize;
            sample.Dock = DockStyle.Fill;
            panelPreview.Controls.Add(sample);
        }

        private void radioButtonAbove_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAbove.Checked)
            {
                updatePreview(new FibAboveLabelsSample());
            }
        }

        private void radioButtonLeftJustified_CheckedChanged(object sender, EventArgs e)
        {
            previewLeftLabels();
        }

        private void previewLeftLabels()
        {
            if (radioButtonLeftJustified.Checked)
            {
                if (checkBoxAlignRight.Checked)
                {
                    updatePreview(new FibLeftLabelsJustifiedSample());
                }
                else
                {
                    updatePreview(new FibLeftLabelsSample());
                }
            }
        }

        private void radioButtonFreeform_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonFreeform.Checked)
            {
                updatePreview(new FibFreeformSample());
            }
        }

        private void radioButtonRightJustified_CheckedChanged(object sender, EventArgs e)
        {
            previewRightLabels();
        }

        private void previewRightLabels()
        {
            if (radioButtonRightJustified.Checked)
            {
                if (checkBoxAlignRight.Checked)
                {
                    updatePreview(new FibRightLabelsJustifiedSample());
                }
                else
                {
                    updatePreview(new FibRightLabelsSample());
                }
            }
        }

        private void checkBoxAlignRight_CheckedChanged(object sender, EventArgs e)
        {
            previewLeftLabels();
            previewRightLabels();
        }

        private void radioButtonAbove_Click(object sender, EventArgs e)
        {
            if (!radioButtonAbove.Checked)
            {
                radioButtonAbove.Checked = true;
            }
        }

        private void radioButtonLeftJustified_Click(object sender, EventArgs e)
        {
            if (!radioButtonLeftJustified.Checked)
            {
                radioButtonLeftJustified.Checked = true;
            }
        }

        private void radioButtonRightJustified_Click(object sender, EventArgs e)
        {
            if (!radioButtonRightJustified.Checked)
            {
                radioButtonRightJustified.Checked = true;
            }
        }

        private void radioButtonFreeform_Click(object sender, EventArgs e)
        {
            if (!radioButtonFreeform.Checked)
            {
                radioButtonFreeform.Checked = true;
            }
        }
    }
}