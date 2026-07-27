// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.Forms
{
    public partial class OptionsView : UserControl
    {
        private bool inForceLayout;
        protected ItemViewBase owner;

        public OptionsView()
        {
            InitializeComponent();
        }

        public void SetError(Control child, string text)
        {
            errorProvider.SetIconAlignment(child, ErrorIconAlignment.MiddleRight);
            errorProvider.SetError(child, text);
        }

        protected void SetOwner(ItemViewBase itemViewBase)
        {
            ItemViewBase priorOwner = owner;
            owner = itemViewBase;

            if (priorOwner != null)
            {
                priorOwner.SuspendLayout();
                priorOwner.Controls.Remove(this);
                priorOwner.ResumeLayout(false);
                forceLayoutAll(priorOwner);
            }

            if (itemViewBase != null && !itemViewBase.Controls.Contains(this))
            {
                itemViewBase.SuspendLayout();
                itemViewBase.Controls.Add(this);
                itemViewBase.ResumeLayout(false);
                forceLayoutAll(itemViewBase);
            }
        }

        private void forceLayoutAll(ItemViewBase itemViewBase)
        {
            if (itemViewBase.Parent != null && itemViewBase.Parent.Parent != null)
            {
                if (!inForceLayout)
                {
                    try
                    {
                        inForceLayout = true;
                        itemViewBase.Parent.PerformLayout();
                    }
                    finally
                    {
                        inForceLayout = false;
                    }
                }
            }
        }
    }
}