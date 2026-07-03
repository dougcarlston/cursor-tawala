// $Workfile: ShowRecordStatement.cs $
// $Revision: 9 $	$Date: 6/18/07 5:51p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using Tawala.Common;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	/// <summary>
	/// Implements a Show statement in the Process.  
	/// Note that it requires deserialization fixups under some circumstances.  See onDeserialized method.
	/// </summary>
	[Serializable]
	public class ShowRecordStatement : ShowStatement
	{
		public ShowRecordStatement()
		{
		}

		public ShowRecordStatement(IForm form)	: this()
		{
			this.component = form;
		}

		public ShowRecordStatement(IXmlElement element, string processName) : this(element, Project.Current.GetProcess(processName))
		{
		}

		public ShowRecordStatement(IXmlElement element, Process process)
			: this()
		{
			modifyOnSubmit = element.GetAttribute("submit").CompareTo("modify") == 0;

			this.component = Project.Current.GetForm(element.GetAttribute("form"));
			if (this.component == null)
			{
                this.component = Tawala.Projects.NullObjects.Form;
			}

			mapRecordFields(process);

			if (element.HasChild("conditions"))
			{
				conditions = new Conditions(element.GetChild("conditions"), process);
			}
		}

		public static ShowRecordStatement ShallowCopy(ShowRecordStatement source)
		{
			ShowRecordStatement copy = new ShowRecordStatement();
			copy.Form = source.Form;
			copy.ModifyOnSubmit = source.ModifyOnSubmit;
			copy.Conditions = Conditions.ShallowCopy(source.Conditions);
			return copy;
		}

		private bool modifyOnSubmit = true;

		public bool ModifyOnSubmit
		{
			get { return modifyOnSubmit; }
			set { modifyOnSubmit = value; }
		}

		private Conditions conditions = new Conditions();

		public Conditions Conditions
		{
			get { return conditions; }
			set { conditions = value; }
		}

		public override Type GetStatementType()
		{
			return typeof(ShowStatement);
		}

		private string format = "{0} stored record from {1}{2}";

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


		private const string xmlShowRecordString = "<edit form=\"{0}\" submit=\"{1}\">\r\n{2}</edit>";

		public override string ToXml()
		{
			string escapedFormName = XMLStringFormatter.EscapeAttributeText(Form.Name);
			string conditionsXml = (conditions != null && conditions.Count > 0) ? conditions.ToXml() + "\r\n" : string.Empty;
			string submit = ModifyOnSubmit ? "modify" : "new";

			StringBuilder sb = new StringBuilder();
			sb.AppendFormat(xmlShowRecordString, escapedFormName, submit, conditionsXml);

			return sb.ToString();
		}

        [OnDeserialized]
        private void onDeserialized(StreamingContext context)
        {
            Debug.Assert(!string.IsNullOrEmpty(serializedComponentName));

            foreach (IForm form in Project.Current.FormList)
            {
                if (form.Name.CompareTo(serializedComponentName) == 0)
                {
                    component = form;
                    break;
                }
            }

            component = Project.Current.GetForm(serializedComponentName);

            if (component == null)
            {
                IForm form = Project.Current.AddForm();
                form.Name = serializedComponentName;
                component = form;
            }
            serializedComponentName = null;
        }

		private void mapRecordFields(Process process)
		{
			process.MappedFields.Qualifiers.Add(FieldUtil.DefaultRecordQualifierName);

			process.MapFormFields(Form);

			process.MappedFields.Map();
		}
	}
}

