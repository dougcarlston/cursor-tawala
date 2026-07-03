// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Tawala.Functions.Runtime.Private
{
    public abstract class CompositeParameterBase : ICompositeParameter
    {
        private const string xmlFormat = "<{0}>{1}</{0}>";

        #region ICompositeParameter Members

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            IParameterInfo parameterInfo = lookupParameterInfo();

            var sbItems = new StringBuilder();

            for (int i = 0; i < parameterInfo.Parameters.Count; ++i)
            {
                IParameterInfo subParam = parameterInfo.Parameters[i];
                PropertyInfo pi = GetType().GetProperty(subParam.PropertyName);
                object value = pi.GetValue(this, null);

                string xml = string.Empty;

                if (value is IFunctionParameterXml)
                {
                    xml = ((IFunctionParameterXml)value).ToFunctionParameterXml();
                }
                else if (value != null)
                {
                    xml = value.ToString();
                }

                if (xml != string.Empty)
                {
                    sbItems.AppendFormat(xmlFormat, subParam.Id, xml);
                }
            }

            return sbItems.ToString();
        }

        public string ToFunctionParameterXml()
        {
            return ToString();
        }

        #endregion

        protected virtual void NotifyChanged(string name) // generated code overrides but still calls base
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private IParameterInfo lookupParameterInfo()
        {
            IFunctionInfo functionInfo = FunctionLoader.Current.Functions[GetType().DeclaringType];

            foreach (IParameterInfo pi in functionInfo.Parameters)
            {
                if (pi.PropertyType == GetType())
                {
                    return pi;
                }

                if (typeof(ICompositeParameterCollection).IsAssignableFrom(pi.PropertyType))
                {
                    if (pi.PropertyType.BaseType.GetGenericArguments()[0] == GetType())
                    {
                        return pi;
                    }
                }
            }

            return null;
        }
    }
}