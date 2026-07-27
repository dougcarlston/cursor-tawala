// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using Tawala.Projects.Forms;

namespace Tawala.Projects
{
    /// <summary>
    /// Maintains a collection of FormItems.
    /// If it is owned by a Form it will fire events off of Project.Events --
    /// the FormItemAdded and FormItemRemoved events
    /// </summary>
    [Serializable]
    public class FormItemList : Collection<IFormItem>, IFormItem
    {
        private const string xmlItemsEndTag = "</items>\r\n";
        private const string xmlItemsStartTag = "<items>\r\n";
        private readonly IForm form;
        private FormItemMap map;

        public FormItemList()
        {
            map = new FormItemMap(this);
        }

        public FormItemList(IForm form)
        {
            this.form = form;
            map = new FormItemMap(this);
        }

        #region IFormItem Members

        public string QualifiedFieldName { get { throw new NotImplementedException(); } }

        public string ToXml()
        {
            var xmlString = new StringBuilder();

            if (Count > 0)
            {
                xmlString.Append(xmlItemsStartTag);

                foreach (IFormItem formItem in Items)
                {
                    var defaultLabel = formItem as IDefaultLabel;

                    if (defaultLabel != null)
                    {
                        xmlString.Append(defaultLabel.ToXml(map[formItem]));
                    }
                    else
                    {
                        xmlString.Append(formItem.ToXml());
                    }
                }

                xmlString.Append(xmlItemsEndTag);
            }

            return xmlString.ToString();
        }

