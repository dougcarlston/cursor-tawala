// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Xml.XPath;

namespace Tawala.Functions.Runtime.Private
{
    public abstract class FunctionRepository
    {
        #region Private

        protected readonly ICategoryInfoCollection categories = new CategoryInfoCollection();
        protected readonly IFunctionInfoCollection functions = new FunctionInfoCollection();

#pragma warning disable 649
        protected XPathNavigator navigator;
#pragma warning restore 649

        internal void Initialize()
        {
            ((CategoryInfoCollection)categories).Initialize();
            ((FunctionInfoCollection)functions).Initialize();

            categorizeFunctions();
        }

        private void categorizeFunctions()
        {
            foreach (IFunctionInfo functionInfo in functions)
            {
                XPathNodeIterator iterator = navigator.Select("//category[element-id = '" + functionInfo.Id + "']/attribute::name");

                while (iterator.MoveNext())
                {
                    categories[iterator.Current.Value].Functions.Add(functionInfo);
                }
            }

            foreach (ICategoryInfo categoryInfo in categories)
            {
                ((FunctionInfoCollection)categoryInfo.Functions).Sort();
            }
        }

        internal XPathNavigator FindByGid(string gid)
        {
            return navigator.SelectSingleNode(string.Format("//*[@gid='{0}']", gid));
        }

        #endregion
    }
}