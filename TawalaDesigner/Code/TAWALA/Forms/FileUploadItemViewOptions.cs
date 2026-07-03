// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Forms
{
    public partial class FileUploadItemViewOptions : OptionsView
    {
        public FileUploadItemViewOptions()
        {
            InitializeComponent();
        }

        internal FileUploadItemView CurrentOwner
        {
            get { return owner as FileUploadItemView; }
            set
            {
                SetOwner(value);

                if (value != null)
                {
                    checkBoxRequired.Checked = CurrentOwner.FileUploadItem.Required;
                }
            }
        }

        public bool Required { get { return checkBoxRequired.Checked; } set { checkBoxRequired.Checked = value; } }

        private void checkBoxRequired_CheckedChanged(object sender, EventArgs e)
        {
            if (Parent != null)
            {
                CurrentOwner.UpdateRequiredFlag();
            }
        }
    }
}