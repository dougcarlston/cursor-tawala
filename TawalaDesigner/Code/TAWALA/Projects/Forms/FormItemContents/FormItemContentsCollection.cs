// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Projects.Forms.NewFormItems;

namespace Tawala.Projects.Forms.FormItemContents
{
    [Serializable]
    public class FormItemContentsCollection : Collection<IFormItemContents>, IFormItemContents
    {
        #region IFormItemContents Members

        public string ToXml()
        {
            var xmlString = new StringBuilder();

            foreach (IFormItemContents contentItem in Items)
            {
                xmlString.Append(contentItem.ToXml());
            }

            return xmlString.ToString();
        }

        public string ToXhtml(IFormItem formItem)
        {
            var xhtmlString = new StringBuilder();

            foreach (IFormItemContents contentItem in Items)
            {
                xhtmlString.Append(contentItem.ToXhtml(formItem));
            }

            return xhtmlString.ToString();
        }

        public FormItemContentsCollection GetDescendants(Type descendantType)
        {
            var descendants = new FormItemContentsCollection();

            foreach (IFormItemContents item in this)
            {
                foreach (IFormItemContents subItem in item.GetDescendants(descendantType))
                {
                    descendants.Add(subItem);
                }
            }

            return descendants;
        }

        public IFormItemContents Contents { get { return this; } set { } }

        public void ApplyFontStyle(FontStyle style)
        {
            foreach (IFormItemContents item in this)
            {
                item.ApplyFontStyle(style);
            }
        }

        public FontStyle GetInnermostFontStyle()
        {
            return Count > 0 ? this[0].GetInnermostFontStyle() : new FontStyle();
        }

        public string Text
        {
            get
            {
                string text = string.Empty;

                foreach (IFormItemContents item in this)
                {
                    text += item.Text;
                }

                return text;
            }
        }

        public void ResolveFieldReferences()
        {
            foreach (IFormItemContents item in this)
            {
                if (item != null)
                {
                    item.ResolveFieldReferences();
                }
            }
        }

        public void ResolveFunctionReferences()
        {
            foreach (IFormItemContents item in this)
            {
                if (item != null)
                {
                    item.ResolveFunctionReferences();
                }
            }
        }

        #endregion
    }
}