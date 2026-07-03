// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text;
using System.Xml.XPath;
using Tawala.Functions.Runtime.Private;
using Tawala.Functions.Runtime.Properties;

namespace Tawala.Functions.Runtime.CodeGen
{
    internal sealed class ParameterPropertyCreator : Creator
    {
        private readonly FunctionClassCreator classCreator;
        private readonly XPathNavigator navParameter;
        private string fieldName;

        private string id;
        private string propName;
        private StringBuilder sbProperty;
        private string typeName;
        private string xmltype;

        public ParameterPropertyCreator(FunctionClassCreator classCreator, XPathNavigator navParameter)
        {
            this.classCreator = classCreator;
            this.navParameter = navParameter;
        }

        public void Create(StringBuilder sbAppendTo)
        {
            id = navParameter.GetAttribute("id", "");
            xmltype = navParameter.GetAttribute("type", "");

            typeName = FunctionParameterTypeMapper.GetObjectTypeFromXmlType(xmltype);
            FunctionParameterTypeMapper.GetControlTypeFromXmlType(xmltype);

            propName = ToTitleCase(CleanIdentifier(id)).Replace("_", "");
            fieldName = char.ToLowerInvariant(propName[0]) + propName.Substring(1);

            createProperty();

            sbAppendTo.AppendLine(sbProperty.ToString());
        }

        private void createProperty()
        {
            if (navParameter.Select("child::parameter").Count == 0)
            {
                createSimpleProperty();
            }
            else
            {
                createAutoCompositeProperty();
            }
        }

        private void createSimpleProperty()
        {
            sbProperty = new StringBuilder(Resources.PARAMETER);
            sbProperty.Replace("${TYPE}", typeName).Replace("${GID}", navParameter.GetAttribute("gid", "")).Replace("${FNAME}", fieldName).
                Replace("${PNAME}", propName);

            sbProperty.Replace("${FASSIGN}", initializeDefault());
        }

        private void createAutoCompositeProperty()
        {
            sbProperty = new StringBuilder(Resources.COMPOSITE_PARAMETER);
            sbProperty.Replace("${FNAME}", fieldName).Replace("${GID}", navParameter.GetAttribute("gid", "")).Replace("${PNAME}", propName).
                Replace("${CCID}", id);

            var sbComposite = new StringBuilder();

            XPathNodeIterator iterator = navParameter.Select("child::parameter");
            while (iterator.MoveNext())
            {
                XPathNavigator navSubParameter = iterator.Current.Clone();
                var propertyCreater = new ParameterPropertyCreator(classCreator, navSubParameter);
                propertyCreater.Create(sbComposite);
            }

            sbProperty.Replace("${PROPERTIES}", sbComposite.ToString());
        }

        private string initializeDefault()
        {
            const string assignFormat = " = {0} ";

            IFunctionParameterTypeInfo info = FunctionParameterTypeMapper.Map[xmltype];

            if (info.Initializer != null)
            {
                return string.Format(assignFormat, info.Initializer);
            }

            if (xmltype == "enumeration")
            {
                XPathNavigator node = navParameter.SelectSingleNode("(child::choice/attribute::value)[1]");
                if (node != null)
                {
                    return string.Format(" = \"{0}\" ", node.Value);
                }
            }

            if (xmltype == "numeric-list")
            {
                XPathNavigator node = navParameter.SelectSingleNode("child::minimum/attribute::value");
                if (node != null)
                {
                    return string.Format(assignFormat, node.Value);
                }
            }

            return string.Empty;
        }
    }
}