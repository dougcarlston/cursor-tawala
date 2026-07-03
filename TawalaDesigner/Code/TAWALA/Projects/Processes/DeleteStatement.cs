// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Runtime.Serialization;
using System.Text;
using Tawala.Projects.Fields;
using Tawala.XmlSupport;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements a Delete statement in the Process
    /// </summary>
    [Serializable]
    public class DeleteStatement : ProcessStatement
    {
        private static readonly string xmlDeleteEndTag = "</delete>";
        private static readonly string xmlDeleteStartTag = "<delete>";
        private static readonly string xmlFormTag = "<form name=\"{0}\"/>";
        protected Conditions conditions = new Conditions();
        private IForm form = Projects.Form.NULL;
        private string format = "{0} records from {1}{2}";

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
            form = insureValidForm(Project.Current.GetForm(formName));

            mapRecordFields(process);

            if (element.HasChild("conditions"))
            {
                conditions = new Conditions(element.GetChild("conditions"), process);
            }
        }

        public IForm Form { get { return insureValidForm(form); } set { form = insureValidForm(value); } }

        public Conditions Conditions { get { return conditions; } set { conditions = value; } }

        public static DeleteStatement ShallowCopy(DeleteStatement sourceDeleteStatement)
        {
            var statement = new DeleteStatement();
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

        private IForm insureValidForm(IForm form)
        {
            if (form == null || Project.Current == null || Project.Current.FormList == null)
            {
                return Projects.Form.NULL;
            }

            return Project.Current.FormList.Contains(form) ? form : Projects.Form.NULL;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
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

        public override string ToXml()
        {
            var xmlString = new StringBuilder(xmlDeleteStartTag);
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
                form = Projects.Form.NULL;
            }

            form = Project.Current.GetForm(form.Name);
        }
    }
}