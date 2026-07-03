// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Links;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
{
    [Serializable]
    public class DocumentIdedInvitationField : DocumentInvitationField
    {
        public DocumentIdedInvitationField(IXmlElement element)
        {
            id = Convert.ToInt32(element.GetAttribute("id"));
            invitation = (Project.InvitationMapById.ContainsKey(id) ? (InvitationField)Project.InvitationMapById[id] : InvitationField.Null);
        }
    }
}