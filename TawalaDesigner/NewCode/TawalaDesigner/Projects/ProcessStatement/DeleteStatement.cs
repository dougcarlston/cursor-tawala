// $Workfile: DeleteStatement.cs $
// $Revision: 10 $	$Date: 12/17/07 4:48p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	/// <summary>
	/// Implements a Delete statement in the Process
	/// </summary>
	[Serializable]
	public class DeleteStatement : Tawala.Projects.ProcessStatement
	{
		public DeleteStatement()
		{
			name = "Delete";
		}

		public DeleteStatement(IXmlElement element, string processName) : this(element, Project.Current.GetProcess(processName))
		{
		}

		public DeleteStatement(IXmlElement element, Process process) : this()
		{
			string formName = element.GetChild("form").GetAttribute("name");
			this.form = insureValidForm(Project.Current.GetForm(formName));

			mapRecordFields(process);

			if (element.HasChild("conditions"))
			{
				this.conditions = new Conditions(element.GetChild("conditions"), process);
			}
		}

		public static DeleteStatement ShallowCopy(DeleteStatement sourceDeleteStatement)
		{
			DeleteStatement statement = new DeleteStatement();
			statement.Form = sourceDeleteStatement.Form;
			statement.Conditions = Conditions.ShallowCopy(sourceDeleteStatement.Conditions);

			return statement;
		}

		private void mapRecordFields(Process process)
		{
			process.MappedFields.Qualifiers.Add(FieldUtil.DefaultRecordQualifierName);

			process.MapFormFields(form);

			process.MappedFields.Map();
		}


        private IForm form = Tawala.Projects.NullObjects.Form;

		public IForm Form
		{
			get
			{
				return insureValidForm(form);
			}
			set
			{
				form = insureValidForm(value);
			}
		}

		private IForm insureValidForm(IForm form)
		{
			if (form == null || Project.Current == null || Project.Current.FormList == null)
			{
                return Tawala.Projects.NullObjects.Form;
			}

            return Project.Current.FormList.Contains(form) ? form : Tawala.Projects.NullObjects.Form;
		}

		protected Conditions conditions = new Conditions();

		public Conditions Conditions
		{
			get
			{
				return conditions;
			}
			set
			{
				conditions = value;
			}
		}

		private string format = "{0} records from {1}{2}";

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat(format, Name, Form.Name, createConditionsString());
			return sb.ToString();
		}

		private string createConditionsString()
		{
			string result = string.Empty;

			if (conditions.Count > 0)
			{
				result += " where ";
				result += conditions.ToString();
			}

			return result;
		}

		private static readonly string xmlDeleteStartTag = "<delete>";
		private static readonly string xmlDeleteEndTag = "</delete>";
		private static readonly string xmlFormTag = "<form name=\"{0}\"/>";

		public override string ToXml()
		{
			StringBuilder xmlString = new StringBuilder(xmlDeleteStartTag);
			xmlString.AppendFormat(xmlFormTag, Form.Name);

			if (conditions.Count > 0)
			{
				xmlString.Append(conditions.ToXml());
			}

			xmlString.AppendFormat(xmlDeleteEndTag);

			return xmlString.ToString();
		}

		[OnDeserialized]
		private void onUndo(StreamingContext context)
		{
			if (form == null)
			{
                form = Tawala.Projects.NullObjects.Form;
			}

			form = Project.Current.GetForm(form.Name);
		}
	}
}
