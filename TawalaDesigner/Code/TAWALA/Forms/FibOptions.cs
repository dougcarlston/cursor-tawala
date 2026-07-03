// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.XPath;
using Tawala.Forms.Properties;
using Tawala.Functions.Runtime;

namespace Tawala.Forms
{
    /// <summary>
    /// Options panel for a Fill-in-the-Blank Form Item (FibItemView)
    /// </summary>
    public partial class FibOptions : OptionsView
    {
        private string oldAlternateLabel = string.Empty;

        public FibOptions()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            populateValidatorComboList();

            comboBoxValidation.SelectionChangeCommitted += comboBoxValidation_SelectionChangeCommitted;

            panel.Controls.Remove(labelLengthTextBox);
            panel.Controls.Remove(textBoxLength);
        }

        internal FibItemView OwningFibItemView
        {
            get { return owner as FibItemView; }
            set
            {
                SetOwner(value);

                if (value != null)
                {
                    //string length = string.Empty;
                    bool required = false;

                    if (value.CurrentBlank != null)
                    {
                        //length = value.CurrentBlank.Length.ToString();
                        required = value.CurrentBlank.Required;
                        ValidationFunction = value.CurrentBlank.ValidationFunction;
                    }

                    //textBoxLength.Text = length;
                    checkBoxRequired.Checked = required;
                    textBoxAlternateLabel.Text = value.FormItem.AlternateLabel;
                    oldAlternateLabel = textBoxAlternateLabel.Text;
                }
            }
        }

        public string AlternateLabel
        {
            get { return textBoxAlternateLabel.Text; }
            set
            {
                textBoxAlternateLabel.Text = value;
                oldAlternateLabel = value;
            }
        }

        public bool Required { get { return checkBoxRequired.Checked; } set { checkBoxRequired.Checked = value; } }

        public int BlankHeight { get { return Convert.ToInt32(textBoxHeight.Text); } set { textBoxHeight.Text = value < 1 ? string.Empty : value.ToString(); } }

        public IFunction ValidationFunction { get { return comboBoxValidation.Tag as IFunction; } set { setValidationComboBoxValue(value); } }

        private void setValidationComboBoxValue(IFunction value)
        {
            comboBoxValidation.Tag = value;

            if (ValidationFunction == null)
            {
                comboBoxValidation.SelectedIndex = 0;
                linkValidationEdit.Enabled = false;
            }
            else
            {
                for (int i = 1; i < comboBoxValidation.Items.Count; ++i)
                {
                    if (ValidationFunction.Info == comboBoxValidation.Items[i] as IFunctionInfo)
                    {
                        comboBoxValidation.SelectedIndex = i;
                        linkValidationEdit.Enabled = ValidationFunction.PropertyCount > 0;
                        break;
                    }
                }
            }
        }

        private void populateValidatorComboList()
        {
            XPathNodeIterator xPathIterator = FunctionLoader.Repository.Xml.SelectDescendants(@"blank-validator", "", true);

            while (xPathIterator.MoveNext())
            {
                IFunctionInfo info = FunctionLoader.Repository.Functions[xPathIterator.Current.GetAttribute("id", "")];
                comboBoxValidation.Items.Add(info);
            }
        }

        private void comboBoxValidation_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                var functionInfo = comboBoxValidation.SelectedItem as IFunctionInfo;
                var currentFunctionInfo = ValidationFunction == null ? null : ValidationFunction.Info;

                if (functionInfo != currentFunctionInfo)
                {
                    if (functionInfo == null)
                    {
                        comboBoxValidation.Tag = null;
                        linkValidationEdit.Enabled = false;
                    }
                    else
                    {
                        comboBoxValidation.Tag = functionInfo.CreateInstance();
                        linkValidationEdit.Enabled = functionInfo.Parameters.Count > 0;
                    }
                }

                OwningFibItemView.UpdateCurrentBlankValidationFunction();
				OwningFibItemView.EditValidationFunction();
			}
        }

        private void linkLabelEdit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OwningFibItemView.EditValidationFunction();
        }

        private void checkBoxRequired_CheckedChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                OwningFibItemView.UpdateCurrentBlankRequiredFlag();
            }
        }

        private void validateAndUpdateOwnerAlternateLabel()
        {
            if (Parent != null && Parent.Parent != null)
            {
                if (!isLabelValid(textBoxAlternateLabel.Text))
                {
                    textBoxAlternateLabel.Text = isLabelValid(oldAlternateLabel) ? oldAlternateLabel : string.Empty;
                }

                OwningFibItemView.UpdateCurrentBlankAlternateLabel();
            }
        }

        private bool isLabelValid(string label)
        {
            return label.Length == 0 ||
                   OwningFibItemView.Parent.Form.ItemList.ValidAlternateLabel(OwningFibItemView.FormItem, OwningFibItemView.CurrentBlank,
                                                                              label);
        }

        public void ForceAlternateLabelUpdate()
        {
            if (owner != null && textBoxAlternateLabel.Focused)
            {
                validateAndUpdateOwnerAlternateLabel();
            }
        }

        public void ForceHeightUpdate()
        {
            if (owner != null && textBoxHeight.Focused)
            {
                OwningFibItemView.UpdateCurrentBlankHeight();
            }
        }

        private void textBoxHeight_Leave(object sender, EventArgs e)
        {
            int height = -1;
            try
            {
                height = Convert.ToInt32(textBoxHeight.Text);
            }
            catch
            {
                height = -1;
            }
            finally
            {
                if (height < 1)
                {
                    textBoxHeight.Text = "1";
                }
            }

            OwningFibItemView.UpdateCurrentBlankHeight();
        }

        private void textBoxAlternateLabel_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
            if (!isLabelValid(textBoxAlternateLabel.Text))
            {
                e.Cancel = true;
                SetError(textBoxAlternateLabel, Resources.InvalidFieldName);
                textBoxAlternateLabel.Select(0, 0);
            }
        }

        private void textBoxAlternateLabel_Validated(object sender, EventArgs e)
        {
            SetError(textBoxAlternateLabel, "");
            validateAndUpdateOwnerAlternateLabel();
        }
    }
}