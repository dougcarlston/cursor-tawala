// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Reflection;
using System.Windows.Forms;

namespace Tawala.Functions.Controls
{
    internal class EditBinding : BaseBinding
    {
        internal EditBinding(IParameterControl c, string controlMember, IParameterBindingOwner owner) : base(c, controlMember, owner)
        {
            bool text = isText(c, controlMember);
            ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;
            DataSourceUpdateMode = text ? DataSourceUpdateMode.OnValidation : DataSourceUpdateMode.OnPropertyChanged;
            c.DataBindings.Add(this);
        }

        private static bool isText(IParameterControl c, string propName)
        {
            PropertyInfo pi = c.GetType().GetProperty(propName);
            return pi != null && pi.PropertyType == typeof(string);
        }
    }
}