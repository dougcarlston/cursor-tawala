// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using Tawala.Projects;

namespace Tawala.Controls
{
    /// <summary>
    /// ComboBox whose dropdown list contains Form names preceded by check boxes.
    /// </summary>
    [Browsable(false)]
    public class CheckedFormComboBox : CheckedComboBox
    {
        public CheckedFormComboBox()
        {
            checkedListBox.ItemCheck += checkedListBox_ItemCheck;
            checkedListBox.LostFocus += checkedListBox_LostFocus;

            DropDown += checkedFormComboBox_DropDown;
        }

        public CheckedFormComboBox(string name) : this()
        {
            Name = name;
        }

        public CheckedListBox CheckedListBox
        {
            get
            {
                return checkedListBox;
            }
        }

        /// <summary>
        /// Gets and sets the entire list of Forms that appear in the combo box's dropdown.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		
        public FormList Forms
        {
            get
            {
                var forms = new FormList();

                foreach (IForm form in checkedListBox.Items)
                {
                    forms.Add(form);
                }

                return forms;
            }

            set
            {
                clearCheckedForms();

                foreach (IForm form in value)
                {
                    int index = checkedListBox.Items.IndexOf(form);

                    if (index == -1)
                    {
                        addUnchecked(form);
                    }
                }

                for (int i = checkedListBox.Items.Count - 1; i >= 0; i--)
                {
                    var form = checkedListBox.Items[i] as IForm;

                    if (!value.Contains(form))
                    {
                        checkedListBox.Items.RemoveAt(i);
                    }
                }

                setHeight();

                setText();
            }
        }

        /// <summary>
        /// Get - Returns a list of Forms that are currently checked in the combo box's dropdown.
        /// Set - Checks the specified Forms and unchecks all other Forms in the combo box's dropdown.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		
        public FormList CheckedForms
        {
            get
            {
                var checkedForms = new FormList();

                for (int i = 0; i < checkedListBox.Items.Count; i++)
                {
                    if (checkedListBox.GetItemChecked(i))
                    {
                        var form = checkedListBox.Items[i] as IForm;
                        checkedForms.Add(form);
                    }
                }

                return checkedForms;
            }

            set
            {
                uncheckAll();

                foreach (IForm form in value)
                {
                    int index = checkedListBox.Items.IndexOf(form);

                    if (index == -1)
                    {
                        addChecked(form);
                    }
                    else
                    {
                        checkedListBox.SetItemChecked(index, true);
                    }
                }

                setText();
            }
        }

        private void checkedFormComboBox_DropDown(object sender, EventArgs e)
        {
            checkedListBox.Parent = Parent;
            checkedListBox.Top = Bottom;
            checkedListBox.Left = Left;
            checkedListBox.Width = Width;
            checkedListBox.Height = 17*checkedListBox.Items.Count;

            checkedListBox.BringToFront();
            checkedListBox.Show();
            checkedListBox.Select();
        }

        private void checkedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            checkStates[e.Index] = e.NewValue;
            setText();
        }

        private void checkedListBox_LostFocus(object sender, EventArgs e)
        {
            checkedListBox.Hide();
        }

        private void clearCheckedForms()
        {
            checkedListBox.Items.Clear();
            checkStates.Clear();
        }

        private void addChecked(IForm form)
        {
            checkedListBox.Items.Add(form);
            checkStates.Add(CheckState.Checked);
            checkedListBox.SetItemChecked(checkedListBox.Items.Count - 1, true);
        }

        private void addUnchecked(IForm form)
        {
            checkedListBox.Items.Add(form);
            checkStates.Add(CheckState.Unchecked);
            checkedListBox.SetItemChecked(checkedListBox.Items.Count - 1, false);
        }

        private void setText()
        {
            Text = getCheckedFormString();
        }

        private void setHeight()
        {
            checkedListBox.Height = 17*checkedListBox.Items.Count;
        }

        private bool allFormsChecked()
        {
            int checkedCount = 0;

            for (int i = 0; i < checkStates.Count; i++)
            {
                if (isChecked(i))
                {
                    checkedCount++;
                }
            }

            return (Forms.Count == checkedCount);
        }

        private string getCheckedFormString()
        {
            var text = new StringBuilder();

            for (int i = 0; i < checkStates.Count; i++)
            {
                if (isChecked(i))
                {
                    IForm form = Forms[i];

                    text.Append(form.Name);

                    if (!isLastCheckedItem(i))
                    {
                        text.Append(", ");
                    }
                }
            }

            return text.ToString();
        }

        private void uncheckAll()
        {
            for (int i = 0; i < checkedListBox.Items.Count; i++)
            {
                checkedListBox.SetItemChecked(i, false);
            }
        }
    }
}