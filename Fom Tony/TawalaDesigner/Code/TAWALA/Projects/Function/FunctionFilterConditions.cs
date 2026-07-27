// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Text;
using Tawala.Functions.Runtime;
using Tawala.Projects.Fields;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
    [Serializable]
    public class FunctionFilterConditions : IFunctionParameterValue, INotifyPropertyChanged
    {
        private Conditions conditions;
        private FunctionFormCollection forms = new FunctionFormCollection();

        public FunctionFilterConditions()
        {
        }

        public FunctionFilterConditions(IXmlElement element)
        {
            // the outtermost node of the element is function-and-parameter specific;
            // we're only interested in the child elements
            if (element.HasChild("form"))
            {
                forms = new FunctionFormCollection(element.GetChildren("form"));
            }

            if (element.HasChild("conditions"))
            {
                IXmlElement conditionsElement = element.GetChild("conditions");

                conditions = new Conditions(conditionsElement, FieldUtil.GetGlobalFieldList(conditionsElement));
            }
        }

        public FunctionFormCollection Forms
        {
            get { return forms; }
            set
            {
                if (forms != value)
                {
                    forms = value;
                    raiseNotifyChanged("Forms");
                }
            }
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
            var xmlString = new StringBuilder();

            xmlString.AppendFormat(forms.ToValueXml());

            if (conditions != null)
            {
                xmlString.Append(conditions.ToXml());
            }

            return xmlString.ToString();
        }

        public string FormattedString { get { throw new NotImplementedException(); } }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public string ToXml()
        {
            var xmlString = new StringBuilder();

            xmlString.AppendFormat(forms.ToXml());

            if (conditions != null)
            {
                xmlString.Append(conditions.ToXml());
            }

            return xmlString.ToString();
        }

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