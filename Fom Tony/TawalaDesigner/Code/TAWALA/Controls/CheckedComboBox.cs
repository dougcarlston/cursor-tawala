// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Tawala.Controls
{
    [Browsable(false)]
    public class CheckedComboBox : ComboBox
    {
        protected CheckedListBox checkedListBox;

        /// <summary>
        /// Bookkeeping for checked/unchecked state for items in checkedListBox. This double-bookkeeping is necessary
        /// for keeping the combo box text in sync, because the ItemCheck event occurs before the items in 
        /// checkListBox are updated by the framework. -SB
        /// </summary>
        protected Collection<CheckState> checkStates = new Collection<CheckState>();

        public CheckedComboBox()
        {
            checkedListBox = new CheckedListBox();
            checkedListBox.Name = "checkedListBox";

            checkedListBox.CheckOnClick = true;
            checkedListBox.ThreeDCheckBoxes = true;
            checkedListBox.Visible = false;
            checkedListBox.MinimumSize = new Size(1, 19);
            checkedListBox.MaximumSize = new Size(700, 17*4);

            Controls.Add(checkedListBox);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		
        public CheckedListBox.ObjectCollection ListItems
        {
            get
            {
                return checkedListBox.Items;
            }
        }

        /// <summary>
        /// Indicates whether the specified index corresponds to the last (bottommost) checked item in checkListBox.
        /// </summary>
        protected bool isLastCheckedItem(int index)
        {
            return (index == lastCheckedIndex());
        }

        /// <summary>
        /// Returns the index of the last (bottommost) checked item in checkListBox.
        /// </summary>
        private int lastCheckedIndex()
        {
            int lastCheckedIndex = -1;

            for (int i = checkStates.Count - 1; i >= 0; i--)
            {
                if (isChecked(i))
                {
                    lastCheckedIndex = i;
                    break;
                }
            }

            return lastCheckedIndex;
        }

        protected bool isChecked(int index)
        {
            return (checkStates[index] == CheckState.Checked);
        }
    }
}