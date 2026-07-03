// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.Functions.Controls
{
    internal class ViewBinding : BaseBinding
    {
        internal ViewBinding(IParameterControl c, string controlMember, IParameterBindingOwner owner) : base(c, controlMember, owner)
        {
            ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;
            DataSourceUpdateMode = DataSourceUpdateMode.Never;
            c.DataBindings.Add(this);
        }
    }
}