        public IForm Form { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public bool IsTextItem { get { throw new NotImplementedException(); } }

        public bool IsQuestionItem { get { throw new NotImplementedException(); } }

        public string AlternateLabel { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public bool Selected { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public void Eliminate()
        {
            throw new NotImplementedException();
        }

        public string Style { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public Conditions DisplayConditions { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public bool HasDisplayConditions { get { throw new NotImplementedException(); } }

        public void ClearId()
        {
            throw new NotImplementedException();
        }

        public void ResolveFieldReferences()
        {
            foreach (IFormItem item in this)
            {
                item.ResolveFieldReferences();
            }
        }

        public void ResolveFunctionReferences()
        {
            foreach (IFormItem item in this)
            {
                item.ResolveFunctionReferences();
            }
        }

        #endregion

        public new bool Contains(IFormItem item)
        {
            return map.Contains(item);
        }

        public bool Contains(int itemId)
        {
            return map.Contains(itemId);
        }

        public IFormItem GetItemById(int itemId)
        {
            return map[itemId];
        }

        public string GetDefaultLabel(IFormItem item)
        {
            return map[item];
        }

        /// <summary>
        /// handles pasting of Form Items
        /// to insure that duplicate Alternate Labels are not generated
        /// </summary>
        public void Paste(int index, IFormItem item)
        {
            if (!string.IsNullOrEmpty(item.AlternateLabel))
            {
                string newName = item.AlternateLabel;

                // check existing question/text labels
                for (int count = 1; ContainsAlternateLabel(newName); ++count)
                {
                    // if match was found, create new label with numeric suffix
                    newName = item.AlternateLabel + count;
                }

                // if that's not already the item's label
                if (newName.CompareTo(item.AlternateLabel) != 0)
                {
                    // assign it
                    item.AlternateLabel = newName;
                }
            }

            if (item is FibItem)
            {
                // follow the same procedure for blanks in FIBs
                var fibItem = item as FibItem;

                foreach (Blank b in fibItem.BlankList)
                {
                    if (!string.IsNullOrEmpty(b.AlternateLabel))
                    {
                        string newName = b.AlternateLabel;
                        for (int count = 1; ContainsAlternateLabel(newName); ++count)
                        {
                            newName = b.AlternateLabel + count;
                        }

                        if (newName.CompareTo(b.AlternateLabel) != 0)
                        {
                            b.AlternateLabel = newName;
                        }
                    }
                }
            }

            Insert(index, item);
        }

        public void Move(IFormItem item, int targetIndex)
        {
            // if dropping at end of list...
            if (targetIndex >= Count)
            {
                // move item to end of list
                Remove(item);
                Add(item);
            }
            else
            {
                if (IndexOf(item) != targetIndex)
                {
                    IFormItem targetItem = this[targetIndex];
                    Remove(item);

                    int newTargetIndex = IndexOf(targetItem);
                    Insert(newTargetIndex, item);
                }
            }
        }

        /// <summary>
        /// returns true if the passed name is already in use as an Alternate Label in this list
        /// </summary>
        public bool ContainsAlternateLabel(string name)
        {
            foreach (IFormItem item in this)
            {
                // check question/text labels
                if (name.CompareTo(item.AlternateLabel) == 0)
                {
                    return true;
                }

                if (item is IFibItem)
                {
                    var fibItem = item as IFibItem;

                    // and blank labels
                    foreach (IBlank b in fibItem.BlankList)
                    {
                        if (b.AlternateLabel.CompareTo(name) == 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        protected override void ClearItems()
        {
            foreach (IFormItem item in Items)
            {
                item.Eliminate();
                item.Form = null;
            }
            base.ClearItems();

            map = new FormItemMap(this);

            Project.Events.RaiseFormChangedEvent(new ComponentEventArgs(form));
        }

        protected override void InsertItem(int index, IFormItem item)
        {
            base.InsertItem(index, item);
            map.Inserted(item, index);

            if (item.Form != form)
            {
                item.Form = form;
            }

            if (form != null)
            {
                Project.Events.RaiseFormItemAddedEvent(new FormItemEventArgs(form, item, index));
                Project.Events.RaiseFormChangedEvent(new ComponentEventArgs(form));
            }
        }

        protected override void SetItem(int index, IFormItem newItem)
        {
            IFormItem oldItem = this[index];
            base.SetItem(index, newItem);
            map.SetItem(oldItem, newItem);
        }

        protected override void RemoveItem(int index)
        {
            IFormItem item = this[index];
            item.Form = null;

            base.RemoveItem(index);

            map.Removed(item, index);

            if (form != null)
            {
                Project.Events.RaiseFormItemRemovedEvent(new FormItemEventArgs(form, item, index));
                Project.Events.RaiseFormChangedEvent(new ComponentEventArgs(form));
            }
        }

        /// <summary>
        /// determines if a label is valid and, if so, sets it in the passed Form item
        /// </summary>
        /// <param name="item">the Form item</param>
        /// <param name="blank">a blank if test a label for a FIBItem blank, otherwise null</param>
        /// <param name="label">the proposed label</param>
        /// <returns>
        /// true if the label was valid and therefore set in the item
        /// </returns>
        public bool ValidAlternateLabel(IFormItem item, Blank blank, string label)
        {
            Debug.Assert((blank == null) || (blank != null && (item is IFibItem)));

            bool validLabel = true;

            // strip off leading and trailing whitespace because existing labels won't have them
            string testLabel = label.Trim();

            // first, make sure that the proposed label is not a duplicate in this list
            if (Contains(item))
            {
                foreach (IFormItem compItem in this)
                {
                    if (compItem != item && compItem.AlternateLabel.Length > 0 && compItem.AlternateLabel == testLabel)
                    {
                        // if a match is found, we can't allow a duplicate
                        validLabel = false;
                        break;
                    }

                    var fibItem = compItem as IFibItem;
                    if (fibItem != null)
                    {
                        // check blanks' labels, too
                        foreach (IBlank compBlank in fibItem.BlankList)
                        {
                            if (compBlank != blank && compBlank.AlternateLabel.Length > 0 && compBlank.AlternateLabel == testLabel)
                            {
                                validLabel = false;
                                break;
                            }
                        }
                    }

                    if (validLabel == false)
                    {
                        break;
                    }
                }
            }

            // now check for illegal name formats
            if (validLabel && testLabel.Length > 0)
            {
                //validLabel = Project.Current.ValidLabelFormat(testLabel);
                validLabel = Project.Current.ValidFieldLabelFormat(testLabel);
            }

            return validLabel;
        }

        #region IField Interface

        private readonly int id = Project.NextUniqueID;

        public string FieldName { get { return ""; } }

        public string FieldString { get { return ""; } }

        public IField this[string name]
        {
            get
            {
                foreach (IField field in RecursiveEnumerator)
                {
                    if (field.FieldName == name)
                    {
                        return field;
                    }
                }

                // field name not found
                return null;
            }
        }

        public int Id { get { return id; } }

        #endregion

        #region IRecursiveEnumerable Interface

        public IEnumerable RecursiveEnumerator
        {
            get
            {
                foreach (IFormItem item in Items)
                {
                    foreach (IField field in item.RecursiveEnumerator)
                    {
                        yield return field;
                    }
                }
            }
        }

        #endregion

        #region Nested type: FormItemMap

        [Serializable]
        private class FormItemMap
        {
            private static readonly Collection<string> defaultLabels = new Collection<string>();
            private readonly FormItemList formItemList;
            private readonly Dictionary<IFormItem, string> formItemToDefaultLabel = new Dictionary<IFormItem, string>();
            private readonly Dictionary<int, IFormItem> idToFormItem = new Dictionary<int, IFormItem>();
            private readonly int[] nextDefaultLabelNums = new int[defaultLabels.Count];

            static FormItemMap()
            {
                defaultLabels.Add("F");
                defaultLabels.Add("H");
                defaultLabels.Add("Q");
                defaultLabels.Add("T");
            }

            public FormItemMap(FormItemList list)
            {
                initializeDefaultLabelNums();

                formItemList = list;

                if (list.Count != 0)
                {
                    updateMap();
                }
            }

            public string this[IFormItem item] { get { return formItemToDefaultLabel.ContainsKey(item) ? formItemToDefaultLabel[item] : string.Empty; } }

            public IFormItem this[int id] { get { return idToFormItem.ContainsKey(id) ? idToFormItem[id] : null; } }

            private void initializeDefaultLabelNums()
            {
                for (int i = 0; i < nextDefaultLabelNums.Length; ++i)
                {
                    nextDefaultLabelNums[i] = 1;
                }
            }

            public void SetItem(IFormItem oldItem, IFormItem newItem)
            {
                if (oldItem != newItem)
                {
                    string label = string.Empty;

                    if (oldItem != null)
                    {
                        label = formItemToDefaultLabel[oldItem];
                        formItemToDefaultLabel.Remove(oldItem);
                        idToFormItem.Remove(oldItem.Id);
                    }

                    if (areSameDefaultLabelType(oldItem, newItem))
                    {
                        formItemToDefaultLabel[newItem] = label;
                        idToFormItem[newItem.Id] = newItem;
                    }
                    else
                    {
                        updateMap();
                    }
                }

                Debug.Assert(formItemList.Count == formItemToDefaultLabel.Keys.Count);
                Debug.Assert(formItemList.Count == idToFormItem.Keys.Count);
            }

            private static bool areSameDefaultLabelType(IFormItem oldItem, IFormItem newItem)
            {
                if (oldItem == null || newItem == null)
                {
                    return false;
                }

                var oldDefault = oldItem as IDefaultLabel;
                var newDefault = newItem as IDefaultLabel;

                if (oldDefault != null && newDefault != null)
                {
                    return oldDefault.DefaultLabelPrefix.CompareTo(newDefault.DefaultLabelPrefix) == 0;
                }

                return false;
            }

            public void Inserted(IFormItem item, int itemListIndex)
            {
                if (itemListIndex == formItemList.Count - 1 || !(item is IDefaultLabel))
                {
                    formItemToDefaultLabel[item] = getNextLabel(item);
                    idToFormItem[item.Id] = item;
                }
                else
                {
                    updateMap();
                }

                Debug.Assert(formItemList.Count == formItemToDefaultLabel.Keys.Count);
                Debug.Assert(formItemList.Count == idToFormItem.Keys.Count);
            }

            public void Removed(IFormItem item, int itemListIndex)
            {
                formItemToDefaultLabel.Remove(item);
                idToFormItem.Remove(item.Id);

                if (item is IDefaultLabel)
                {
                    if (itemListIndex < formItemList.Count)
                    {
                        updateMap();
                    }
                    else
                    {
                        var formItemWithDefaultLabel = item as IDefaultLabel;

                        int index = defaultLabels.IndexOf(formItemWithDefaultLabel.DefaultLabelPrefix);
                        nextDefaultLabelNums[index] -= 1;
                    }
                }
                Debug.Assert(formItemList.Count == formItemToDefaultLabel.Keys.Count);
                Debug.Assert(formItemList.Count == idToFormItem.Keys.Count);
            }

            public bool Contains(IFormItem item)
            {
                return item == null ? false : formItemToDefaultLabel.ContainsKey(item);
            }

            public bool Contains(int id)
            {
                return idToFormItem.ContainsKey(id);
            }

            private static bool hasDefaultLabel(IFormItem item)
            {
                return item is IDefaultLabel;
            }

            private string getNextLabel(IFormItem item)
            {
                var formItemWithDefaultLabel = item as IDefaultLabel;

                if (formItemWithDefaultLabel == null)
                {
                    return string.Empty;
                }

                int index = defaultLabels.IndexOf(formItemWithDefaultLabel.DefaultLabelPrefix);
                string defaultLabel = defaultLabels[index] + nextDefaultLabelNums[index];
                nextDefaultLabelNums[index] += 1;

                return defaultLabel;
            }

            private void updateMap()
            {
                initializeDefaultLabelNums();

                foreach (IFormItem item in formItemList)
                {
                    formItemToDefaultLabel[item] = getNextLabel(item);
                    idToFormItem[item.Id] = item;
                }
            }
        }

        #endregion
    }
}