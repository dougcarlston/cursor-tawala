// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Runtime.Serialization;
using Tawala.Projects.Documents;

namespace Tawala.Projects.Links
{
    [Serializable]
    public abstract class HyperlinkField : ParagraphComponent
    {
        [NonSerialized]
        protected Hyperlink hyperlink = Hyperlink.NULL;

        protected int id;

        public override string ToXml()
        {
            return hyperlink.ToXml();
        }

        public override string ToRtf()
        {
            return hyperlink.ToRtf();
        }

        [OnDeserialized]
        private void onDeserialized(StreamingContext context)
        {
            if (Project.InvitationMapById.ContainsKey(id))
            {
                hyperlink = Hyperlink.Duplicate(hyperlink);
            }
            else
            {
                hyperlink = Hyperlink.NULL;
            }
        }
    }
}