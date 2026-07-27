// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.Functions.Controls
{
    internal class ParameterComboBox : ComboBox, IParameterEditControl
    {
        public ParameterComboBox()
        {
            new DebugInitEventsBehavior(this);
            DropDownStyle = ComboBoxStyle.DropDownList;
            Margin = new Padding(3, 6, 3, 6);
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