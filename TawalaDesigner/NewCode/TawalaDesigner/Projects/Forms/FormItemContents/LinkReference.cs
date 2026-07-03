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
	public abstract class LinkReference : FormItemContents
	{
        private int linkId;

        public static LinkReference MakeHtmlLink(IXmlElement element)
        {
            LinkReference contents = null;

            int id = GetLinkIdFromXml(element);
            ILink link = Project.InvitationMapById[id];

            if (link is Hyperlink)
            {
                contents = new HyperlinkReference(element);
            }
            else if (link is InvitationField)
            {
                contents = new InvitationReference(element);
            }
            return contents;
        }

        protected LinkReference()
        {
        }

        protected ILink Link
        {
            get { return Project.InvitationMapById[linkId]; }
            set { linkId = value.Id; }
        }

        protected void SetLinkById(IXmlElement element)
        {
            int id = GetLinkIdFromXml(element);
            Link = Project.InvitationMapById[id];
        }

        protected static int GetLinkIdFromXml(IXmlElement element)
        {
            return Convert.ToInt32(element.GetAttribute("id").Replace("link_", ""));
        }

		#region IFormItemContents Members

		public override string ToXml()
		{
            return Link.ToXml();
        }

		public override string ToXhtml(IFormItem formItem)
		{
            return string.Format("<t:link id=\"link_{0}\">{1}</t:link>", Link.Id, Link.DisplayText);
        }

		#endregion
	}
}
