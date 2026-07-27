// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.XmlSupport;

namespace Tawala.Projects.Links
{
    [Serializable]
    public class IdedHyperlinkField : HyperlinkField
    {
        public IdedHyperlinkField(IXmlElement element)
        {
            id = Convert.ToInt32(element.GetAttribute("id"));
            hyperlink = (Project.InvitationMapById.ContainsKey(id) ? (Hyperlink)Project.InvitationMapById[id] : Hyperlink.NULL);
        }
    }
}