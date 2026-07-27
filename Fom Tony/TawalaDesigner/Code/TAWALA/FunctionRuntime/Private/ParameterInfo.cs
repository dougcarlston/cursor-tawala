// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Reflection;
using System.Xml.XPath;

namespace Tawala.Functions.Runtime.Private
{
    internal sealed class ParameterInfo : IParameterInfo
    {
        #region IParameterInfo Members

        public void SetValue(IFunction instance, object value)
        {
            if (PropertyType.IsArray || instance == null)
            {
                return; //can't handle it
            }

            setValue(instance, value);
        }

        public object GetValue(IFunction instance)
        {
            return getValue(instance);
        }

        public bool HasValidValue(IFunction instance)
        {
            return hasValidValue(instance);
        }

        public IParameterInfo Parent
        {
            get
            {
                return parentParameter;
            }
        }

        public IParameterInfoCollection Parameters
        {
            get
            {
                return children;
            }
        }

        public string Id
        {
            get
            {
                return navParam.GetAttribute("id", "");
            }
        }

        public string Name
        {
            get
            {
                return navParam.GetAttribute("name", "");
            }
        }

        public string Description
        {
            get
            {
                return navParam.SelectSingleNode("child::description").Value;
            }
        }

        public bool Required
        {
            get
            {
                string req = navParam.GetAttribute("required", "");
                return req != null && req.CompareTo("true") == 0;
            }
        }

        public IFunctionInfo FunctionInfo
        {
            get
            {
                return functionInfo;
            }
        }

        /// <summary>Name of parameter property on function</summary>
        public string PropertyName
        {
            get
            {
                return propertyInfo.Name;
            }
        }

        public Type PropertyType
        {
            get
            {
                return propertyInfo.PropertyType;
            }
        }

        public IFunctionParameterTypeInfo MapInfo
        {
            get
            {
                return FunctionParameterTypeMapper.Map[navParam.GetAttribute("type", "")];
            }
        }

        public ParameterRestrictions Restrictions
        {
            get
            {
                return restrictions;
            }
        }

        public string RecordListName
        {
            get
            {
                return recordListName;
            }
        }

        /// <summary>The parameter's xml definition stripped of namespaces</summary>
        public XPathNavigator Xml
        {
            get
            {
                return navParam.Clone();
            }
        }

        #endregion

        #region Private

        private readonly ParameterInfoCollection children;
        private readonly IFunctionInfo functionInfo;
        private readonly bool isComposite;
        private readonly bool isCompositeCollection;
        private readonly XPathNavigator navParam;
        private readonly ParameterPropertyAttribute parameterPropertyAttribute;
        private readonly IParameterInfo parentParameter;
        private readonly PropertyInfo propertyInfo;
        private string recordListName = string.Empty;
        private ParameterRestrictions restrictions = ParameterRestrictions.Default;

        internal ParameterInfo(IFunctionInfo fInfo, IParameterInfo pInfo, PropertyInfo pi, ParameterPropertyAttribute attr)
        {
            functionInfo = fInfo;
            parentParameter = pInfo;
            propertyInfo = pi;
            parameterPropertyAttribute = attr;

            isComposite = typeof(ICompositeParameter).IsAssignableFrom(propertyInfo.PropertyType);
            isCompositeCollection = typeof(ICompositeParameterCollection).IsAssignableFrom(propertyInfo.PropertyType);

            navParam = FunctionLoader.Repository.FindByGid(parameterPropertyAttribute.Gid);

            processRestrictions(navParam);

            children = new ParameterInfoCollection(fInfo, this, parameterPropertyAttribute.CompositeType);
        }

        private void setValue(IFunction instance, object value)
        {
            if (instance == null)
            {
                return;
            }

            if (parentParameter == null)
            {
                setParameterValue(instance, value);
            }
        }

        private void setParameterValue(IFunction instance, object value)
        {
            if (value == null && PropertyType.IsClass)
            {
                propertyInfo.SetValue(instance, null, null);
            }
            else if (PropertyType.IsAssignableFrom(value.GetType()))
            {
                propertyInfo.SetValue(instance, value, null);
            }
        }

        private object getValue(IFunction instance)
        {
            if (parentParameter == null)
            {
                return propertyInfo.GetValue(instance, null);
            }
            object o = parentParameter.GetValue(instance);
            return propertyInfo.GetValue(o, null);
        }

        private bool hasValidValue(IFunction instance)
        {
            if (parentParameter != null)
            {
                throw new InvalidOperationException(
                    "Only Top Level parameter is capable of correctly evaluating validity of parameter values");
            }

            if (instance == null)
            {
                return false;
            }

            object o = GetValue(instance);

            if (o == null)
            {
                if (!Required)
                {
                    return true;
                }

                return false;
            }

            if (Parameters.Count == 0)
            {
                return true;
            }

            if (isComposite)
            {
                return validateAutoComposite(o as ICompositeParameter);
            }

            if (isCompositeCollection)
            {
                return validateAutoCompositeCollection(o as ICompositeParameterCollection);
            }

            return false;
        }

        private bool validateAutoCompositeCollection(ICompositeParameterCollection collection)
        {
            foreach (ICompositeParameter composite in collection)
            {
                if (!validateAutoComposite(composite))
                {
                    return false;
                }
            }

            return true;
        }

        private bool validateAutoComposite(ICompositeParameter composite)
        {
            if (composite == null)
            {
                return false;
            }

            foreach (IParameterInfo subParam in Parameters)
            {
                if (subParam.Required)
                {
                    if (composite[subParam.Id] == null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void processRestrictions(XPathNavigator nav)
        {
            XPathNavigator worksWithinIteration = nav.SelectSingleNode("works-within-record-iteration");

            if (worksWithinIteration != null)
            {
                string when = worksWithinIteration.GetAttribute("when", "");
                recordListName = worksWithinIteration.GetAttribute("record-list-name", "");

                if (when.CompareTo("always") == 0)
                {
                    restrictions = ParameterRestrictions.RecordIterationAlways;
                }
                else if (when.CompareTo("never") == 0)
                {
                    restrictions = ParameterRestrictions.RecordIterationNever;
                }
            }
        }

        #endregion
    }
}