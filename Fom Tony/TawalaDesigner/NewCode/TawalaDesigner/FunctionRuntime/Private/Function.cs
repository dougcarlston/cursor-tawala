// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Text;

namespace Tawala.Functions.Runtime.Private
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class Function : IFunction
    {
        #region IFunction Members

        public IFunctionInfo Info { get { return info; } }

        public int InstanceId { get { return instanceId; } }

        public bool HasValidParameterValues()
        {
            for (int i = 0; i < info.Parameters.Count; ++i)
            {
                if (!info.Parameters[i].HasValidValue(this))
                {
                    return false;
                }
            }

            return true;
        }

        public void SetValue(string id, object value)
        {
            info.Parameters[id].SetValue(this, value);
        }

        public object GetValue(string id)
        {
            return info.Parameters[id].GetValue(this);
        }

        public string ToXml()
        {
            return toXml(); // see below, don't add more here, add it to toXml(), but toXml() should remain simple!
        }

        public string ToDisplayString()
        {
            var displayString = new StringBuilder();

            displayString.AppendFormat("<<{0}(", info.Name);

            displayString.AppendFormat(info.Parameters.ToDisplayString(this));

            displayString.Append(")>>");
            displayString.Replace(", )>>", ")>>");

            return displayString.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // overridden in generated code for its own purposes but it calls base as well just in case
        public virtual void BeginEdit()
        {
        }

        public virtual void EndEdit()
        {
        }

        #endregion

        #region Protected methods for derived classes

        protected void NotifyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Private / Implementation

        private const string functionElementEndXml = "</{0}>";
        private const string functionElementStartXml = "<{0} version=\"{1}\">";
        private const string parameterElementXml = "<{0}>{1}</{0}>";
        private static int nextInstanceId = 0x21000;

        private readonly IFunctionInfo info;
        private readonly int instanceId = -1;

        protected Function()
        {
            instanceId = nextInstanceId++;

            info = FunctionLoader.Current.Functions[GetType()];
        }

        private string toXml()
        {
            var xmlString = new StringBuilder();

            IFunctionInfo functionInfo = Info;

            xmlString.AppendFormat(functionElementStartXml, functionInfo.Id, functionInfo.Version);

            appendStandardFunctionXml(xmlString);

            xmlString.AppendFormat(functionElementEndXml, functionInfo.Id);

            return xmlString.ToString();
        }

        private void appendStandardFunctionXml(StringBuilder xmlString)
        {
            foreach (string id in info.Parameters.Ids)
            {
                object parameterValue = info.Parameters[id].GetValue(this);

                string xml = string.Empty;

                if (parameterValue is ICompositeParameterCollection)
                {
                    xmlString.Append(((ICompositeParameterCollection)parameterValue).ToFunctionParameterXml());
                    continue;
                }

                if (parameterValue is IFunctionParameterXml)
                {
                    xml = ((IFunctionParameterXml)parameterValue).ToFunctionParameterXml();
                }
                else if (parameterValue != null)
                {
                    xml = parameterValue.ToString();
                }

                xmlString.AppendFormat(parameterElementXml, id, xml);
            }
        }

        #endregion

        public override string ToString() // Object override
        {
            return info.Name;
        }
    }
}