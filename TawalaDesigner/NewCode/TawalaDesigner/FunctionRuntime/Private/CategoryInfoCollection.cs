// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.XPath;

namespace Tawala.Functions.Runtime.Private
{
    internal sealed class CategoryInfoCollection : Collection<ICategoryInfo>, ICategoryInfoCollection
    {
        private readonly Dictionary<string, ICategoryInfo> nameToInfo = new Dictionary<string, ICategoryInfo>();

        #region ICategoryInfoCollection Members

        public ICategoryInfo this[string name] { get { return nameToInfo[name]; } }

        public bool Contains(string name)
        {
            return nameToInfo.ContainsKey(name);
        }

        #endregion

        internal void Initialize()
        {
            XPathExpression expression = XPathExpression.Compile("descendant::category");
            expression.AddSort("@name", StringComparer.Ordinal);
            XPathNodeIterator iterator = FunctionLoader.Current.Xml.Select(expression);

            while (iterator.MoveNext())
            {
                Add(new CategoryInfo(iterator.Current.Clone()));
            }
        }

        protected override void InsertItem(int index, ICategoryInfo item)
        {
            base.InsertItem(index, item);
            nameToInfo.Add(item.Name, item);
        }
    }
}