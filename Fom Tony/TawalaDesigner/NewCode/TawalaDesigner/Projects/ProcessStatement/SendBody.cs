// Copyright © 2005 - 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Diagnostics;
using System.Text;
using System.Runtime.Serialization;
using Tawala.Common;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	[Serializable]
	public class SendBody
	{
		public static SendBody NULL = new NullSendBody();

		public SendBody()
		{
		}

		/// <summary>
		/// Construct SendStatement from XML "<body>" element.
		/// </summary>
		public SendBody(IXmlElement element)
		{
		}

		public virtual string ToXml()
		{
			return ("SendBody.ToXml() should never be called directly");
		}

		public override string ToString()
		{
			return ("SendBody.ToString() should never be called directly");
		}

		public virtual bool IsValid(ProcessStatement statement)
		{
			return false;
		}
	}

	[Serializable]
	public class SendEmailBody : SendBody
	{
		protected string text = "";

		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
			}
		}

		public SendEmailBody()
		{
		}

		public SendEmailBody(string text)
		{
			this.text = text;
		}

		public SendEmailBody(IXmlElement element) : base(element)
		{
			this.text = (element.Text == null ? "" : element.Text);
		}

		public override string ToString()
		{
			return ("Email");
		}

		private static readonly string xmlSendEmailBodyTag = "<body>$BODY</body>\r\n";

		public override string ToXml()
		{
			StringBuilder xmlString = new StringBuilder(xmlSendEmailBodyTag);
			xmlString.Replace("$BODY", XMLStringFormatter.EscapeElementText(text));

			return xmlString.ToString();
		}

		public override bool IsValid(ProcessStatement statement)
		{
			return true;
		}
	}

	[Serializable]
	public class SendDocumentBody : SendBody
	{
        private IDocument document = NullObjects.Document;

		public IDocument Document
		{
			get
			{
				return document;
			}
			set
			{
				document = value;
			}
		}

		public SendDocumentBody(IDocument document)
		{
			this.document = document;
		}

		public SendDocumentBody(IXmlElement element)
			: base(element)
		{
			this.document = Project.Current.GetRealOrVirtualDocument(element.GetAttribute("document"), true);
			this.resetDocumentAfterSend = element.GetAttribute("reset") == "true" ? true : false;
			if (element.HasAttribute("showHeader"))
			{
				this.showPageHeader = element.GetAttribute("showHeader") == "true" ? true : false;
			}
		}

		private bool resetDocumentAfterSend = false;

		public bool ResetDocumentAfterSend
		{
			get { return resetDocumentAfterSend; }
			set { resetDocumentAfterSend = value; }
		}

		private bool showPageHeader = true;

		public bool ShowPageHeader
		{
			get { return showPageHeader; }
			set { showPageHeader = value; }
		}

		public override string ToString()
		{
			return (document.Name);
		}

		private static readonly string xmlSendDocumentBodyTag = "<body document=\"{0}\" reset=\"{1}\" showHeader=\"{2}\"/>\r\n";

		public override string ToXml()
		{
			StringBuilder xmlString = new StringBuilder();
			xmlString.AppendFormat(xmlSendDocumentBodyTag,
				XMLStringFormatter.EscapeAttributeText(document.Name),
				resetDocumentAfterSend ? "true" : "false",
				showPageHeader ? "true" : "false");

			return xmlString.ToString();
		}

		public override bool IsValid(ProcessStatement statement)
		{
			return statement.ValidateDocumentReference(document) == ProcessStatement.StatementStatus.Valid;
		}

		[OnDeserialized]
		private void onUndo(StreamingContext context)
		{
			this.document = Project.Current.GetRealOrVirtualDocument(document.Name, false);

			if (this.document == null)
			{
                this.document = NullObjects.Document;
			}
		}

	}

	[Serializable]
	public class SendInvitationBody : SendEmailBody
	{
		protected IForm form;

		public IForm Form
		{
			get
			{
				return form;
			}
			set
			{
				form = value;
			}
		}

		public virtual string ProjectName
		{
			get
			{
				return Project.Current.Name;
			}
			set
			{
			}
		}

		public SendInvitationBody()
		{
		}

		public SendInvitationBody(IForm form)
		{
			this.form = form;
		}

		public SendInvitationBody(IForm form, string text) : this(form)
		{
			this.text = text;
		}

		public SendInvitationBody(IXmlElement element)
			: base(element)
		{
			string formName = element.GetAttribute("inviteTo");

			this.form = Project.Current.GetForm(formName);

			if (this.form == null)
			{
				this.form = new FormInfo(formName);
			}
		}

		public override string ToString()
		{
			return ("Invitation to " + form.Name);
		}

		private static readonly string xmlSendInvitationBodyTag = "<body inviteTo=\"$STARTINGPOINT\">$BODY</body>\r\n";
		
		public override string ToXml()
		{
			StringBuilder xmlString = new StringBuilder(xmlSendInvitationBodyTag);
			xmlString.Replace("$STARTINGPOINT", XMLStringFormatter.EscapeAttributeText(form.Name));
			xmlString.Replace("$BODY", XMLStringFormatter.EscapeElementText(text));

			return xmlString.ToString();
		}

		public override bool IsValid(ProcessStatement statement)
		{
			if (form is IForm)
			{
				return statement.ValidateFormReference((IForm)form) == ProcessStatement.StatementStatus.Valid;
			}
			else
			{
				return true;
			}
		}
	}

	[Serializable]
	public class SendForeignInvitationBody : SendInvitationBody
	{
		private string projectName = "";

		public override string ProjectName
		{
			get
			{
				return projectName;
			}
			set
			{
				projectName = value;
			}
		}

		public SendForeignInvitationBody()
		{
		}

		public SendForeignInvitationBody(IForm form, string projectName, string text) : base(form, text)
		{
			this.projectName = projectName;
		}

		public SendForeignInvitationBody(IXmlElement element) : base(element)
		{
			this.projectName = element.GetAttribute("project");
		}

		public override string ToString()
		{
			if (projectName == null || projectName == "" || projectName == "(Current Project)" || projectName == Project.Current.Name)
			{
				return base.ToString();
			}
			else
			{
				return ("Invitation to " + projectName + ":" + form.Name);
			}
		}

		private static readonly string xmlSendForeignInvitationBodyTag = "<body inviteTo=\"$STARTINGPOINT\" project=\"$PROJECT\">$BODY</body>\r\n";

		public override string ToXml()
		{
			StringBuilder xmlString;

			if (projectName == null || projectName == "" || projectName == "(Current Project)" || projectName == Project.Current.Name)
			{
				return base.ToXml();
			}
			else
			{
				xmlString = new StringBuilder(xmlSendForeignInvitationBodyTag);
				xmlString.Replace("$STARTINGPOINT", XMLStringFormatter.EscapeAttributeText(form.Name));
				xmlString.Replace("$BODY", XMLStringFormatter.EscapeElementText(text));
				xmlString.Replace("$PROJECT", projectName);
			}

			return xmlString.ToString();
		}
	}

	[Serializable]
	public class NullSendBody : SendBody
	{
		public NullSendBody()
		{
		}

		public override string ToString()
		{
			return ("Email");
		}

		public override bool IsValid(ProcessStatement statement)
		{
			return false;
		}
	}

}
