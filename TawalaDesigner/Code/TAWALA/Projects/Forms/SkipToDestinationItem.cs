// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Collections.ObjectModel;
using Tawala.Projects.Forms;

namespace Tawala.Projects
{
    /// <summary>
    /// Class to contain an item that can be the target of a SkipToStatement.
    /// </summary>
    [Serializable]
    public class SkipToDestinationItem
    {
        private const int endOfFormItemId = -2;
        public static int InvalidItemId = -1;

        private readonly string formItemLabel;
        private readonly int itemId = InvalidItemId;

        // special case: default constructor creates a "skip to end of Form" item
        public SkipToDestinationItem()
        {
            itemId = endOfFormItemId;
        }

        public SkipToDestinationItem(IFormItem formItem)
        {
            if (formItem != null)
            {
                itemId = formItem.Id;
            }
        }

        public SkipToDestinationItem(string formItemLabel)
        {
            this.formItemLabel = formItemLabel;
        }

        public string FormItemLabel { get { return formItemLabel; } }

        /// <summary>
        /// Reference to the FormItem
        /// </summary>
        public int ItemId { get { return itemId; } }

        /// <summary>
        /// denotes the validity of the destination item
        /// must be either an end-of-form item or contain the ID of a valid FormItem
        /// </summary>
        public bool Valid { get { return itemId == endOfFormItemId || Project.Current.GetFormItem(itemId) != null; } }

        /// <summary>
        /// Returns the Form Item's default or alternate label
        /// </summary>
        public override string ToString()
        {
            string labelString = "";

            if (itemId == endOfFormItemId)
            {
                labelString = "End of Form";
            }
            else
            {
                IFormItem item = Project.Current.GetFormItem(itemId);
                if (item != null)
                {
                    if (item.AlternateLabel.Length != 0)
                    {
                        labelString = item.AlternateLabel;
                    }
                    else
                    {
                        labelString = Project.Current.GetDefaultLabel(item);
                    }
                }
            }

            return labelString;
        }

        /// <summary>
        /// returns the string to be used for the XML attribute in the SkipTo statement
        /// </summary>
        public string AttributeString()
        {
            if (itemId == endOfFormItemId)
            {
                return "__EndOfForm__";
            }
            else
            {
                return ToString();
            }
        }
    }

    /// <summary>
    /// class to contain list of SkipToDestinationItems
    /// </summary>
    [Serializable]
    public class SkipToDestinationList : Collection<SkipToDestinationItem>, IField, IRecursiveEnumerable
    {
        #region IField Interface

        private readonly int id = Project.NextUniqueID;

        public string FieldName { get { return "Unnamed SkipToDestinationList"; } }

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
                foreach (SkipToDestinationItem item in Items)
                {
                    if (item.ItemId < 0)
                    {
                        yield break;
                    }

                    IField field = Project.Current.GetFormItem(item.ItemId);
                    yield return field;
                }
            }
        }

        #endregion

        #region IField Members

        public string QualifiedFieldName { get { throw new NotImplementedException(); } }

        #endregion
    }
}