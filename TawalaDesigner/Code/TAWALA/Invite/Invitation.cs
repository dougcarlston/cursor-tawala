// $Workfile: Invitation.cs $
// $Revision: 4 $	$Date: 11/25/05 4:35p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Tawala.Invite
{
	/// <summary>
	/// Simple class for a single invitation.
	/// </summary>
	[Serializable]
	internal class Invitation
	{
		private static readonly string mailToFormat = "mailto:?subject={0}&body={1}";

		internal Invitation(string name, string project, string form)
		{
			this.name = name;
			this.project = project;
			this.form = form;
		}

		/// <summary>
		/// Launch the users e-mail client to create a new e-mail with the subject and body of this invitation.
		/// </summary>
		internal void Send()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat(mailToFormat,
							Uri.EscapeDataString(subject),
							Uri.EscapeDataString(body));
			Process.Start(sb.ToString());
		}

		private string name; // invitation name

		internal string Name
		{
			get { return name; }
			set { name = value; }
		}

		private string project; // project name

		internal string Project
		{
			get { return project; }
			set { project = value; }
		}

		private string form; // form name

		internal string Form
		{
			get { return form; }
			set { form = value; }
		}

		private string subject = string.Empty;

		internal string Subject
		{
			get { return subject; }
			set { subject = value != null ? value : string.Empty; ; }
		}

		private string body = string.Empty;

		internal string Body
		{
			get { return body; }
			set { body = value != null ? value : string.Empty; }
		}

		// The url is regenerated every time the deployment info is received
		// and the invitations are connected up to the forms and project in the UI.
		// So don't save the url.  But we do need it on send to replace the link placedholder with
		// the url.
		[NonSerialized]
		private string url = string.Empty;

		internal string Url
		{
			get { return url; }
			set { url = value; }
		}
	}
}
