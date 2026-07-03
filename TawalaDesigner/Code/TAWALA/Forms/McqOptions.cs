// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.Forms
{
    /// <summary>
    /// Options panel for a Multiple Choice Form Item (McqItemView)
    /// </summary>
    public partial class McqOptions : OptionsView
    {
        public McqOptions()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        internal McqItemView OwningMCQItemView
        {
            get { return owner as McqItemView; }
            set
            {
                SetOwner(value);

                if (value != null)
                {
                    checkBoxMoreThanOneAllowed.Checked = !value.ProjectMCItem.SelectOnlyOne;
                    checkBoxRequired.Checked = value.ProjectMCItem.RequireAtLeastOne;
                    comboBoxSource.SelectedIndex = value.ProjectMCItem.DataSourceFunction != null ? 1 : 0;
                }
            }
        }

        private void comboBoxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            linkLabelEdit.Enabled = comboBoxSource.SelectedIndex > 0;

            if (comboBoxSource.SelectedIndex == 0)
            {
                OwningMCQItemView.ProjectMCItem.DataSourceFunction = null;
            }
            else if (OwningMCQItemView.ProjectMCItem.DataSourceFunction == null)
            {
                OwningMCQItemView.EditDataSourceFunction();
            }
        }

        private void linkLabelEdit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OwningMCQItemView.EditDataSourceFunction();
        }

        #region Control Events

        private void checkBoxOnlyOne_CheckedChanged(object sender, EventArgs e)
        {
            OwningMCQItemView.ProjectMCItem.SelectOnlyOne = !checkBoxMoreThanOneAllowed.Checked;
        }

        private void checkBoxRequired_CheckedChanged(object sender, EventArgs e)
        {
            OwningMCQItemView.ProjectMCItem.RequireAtLeastOne = checkBoxRequired.Checked;
        }

        #endregion
    }
}