// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Factories;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms.FormItemContents
{
    [Serializable]
    public abstract class TableBodyContents : FormItemContents
    {
        protected TableBodyContents(IXmlElement element)
        {
            Contents = FormItemContentsFactory.MakeChildren(element);
        }

        public override string ToXml()
        {
            return Contents.ToXml();
        }

        public override string ToXhtml(IFormItem formItem)
        {
            return string.Format("<tbody>{0}</tbody>", Contents.ToXhtml(formItem));
        }
    }

    [Serializable]
    public class TableBodyXmlContents : TableBodyContents
    {
        public TableBodyXmlContents(IXmlElement element)
            : base(element)
        {
        }
    }

    [Serializable]
    public class TableBodyXhtmlContents : TableBodyContents
    {
        public TableBodyXhtmlContents(IXmlElement element)
            : base(element)
        {
        }
    }
}