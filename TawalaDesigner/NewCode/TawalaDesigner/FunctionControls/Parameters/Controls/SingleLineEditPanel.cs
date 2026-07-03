// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.Functions.Controls
{
    internal partial class SingleLineEditPanel : UserControl
    {
        private const int pad = 14;
        private readonly Control edit;

        public SingleLineEditPanel(IParameterControl pc)
        {
            new DebugInitEventsBehavior(this);

            edit = pc.GetControl();
            InitializeComponent();

            edit.Padding = new Padding(0);
            edit.Margin = new Padding(0);
            edit.TabStop = true;
            edit.TabIndex = 2;
            Controls.Add(edit);
            Controls.SetChildIndex(edit, 1);

            TabStop = true;

            PerformLayout();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            IParameterBinder binder = ParameterControlManager.LookupBinder(edit as IParameterControl);
            binder.SetSingleLineEditLabels(labelName, labelTag);
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            double dwidth = ClientSize.Width - pad;

            int height = ClientSize.Height;

            labelName.Left = 0;
            labelName.Width = (int)(dwidth*0.3);

            edit.Left = labelName.Right + pad/2;
            edit.Width = (int)(dwidth*0.4);

            labelTag.Left = edit.Right + pad/2;
            labelTag.Width = (int)(dwidth*0.3);

            edit.Top = (height - edit.Height)/2;
        }
    }
}