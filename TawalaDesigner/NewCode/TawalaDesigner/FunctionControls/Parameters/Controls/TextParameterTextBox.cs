// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.Functions.Controls
{
    internal class TextParameterTextBox : TextBox, IParameterEditControl
    {
        public TextParameterTextBox()
        {
            new DebugInitEventsBehavior(this);
            AllowDrop = false;
            ReadOnly = false;
            new HighlightBehavior(this);
        }

        #region IParameterEditControl Members

        public void CommitPendingChanges()
        {
        }

        public Control GetControl()
        {
            return this;
        }

        #endregion
    }
}