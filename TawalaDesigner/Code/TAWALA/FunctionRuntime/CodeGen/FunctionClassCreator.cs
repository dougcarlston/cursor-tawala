// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
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
        private readonly Dictionary<string, string> replaceDictionary = new Dictionary<string, string>();

        public FunctionClassCreator(FunctionAssemblyCreator _assemblyCreator, XPathNavigator _navComponent, string _nameSpace)
        {
            assemblyCreator = _assemblyCreator;
            functionRoot = _navComponent;
            nameSpace = _nameSpace;
        }

        public void Create()
        {
            string version = getVersion();
            string gid = functionRoot.GetAttribute("gid", "");
            string classnameV = string.Format("{0}_V{1}", makeClassName(functionRoot), version);

            replaceDictionary.Add("${YEAR}", DateTime.Now.Year.ToString());
            replaceDictionary.Add("${NAMESPACE}", nameSpace);
            replaceDictionary.Add("${CLASSNAMEV}", classnameV);
            replaceDictionary.Add("${GID}", gid);
            replaceDictionary.Add("${FID}", functionRoot.GetAttribute("id", ""));
            replaceDictionary.Add("${FNAME}", functionRoot.GetAttribute("name", ""));
            replaceDictionary.Add("${FVERSION}", version);
            addParameterProperties();

            var sbFunction = new StringBuilder(Resources.CODE_PREAMBLE);
            sbFunction.Append(Resources.FUNCTION);
            applyReplacements(sbFunction);
            assemblyCreator.AddFunction(gid, classnameV, sbFunction);

            var sbFunctionInfo = new StringBuilder(Resources.CODE_PREAMBLE);
            sbFunctionInfo.Append(Resources.FUNCTION_INFO);
            applyReplacements(sbFunctionInfo);
            assemblyCreator.AddFunctionInfo(sbFunctionInfo);
        }

        private void applyReplacements(StringBuilder sb)
        {
            foreach (string key in replaceDictionary.Keys)
            {
                sb.Replace(key, replaceDictionary[key]);
            }
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

            replaceDictionary.Add("${PROPERTIES}", sbParameters.ToString());
        }
    }
}