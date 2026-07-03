// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text;
using System.Xml.XPath;
using Tawala.Functions.Runtime.Properties;

namespace Tawala.Functions.Runtime.CodeGen
{
    internal sealed class FunctionClassCreator : Creator
    {
        private readonly FunctionAssemblyCreator assemblyCreator;
        private readonly XPathNavigator functionRoot;
        private readonly string nameSpace;
        private readonly StringBuilder sbFunctionDef = new StringBuilder();

        public FunctionClassCreator(FunctionAssemblyCreator _assemblyCreator, XPathNavigator _navComponent,
                                    string _nameSpace)
        {
            assemblyCreator = _assemblyCreator;
            functionRoot = _navComponent;
            nameSpace = _nameSpace;
        }

        public StringBuilder Create()
        {
            sbFunctionDef.Append(Resources.CODE_PREAMBLE_TEMPLATE);
            sbFunctionDef.Replace("${YEAR}", DateTime.Now.Year.ToString());
            sbFunctionDef.Replace("${NAMESPACE}", nameSpace);

            sbFunctionDef.Append(Resources.FUNCTION_TEMPLATE);

            string version = getVersion();
            string gid = functionRoot.GetAttribute("gid", "");
            string classnameV = string.Format("{0}_V{1}", makeClassName(functionRoot), version);
            sbFunctionDef.Replace("${CLASSNAMEV}", classnameV);

            addParameterProperties();

            assemblyCreator.AddFunction(gid, classnameV);
            return sbFunctionDef;
        }

        private string getVersion()
        {
            string version = functionRoot.GetAttribute("version", "");
            if (string.IsNullOrEmpty(version))
            {
                return "0";
            }
            return version;
        }

        private void addParameterProperties()
        {
            var sbParameters = new StringBuilder();

            XPathNodeIterator iterator = functionRoot.Select("child::parameter");
            while (iterator.MoveNext())
            {
                XPathNavigator navParameter = iterator.Current.Clone();
                var propertyCreater = new ParameterPropertyCreator(this, navParameter);
                propertyCreater.Create(sbParameters);
            }

            sbFunctionDef.Replace("${PROPERTIES}", sbParameters.ToString());
        }
    }
}