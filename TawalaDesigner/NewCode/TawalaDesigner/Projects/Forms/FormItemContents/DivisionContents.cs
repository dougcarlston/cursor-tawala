// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Factories;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms.FormItemContents
{
    [Serializable]
    public class DivisionContents : FormItemContents
    {
        protected int leftOffsetInTwips;

        public DivisionContents(IXmlElement element)
        {
            Contents = FormItemContentsFactory.MakeChildren(element);
            leftOffsetInTwips = Convert.ToInt32(element.GetAttribute("indent"));
        }

        public override string ToXml()
        {
            return string.Format("<division indent=\"{0}\" align=\"left\"><font>{1}</font></division>", leftOffsetInTwips, Contents.ToXml());
        }

        public override string ToXhtml(IFormItem formItem)
        {
            return Contents.ToXhtml(formItem);
        }
    }
}