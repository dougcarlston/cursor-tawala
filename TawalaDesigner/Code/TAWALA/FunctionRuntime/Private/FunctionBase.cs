// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Text;

namespace Tawala.Functions.Runtime.Private
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class FunctionBase : IFunction
    {
        protected IPropertyCollection Properties { get; set; }

        #region IFunction Members

        public abstract IFunctionInfo Info { get; }

        public int InstanceId { get { return instanceId; } }

        public bool HasValidParameterValues()
        {
            for (int i = 0; i < Info.Parameters.Count; ++i)
            {
                if (!Info.Parameters[i].HasValidValue(this))
                {
                    return false;
                }
            }

            return true;
        }

        public void SetValue(string id, object value)
        {
            Info.Parameters[id].SetValue(this, value);
        }

        public object GetValue(string id)
        {
            return Info.Parameters[id].GetValue(this);
        }

        public string ToXml()
        {
            return toXml(); // see below, don't add more here, add it to toXml(), but toXml() should remain simple!
        }

        public string ToDisplayString()
        {
            var displayString = new StringBuilder();

            displayString.AppendFormat("<<{0}(", Info.Name);

            displayString.AppendFormat(Info.Parameters.ToDisplayString(this));

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

        public override string ToString()
        {
            return Info.Name;
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

        private readonly int instanceId = -1;

        protected FunctionBase()
        {
            instanceId = nextInstanceId++;
        }

        private string toXml()
        {
            var xmlString = new StringBuilder();

            xmlString.AppendFormat(functionElementStartXml, Info.Id, Info.Version);

            appendStandardFunctionXml(xmlString);

            xmlString.AppendFormat(functionElementEndXml, Info.Id);

            return xmlString.ToString();
        }

        private void appendStandardFunctionXml(StringBuilder xmlString)
        {
            foreach (string id in Info.Parameters.Ids)
            {
                object parameterValue = Info.Parameters[id].GetValue(this);

                string xml = string.Empty;

                if (parameterValue is ICompositeParameterCollection)
                {
                    xmlString.Append(((IFunctionParameterValue)parameterValue).ToValueXml());
                    continue;
                }

                if (parameterValue is IFunctionParameterValue)
                {
                    xml = ((IFunctionParameterValue)parameterValue).ToValueXml();
                }
                else if (parameterValue != null)
                {
                    xml = parameterValue.ToString();
                }

                xmlString.AppendFormat(parameterElementXml, id, xml);
            }
        }

        #endregion

        #region IPropertyCollection

        public object this[int index] { get { return Properties[index]; } set { Properties[index] = value; } }

        public object this[string id] { get { return Properties[id]; } set { Properties[id] = value; } }

        public object this[IParameterInfo info] { get { return Properties[info]; } set { Properties[info] = value; } }

        public int PropertyCount { get { return Properties.PropertyCount; } }

        #endregion
    }
}