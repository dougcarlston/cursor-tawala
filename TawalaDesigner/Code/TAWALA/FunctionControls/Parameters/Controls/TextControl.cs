// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.Functions.Controls
{
    internal class TextControl : TextBox, IParameterControl
    {
        public TextControl()
        {
            AllowDrop = false;
            ReadOnly = false;
            new HighlightBehavior(this);
        }

        #region IParameterControl Members

        public bool CustomControl
        {
            get
            {
                return false;
            }
        }

        public void CommitPendingChanges()
        {
        }

        #endregion
    }
}