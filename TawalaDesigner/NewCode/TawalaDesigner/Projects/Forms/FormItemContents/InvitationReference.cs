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
    public class InvitationReference : LinkReference
    {
        public InvitationReference()
        {
        }

        public InvitationReference(IXmlElement element)
            : this()
        {
            if (element.HasAttribute("id"))
            {
                SetLinkById(element);
            }
            else
            {
                Link = new InvitationField(element);
            }
        }

        public InvitationField Invitation
        {
            get { return Link as InvitationField; }
            set { Link = value; }
        }  
    }
}
