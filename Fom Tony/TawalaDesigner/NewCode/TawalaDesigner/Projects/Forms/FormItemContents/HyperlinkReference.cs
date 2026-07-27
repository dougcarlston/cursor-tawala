// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Tawala.XmlSupport;
using Tawala.Common;
using Tawala.Projects;
using Tawala.Projects.Forms.NewFormItems;

namespace Tawala.Projects.Forms.FormItemContents
{
    [Serializable]
    public class HyperlinkReference : LinkReference
    {
        public HyperlinkReference()
        {
        }

        public HyperlinkReference(IXmlElement element)
            : this()
        {

            if (element.HasAttribute("id"))
            {
                SetLinkById(element);
            }
            else
            {
               Link = new Hyperlink(element);
            }
        }

        public Hyperlink Hyperlink
        {
            get { return Link as Hyperlink; }
            set { Link = value; }
        }
    }
}
