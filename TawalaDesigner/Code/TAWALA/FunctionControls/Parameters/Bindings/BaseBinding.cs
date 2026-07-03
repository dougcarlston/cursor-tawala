// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.Functions.Controls
{
    internal abstract class BaseBinding : Binding
    {
        protected Type controlMemberType;
        protected IParameterBindingOwner owner;

        internal BaseBinding(IParameterControl control, string controlMember, IParameterBindingOwner owner)
            : base(controlMember, owner.DataSource, owner.DataMember, true)
        {
            this.owner = owner;
            controlMemberType = control.GetType().GetProperty(controlMember).PropertyType;
            NullValue = getControlNullValue();
            DataSourceNullValue = getDataNullValue();
        }

        private object getControlNullValue()
        {
            if (controlMemberType == typeof(string))
            {
                return string.Empty;
            }
            if (controlMemberType == typeof(int))
            {
                int i = 0;
                object o = i;
                return o;
            }
            return null;
        }

        private object getDataNullValue()
        {
            if (controlMemberType == typeof(string))
            {
                return owner.Required ? null : string.Empty;
                // if required, want function's value to be null on when control is empty so you can't press OK
            }
            return null;
        }

        protected override void OnFormat(ConvertEventArgs cevent)
        {
            owner.OnFormat(cevent);
            base.OnFormat(cevent);
        }

        protected override void OnParse(ConvertEventArgs cevent)
        {
            owner.OnParse(cevent);
            base.OnParse(cevent);
        }
    }
}