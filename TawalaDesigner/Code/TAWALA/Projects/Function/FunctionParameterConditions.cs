// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using Tawala.Functions.Runtime;
using Tawala.Projects.Fields;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
    [Serializable]
    [DefaultBindingProperty("Conditions")]
    public class FunctionParameterConditions : IFunctionParameterValue, INotifyPropertyChanged
    {
        private Conditions conditions;

        public FunctionParameterConditions()
        {
            conditions = new Conditions();
        }

        public FunctionParameterConditions(IXmlElement element)
        {
            conditions = new Conditions(element, FieldUtil.GetGlobalFieldList(element));
        }

        public FunctionParameterConditions(Conditions c)
        {
            conditions = c;
        }

        public Conditions Conditions
        {
            get { return conditions; }
            set
            {
                if (conditions != value)
                {
                    conditions = value;
                    raiseNotifyChanged("Conditions");
                }
            }
        }

        #region IFunctionParameterValue Members

        public string ToValueXml()
        {
            return conditions != null ? conditions.ToValueXml() : string.Empty;
        }

        public string FormattedString { get { throw new NotImplementedException(); } }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public override string ToString()
        {
            return conditions == null ? string.Empty : conditions.ToString();
        }

        private void raiseNotifyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}