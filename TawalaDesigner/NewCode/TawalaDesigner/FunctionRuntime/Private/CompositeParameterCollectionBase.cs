// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace Tawala.Functions.Runtime.Private
{
    public abstract class CompositeParameterCollectionBase : Collection<ICompositeParameter>, ICompositeParameterCollection
    {
        #region ICompositeParameterCollection Members

        public override string ToString()
        {
            var sbXml = new StringBuilder();

            foreach (ICompositeParameter composite in this)
            {
                sbXml.Append(composite.ToString());
            }

            return sbXml.ToString();
        }

        public abstract ICompositeParameter CreateItem();
        public abstract IBindingList CreateBindingList();

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            NotifyChanged("Items");
        }

        protected override void InsertItem(int index, ICompositeParameter item)
        {
            base.InsertItem(index, item);
            NotifyChanged("Items");
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            NotifyChanged("Items");
        }

        protected override void SetItem(int index, ICompositeParameter item)
        {
            base.SetItem(index, item);
            NotifyChanged("Items");
        }

        #region ICompositeParameterCollection Members

        #endregion

        #region IFunctionParameterXml Members

        public abstract string ToFunctionParameterXml();

        #endregion
    }

    public abstract class CompositeParameterCollectionBase<T> : CompositeParameterCollectionBase, IList<T>
        where T : CompositeParameterBase, ICompositeParameter
    {
        #region IList<T> Members

        public int IndexOf(T item)
        {
            return base.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            base.Insert(index, item);
        }

        public new T this[int index] { get { return base[index] as T; } set { base[index] = value; } }

        public void Add(T item)
        {
            base.Add(item);
        }

        public bool Contains(T item)
        {
            return base.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool IsReadOnly { get { return false; } }

        public bool Remove(T item)
        {
            return base.Remove(item);
        }

        public new IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return this[i];
            }
        }

        #endregion
    }
}
