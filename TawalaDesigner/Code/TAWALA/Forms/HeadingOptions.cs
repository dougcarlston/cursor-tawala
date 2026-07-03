// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects;

namespace Tawala.Forms
{
    public partial class HeadingOptions : OptionsView
    {
        public HeadingOptions()
        {
            InitializeComponent();
            comboBoxType.DataSource = Enum.GetValues(typeof(HeadingType));
        }

        internal HeadingView OwningHeading
        {
            get { return owner as HeadingView; }
            set
            {
                SetOwner(value);

                if (value != null)
                {
                    comboBoxType.SelectedItem = value.ProjectHeadingItem.HeadingType;
                }
            }
        }

        private void comboBoxType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            OwningHeading.ProjectHeadingItem.HeadingType = (HeadingType)comboBoxType.SelectedValue;
        }
    }
}