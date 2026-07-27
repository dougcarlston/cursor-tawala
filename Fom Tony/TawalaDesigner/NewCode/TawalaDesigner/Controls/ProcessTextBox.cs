// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.Controls
{
    public class ProcessTextBox : TextBox
    {
        /// <summary>
        /// Indicates whether this text box accepts drag/drop of the specified data.
        /// </summary>
        public virtual bool AcceptsDropOf(IDataObject data)
        {
            return false;
        }
    }
}