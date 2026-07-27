// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Links;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
{
    [Serializable]
    public class DocumentNamedInvitationField : DocumentInvitationField
    {
        public DocumentNamedInvitationField(IXmlElement element)
        {
            invitation = new InvitationField(element);
            id = invitation.Id;
        }
    }
}