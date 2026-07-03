// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms.FormItemContents
{
    public class XhtmlElement : XmlElement, IXhtmlElement
    {
        public XhtmlElement(string htmlString, bool preserveWhitespace) : base(XmlUtility.ToXhtml(htmlString), preserveWhitespace)
        {
        }
    }
}