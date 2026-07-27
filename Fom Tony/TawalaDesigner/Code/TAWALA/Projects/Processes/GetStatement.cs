// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text;
using Tawala.Projects.Forms;
using Tawala.Projects.Properties;
using Tawala.XmlSupport;

namespace Tawala.Projects.Processes
{
    [Serializable]
    public class GetStatement : ProcessStatement
    {
        private const string xmlExternalFormTag = "<form name=\"{0}\" externalSharedData=\"true\"/>\r\n";
        private const string xmlFormListEndTag = "</forms>\r\n";
        private const string xmlFormListStartTag = "<forms>\r\n";
        private const string xmlFormTag = "<form name=\"{0}\"/>\r\n";
        private const string xmlGetEndTag = "</get>";
        private const string xmlGetStartTag = "<get recordList=\"{0}\">\r\n";
        protected Conditions conditions = new Conditions();

        [NonSerialized]
        private Process originalProcess = Process.NULL;

        [NonSerialized]
        private IXmlElement originalXmlElement = XmlElement.NULL;

        private RecordSet records;

        public GetStatement()
        {
            name = "Get";

            Project.Events.FieldProvidersChanged += events_FieldProvidersChanged;
        }

        public GetStatement(RecordSet records) : this()
        {
            this.records = records;
        }

        public GetStatement(IXmlElement element, string processName) : this(element, Project.Current.GetProcess(processName))
        {
        }

        public GetStatement(IXmlElement element, Process process) : this()
        {
            originalProcess = process;
            originalXmlElement = element;

            constructFromXml(element, process);
        }

        public RecordSet Records { set { records = value; } get { return records; } }

        public Conditions Conditions { get { return conditions; } set { conditions = value; } }

        private void events_FieldProvidersChanged(object sender, EventArgs e)
        {
            if (referencesExternalForms(originalXmlElement))
            {
                constructFromXml(originalXmlElement, originalProcess);
            }
        }

        private bool referencesExternalForms(IXmlElement element)
        {
            IXmlElement formListElement = element.GetChild("forms");
            foreach (IXmlElement formElement in formListElement.GetChildren("form"))
            {
                if (FieldProviders.ExternalForms.ContainsComponentNamed(formElement.GetAttribute("name")))
                {
                    return true;
                }
            }
            return false;
        }

        private void constructFromXml(IXmlElement element, Process process)
        {
            FormList forms = getForms(element.GetChild("forms"));
            records = new RecordSet(element.GetAttribute("recordList"), forms);

            if (!process.RecordSets.Contains(records))
            {
                process.RecordSets.Add(records);
            }

            mapFields(process);

            if (element.HasChild("conditions"))
            {
                conditions = new Conditions(element.GetChild("conditions"), process);
            }
        }

        private FormList getForms(IXmlElement element)
        {
            var forms = new FormList();

            if (element != XmlElement.NULL)
            {
                FormList allForms = Project.Current.AllForms;

                foreach (IXmlElement formElement in element.GetChildren("form"))
                {
                    string name = formElement.GetAttribute("name");
                    if (allForms.IndexOf(name) >= 0)
                    {
                        forms.Add(allForms[name]);
                    }
                }
            }
            return forms;
        }

        /// <summary>
        /// Adds record set-qualified fields (such as "Record List 1:Q1:a") to the specified process's field mapper.
        /// </summary>
        private void mapFields(Process process)
        {
            process.MappedFields.Qualifiers.Add(records.FieldName);

            records.MapFormFields(process);

            process.MappedFields.Map();
        }

        public override string ToString()
        {
            string resultString = Name + " " + records.FieldName + Resources.GetStatementFrom;

            resultString = appendFormNames(resultString);

            resultString = appendConditions(resultString);

            return resultString;
        }

        public static GetStatement ShallowCopy(GetStatement sourceGetStatement)
        {
            var statement = new GetStatement();
            statement.Records = RecordSet.ShallowCopy(sourceGetStatement.Records);
            statement.Conditions = Conditions.ShallowCopy(sourceGetStatement.Conditions);
            return statement;
        }

        private string appendFormNames(string resultString)
        {
            return resultString += FieldProviders.CreateValidFormListString(records.Forms, Resources.GetStatementNoFormsSelected);
        }

        private string appendConditions(string resultString)
        {
            if (conditions.Count > 0)
            {
                resultString += Resources.GetStatementWhere;
                resultString += conditions.ToString();
            }

            return resultString;
        }

        public override string ToXml()
        {
            var xmlString = new StringBuilder();
            xmlString.AppendFormat(xmlGetStartTag, Records.FieldName);

            FormList validForms = FieldProviders.CreateValidFormList(records.Forms);

            xmlString.Append(xmlFormListStartTag);
            if (validForms.Count != 0)
            {
                foreach (IForm f in validForms)
                {
                    if (f is ExternalForm)
                    {
                        xmlString.AppendFormat(xmlExternalFormTag, f.Name);
                    }
                    else
                    {
                        xmlString.AppendFormat(xmlFormTag, f.Name);
                    }
                }
            }
            xmlString.Append(xmlFormListEndTag);

            if (conditions.Count > 0)
            {
                xmlString.Append(conditions.ToXml());
            }

            xmlString.Append(xmlGetEndTag);

            return xmlString.ToString();
        }
    }
}