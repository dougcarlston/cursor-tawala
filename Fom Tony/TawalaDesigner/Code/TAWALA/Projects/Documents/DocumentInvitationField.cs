// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Runtime.Serialization;
using Tawala.Projects.Links;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
{
    [Serializable]
    public abstract class DocumentInvitationField : ParagraphComponent, IDocumentConversions
    {
        protected int id;

        [NonSerialized]
        protected InvitationField invitation = InvitationField.Null;

        public InvitationField Invitation
        {
            get { return invitation; }
        }

        #region IDocumentConversions Members

        public override string ToXml()
        {
            return invitation.ToXml();
        }

        [Obsolete]
        public override string ToHtml()
        {
            return "ToHtml() is obsolete!";
        }

        public override string ToRtf()
        {
            return invitation.ToRtf();
        }

        public override string ToRtf(RtfDocument document)
        {
            return ToRtf();
        }

        #endregion

        [OnDeserialized]
        private void onDeserialized(StreamingContext context)
        {
            if (Project.InvitationMapById.ContainsKey(id))
            {
                cloneInvitationField();
            }
            else
            {
                invitation = InvitationField.Null;
            }
        }

        private void cloneInvitationField()
        {
            var invitationToCopy = (InvitationField)Project.InvitationMapById[id];
            IXmlElement invitationXml = new XmlElement(invitationToCopy.ToXml());
            invitation = new InvitationField(invitationXml);
        }
    }
}