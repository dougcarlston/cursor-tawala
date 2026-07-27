// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Xml.XPath;

namespace Tawala.Functions.Runtime.Private
{
    internal sealed class CategoryInfo : ICategoryInfo
    {
        #region ICategoryInfo Members

        public string Name { get { return name; } }

        public IFunctionInfoCollection Functions { get { return functions; } }

        public XPathNavigator Xml { get { return navCategory.Clone(); } }

        #endregion

        public override string ToString()
        {
            return Name;
        }

        #region PRIVATE

        private readonly FunctionInfoCollection functions = new FunctionInfoCollection();
        private readonly string name;
        private readonly XPathNavigator navCategory;

        internal CategoryInfo(XPathNavigator _navCategory)
        {
            navCategory = _navCategory;
            name = navCategory.GetAttribute("name", "");
        }

        #endregion
    }
}