// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tawala.Functions.Runtime.Private
{
    internal sealed class FunctionInfoCollection : Collection<IFunctionInfo>, IFunctionInfoCollection
    {
        private readonly Dictionary<string, IFunctionInfo> idToInfo = new Dictionary<string, IFunctionInfo>();
        private readonly Dictionary<RuntimeTypeHandle, IFunctionInfo> typeHandleToInfo = new Dictionary<RuntimeTypeHandle, IFunctionInfo>();

        #region IFunctionInfoCollection Members

        public IFunctionInfo this[string id] { get { return idToInfo[id]; } }

        public IFunctionInfo this[Type type] { get { return typeHandleToInfo[type.TypeHandle]; } }

        public bool Contains(string id)
        {
            return idToInfo.ContainsKey(id);
        }

        public bool Contains(Type type)
        {
            return typeHandleToInfo.ContainsKey(type.TypeHandle);
        }

        #endregion

        internal void Initialize()
        {
            object[] attributes = FunctionLoader.Current.GeneratedAssembly.GetCustomAttributes(typeof(FunctionClassAttribute), false);
            var functionAttrs = new FunctionClassAttribute[attributes.Length];
            Array.Copy(attributes, functionAttrs, attributes.Length);

            for (int i = 0; i < functionAttrs.Length; ++i)
            {
                Add(new FunctionInfo(functionAttrs[i]));
            }

            Sort();
        }

        internal void Sort()
        {
            var list = new List<IFunctionInfo>(this);
            list.Sort(compareFunctionInfoByName);
            Clear();
            for (int i = 0; i < list.Count; ++i)
            {
                Add(list[i]);
            }
        }

        internal void Add(FunctionInfoCollection fic)
        {
            for (int i = 0; i < fic.Count; ++i)
            {
                Add(fic[i]);
            }
        }

        protected override void InsertItem(int index, IFunctionInfo item)
        {
            base.InsertItem(index, item);
            idToInfo.Add(item.Id, item);
            typeHandleToInfo.Add(item.Type.TypeHandle, item);
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            idToInfo.Clear();
            typeHandleToInfo.Clear();
        }

        private static int compareFunctionInfoByName(IFunctionInfo info1, IFunctionInfo info2)
        {
            return info1.Name.CompareTo(info2.Name);
        }
    }
}