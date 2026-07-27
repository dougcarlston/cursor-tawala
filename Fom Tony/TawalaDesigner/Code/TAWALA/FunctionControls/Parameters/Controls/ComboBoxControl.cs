// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.Functions.Controls
{
    internal class ComboBoxControl : ComboBox, IParameterControl
    {
        public ComboBoxControl()
        {
            DropDownStyle = ComboBoxStyle.DropDownList;
            Margin = new Padding(3, 6, 3, 6);
            TabStop = true;
            new HighlightBehavior(this);
            FlatStyle = FlatStyle.Flat;
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