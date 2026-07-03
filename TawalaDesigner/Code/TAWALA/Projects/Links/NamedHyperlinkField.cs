// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.XmlSupport;

namespace Tawala.Projects.Links
{
    [Serializable]
    public class NamedHyperlinkField : HyperlinkField
    {
        public NamedHyperlinkField(IXmlElement element)
        {
            hyperlink = new Hyperlink(element);
            id = hyperlink.Id;
        }
    }
}