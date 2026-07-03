// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Forms;

namespace Tawala.Controls
{
    [Browsable(false)]
    public class FormCheckBox : CheckBox
    {
        public IForm form = NullObjects.Form;

        public FormCheckBox()
        {
            Checked = true;
        }

        public FormCheckBox(IForm form) : this()
        {
            this.form = form;
            Text = form.Name;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public IForm Form
        {
            get { return form; }
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // FormCheckBox
            // 
            ResumeLayout(false);
        }
    }
}