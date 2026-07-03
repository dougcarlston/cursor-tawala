// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using Tawala.Projects.Factories;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms.FormItemContents
{
    [Serializable]
    public abstract class FormItemContents : IFormItemContents
    {
        public static readonly NullContents NULL = new NullContents();

        protected FormItemContents()
        {
        }

        protected FormItemContents(IXmlElement element)
        {
            Contents = FormItemContentsFactory.MakeChildren(element);
        }

        #region IFormItemContents Members

        public virtual string ToXml()
        {
            return string.Empty;
        }

        public virtual string ToXhtml(IFormItem formItem)
        {
            return string.Empty;
        }

        public FormItemContentsCollection GetDescendants(Type descendantType)
        {
            var descendants = new FormItemContentsCollection();

            if (descendantType.IsInstanceOfType(this))
            {
                descendants.Add(this);
            }

            if (Contents != null)
            {
                foreach (IFormItemContents item in Contents.GetDescendants(descendantType))
                {
                    descendants.Add(item);
                }
            }

            return descendants;
        }

        public IFormItemContents Contents { get; set; }

        public virtual void ApplyFontStyle(FontStyle style)
        {
            if (Contents != null)
            {
                Contents.ApplyFontStyle(style);
            }
        }

        public virtual FontStyle GetInnermostFontStyle()
        {
            if (Contents != null)
            {
                return Contents.GetInnermostFontStyle();
            }

            return null;
        }

        public virtual string Text
        {
            get { return Contents.Text; }
        }

        public virtual void ResolveFieldReferences()
        {
            if (Contents != null)
            {
                Contents.ResolveFieldReferences();
            }
        }

        public virtual void ResolveFunctionReferences()
        {
            if (Contents != null)
            {
                Contents.ResolveFunctionReferences();
            }
        }

        #endregion
    }
}