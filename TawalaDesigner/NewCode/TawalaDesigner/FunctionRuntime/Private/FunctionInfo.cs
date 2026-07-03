// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Reflection;
using System.Xml.XPath;

namespace Tawala.Functions.Runtime.Private
{
    internal class FunctionInfo : IFunctionInfo
    {
        #region IFunctionInfo Members

        public IFunction CreateInstance()
        {
            return MethodBase.GetMethodFromHandle(methodCreateHandle).Invoke(null, null) as IFunction;
        }

        public IParameterInfoCollection Parameters { get { return parameters; } }

        public string Id { get { return navFunction.GetAttribute("id", ""); } }

        public string Name { get { return navFunction.GetAttribute("name", ""); } }

        public string Description { get { return navFunction.SelectSingleNode("child::description").Value; } }

        public string Version { get { return navFunction.GetAttribute("version", ""); } }

        public Type Type { get { return Type.GetTypeFromHandle(typeHandle); } }

        public XPathNavigator Xml { get { return navFunction.Clone(); } }

        #endregion

        public override string ToString()
        {
            return Name;
        }

        #region Private

        private readonly RuntimeMethodHandle methodCreateHandle;
        private readonly XPathNavigator navFunction;
        private readonly ParameterInfoCollection parameters;
        private readonly RuntimeTypeHandle typeHandle;

        internal FunctionInfo(FunctionClassAttribute attr)
        {
            var fa = FunctionLoader.Current as FunctionRepository;
            navFunction = fa.FindByGid(attr.Gid);
            typeHandle = attr.FunctionType.TypeHandle;
            methodCreateHandle = attr.FunctionType.GetMethod("CreateInstance", BindingFlags.Public | BindingFlags.Static).MethodHandle;

            parameters = new ParameterInfoCollection(this, null, attr.FunctionType);
        }

        #endregion
    }
}