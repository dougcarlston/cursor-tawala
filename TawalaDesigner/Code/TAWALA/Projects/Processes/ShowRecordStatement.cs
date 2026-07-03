// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using Tawala.Common;
using Tawala.Projects.Fields;
using Tawala.XmlSupport;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements a Show statement in the Process.  
    /// Note that it requires deserialization fixups under some circumstances.  See onDeserialized method.
    /// </summary>
    [Serializable]
    public class ShowRecordStatement : ShowStatement
    {
        private const string xmlShowRecordString = "<edit form=\"{0}\" submit=\"{1}\">\r\n{2}</edit>";
        private Conditions conditions = new Conditions();
        private string format = "{0} stored record from {1}{2}";
        private bool modifyOnSubmit = true;

        public ShowRecordStatement()
        {
        }

        public ShowRecordStatement(IForm form) : this()
        {
            component = form;
        }

        public ShowRecordStatement(IXmlElement element, string processName) : this(element, Project.Current.GetProcess(processName))
        {
        }

        public ShowRecordStatement(IXmlElement element, Process process) : this()
        {
            modifyOnSubmit = element.GetAttribute("submit").CompareTo("modify") == 0;

            component = Project.Current.GetForm(element.GetAttribute("form"));
            if (component == null)
            {
                component = Projects.Form.NULL;
            }

            mapRecordFields(process);

            if (element.HasChild("conditions"))
            {
                conditions = new Conditions(element.GetChild("conditions"), process);
            }
        }

        public bool ModifyOnSubmit { get { return modifyOnSubmit; } set { modifyOnSubmit = value; } }

        public Conditions Conditions { get { return conditions; } set { conditions = value; } }

        public static ShowRecordStatement ShallowCopy(ShowRecordStatement source)
        {
            var copy = new ShowRecordStatement();
            copy.Form = source.Form;
            copy.ModifyOnSubmit = source.ModifyOnSubmit;
            copy.Conditions = Conditions.ShallowCopy(source.Conditions);
            return copy;
        }

        public override Type GetStatementType()
        {
            return typeof(ShowStatement);
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
            string escapedFormName = XMLStringFormatter.EscapeAttributeText(Form.Name);
            string conditionsXml = (conditions != null && conditions.Count > 0) ? conditions.ToXml() + "\r\n" : string.Empty;
            string submit = ModifyOnSubmit ? "modify" : "new";

            var sb = new StringBuilder();
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