// $Workfile: InvitationField.cs $
// $Revision: 23 $	$Date: 3/14/08 10:02a $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using Tawala.Common;

namespace Tawala.Projects
{
    [Serializable]
	public class InvitationField : ILink
	{
		public static InvitationField NULL = new NullInvitationField();

		public InvitationField()
		{
			this.id = Project.NextUniqueID;
			Project.InvitationMapById.AddUnique(this);
		}

		public InvitationField(IXmlElement element) : this()
	    {
			this.initialFormName = element.GetAttribute("form");
			this.projectName = element.GetAttribute("project");
			this.displayText = element.Text;
			this.isPrivate = element.HasAttribute("private") && element.GetAttribute("private").CompareTo("true") == 0;

			if (isPrivate)
			{
				if (element.HasChild("authenticationTokenValue"))
				{
					authenticationTokenExpression = new CompoundExpression(element.GetChild("authenticationTokenValue"), buildFieldResolver());
				}
			}

			if (invitationReferencesCurrentProject())
			{
				this.form = makeFormReference();
			}
	    }

		private FieldList buildFieldResolver()
		{
			FieldList allFields = new FieldList();

			foreach (IForm form in Project.Current.FormList)
			{
				allFields.Add(form.GetAllFields());
			}

			allFields.Add(Project.Current.AllVariables);

			return allFields;
		}

	    private IForm makeFormReference()
	    {
	        IForm form = Project.Current.GetForm(initialFormName);

	        if (form == null)
	        {
                form = Tawala.Projects.NullObjects.Form;
	        }

	        return form;
	    }

	    private bool invitationReferencesCurrentProject()
	    {
	        return projectName.Equals("");
	    }

		private int id = 0;

		public int Id
		{
			get { return id; }
		}

        private IForm form = Tawala.Projects.NullObjects.Form;

		public IForm Form
		{
			get { return form; }
			set { form = value; }
		}

		private string initialFormName = "";

		public string InitialFormName
		{
			get { return initialFormName; }
			set { initialFormName = value; }
		}

		public string FormName
		{
			get
			{
                if (form == Tawala.Projects.NullObjects.Form || form == null)
				{
					return initialFormName;
				}
				
				return form.Name;
			}
		}

		protected string projectName = "";

		public string ProjectName
		{
			get { return projectName; }
			set { projectName = value; }
		}

		protected string displayText = "";

		public string DisplayText
		{
			get { return displayText; }
			set { displayText = value; }
		}

		private bool isPrivate = false;

		public bool IsPrivate
		{
			get { return isPrivate; }
			set { isPrivate = value; }
		}

		CompoundExpression authenticationTokenExpression;

		public CompoundExpression AuthenticationTokenExpression
		{
			get { return authenticationTokenExpression; }
			set { authenticationTokenExpression = value; }
		}

		private static string publicFormat = "<invitation form=\"{0}\" project=\"{1}\">{2}</invitation>";
		private static string privateFormat = "<invitation form=\"{0}\" project=\"{1}\" private=\"true\"><authenticationTokenValue>{2}</authenticationTokenValue>{3}</invitation>";
		public string ToXml()
		{
			if (isPrivate)
			{
				return String.Format(privateFormat, FormName, projectName, authenticationTokenExpression.ToXml(), XMLStringFormatter.EscapeElementText(displayText));
			}
			else
			{
				return String.Format(publicFormat, FormName, projectName, XMLStringFormatter.EscapeElementText(displayText));
			}
		}

		[Serializable]
		private class NullInvitationField : InvitationField
		{
			public NullInvitationField() : base()
			{
				Project.InvitationMapById.Remove(this.Id);
			}
		}
	}
}
