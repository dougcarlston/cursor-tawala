// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Tawala.Functions.Runtime;

namespace Tawala.Functions.Controls
{
    public abstract class ParameterBinder : IParameterBinder, IParameterBindingOwner
    {
        protected IFunction function;
        protected IParameterInfo parameterInfo;
        protected Source source;

        protected ParameterBinder(IFunction function, IParameterInfo parameterInfo)
        {
            this.function = function;
            this.parameterInfo = parameterInfo;
            source = new Source(this);
        }

        #region IParameterBinder Members

        public void SetLabels(Label name, Label description)
        {
            name.Text = parameterInfo.Name;
            description.Text = parameterInfo.Description;
        }

        public void SetSingleLineEditLabels(Label name, Label tagLine)
        {
            name.Text = (parameterInfo.Required ? "*&" : "&") + parameterInfo.Name + ":  ";
            tagLine.Text = parameterInfo.MapInfo.TagLine;
        }

        public void Initialize(IParameterControl control)
        {
            object curVal = parameterInfo.GetValue(function);
            object newVal = RangeCheck(curVal);
            if (curVal != newVal)
            {
                parameterInfo.SetValue(function, newVal);
            }

            Bind(control);
        }

        #endregion

        #region IParameterBindingOwner Members

        public virtual void OnBindingComplete(BindingCompleteEventArgs bce)
        {
        }

        public virtual void OnFormat(ConvertEventArgs ce)
        {
        }

        public virtual void OnParse(ConvertEventArgs ce)
        {
        }

        public object DataSource { get { return source; } }

        public string DataMember { get { return string.IsNullOrEmpty(source.DataMember) ? parameterInfo.PropertyName : string.Empty; } }

        public bool Required
        {
            get { return parameterInfo.Required; } // only acknowledged by BaseBinding when data is text
        }

        #endregion

        protected abstract void Bind(IParameterControl c);

        protected virtual void OnDataError(BindingManagerDataErrorEventArgs e)
        {
            Debugger.Break();
        }

        protected virtual object RangeCheck(object o)
        {
            return o;
        }

        #region Source (BindingSource derived nested class)

        public sealed class Source : BindingSource
        {
            private readonly ParameterBinder binder;

            internal Source(ParameterBinder binder)
            {
                this.binder = binder;
                AllowNew = false;
                DataSource = binder.function;
            }

            internal Source(ParameterBinder binder, IBindingList list)
            {
                this.binder = binder;
                AllowNew = true;
                DataSource = list;
            }

            protected override void OnDataError(BindingManagerDataErrorEventArgs e)
            {
                binder.OnDataError(e);
                base.OnDataError(e);
            }
        }

        #endregion
    }
}