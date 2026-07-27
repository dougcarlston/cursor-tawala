// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;
using Tawala.Common;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using XmlElement=Tawala.XmlSupport.XmlElement;

namespace Tawala.Projects
{
    public static class FieldProviders
    {
        private const string queryXML =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + "<request type=\"queryDataSources\" protocol=\"1.0\">" + "{0}" + "</request>";

        private static volatile bool initialized;

        public static bool IsInitialized { get { return initialized; } }

        public static FormList ExternalForms { get { return externalForms; } set { externalForms = value; } }

        public static FormList CreateValidFormList(FormList unvalidated)
        {
            FormList current = Project.Current.AllForms;
            var validated = new FormList();

            foreach (IForm f in unvalidated)
            {
                if (current.Contains(f))
                {
                    validated.Add(f);
                }
            }

            return validated;
        }

        public static string CreateValidFormListString(FormList unvalidated, string noForms)
        {
            FormList validated = CreateValidFormList(unvalidated);

            if (validated.Count == 0)
            {
                return noForms;
            }

            var sb = new StringBuilder();
            string separator = ", ";

            foreach (IForm f in validated)
            {
                if (sb.Length != 0)
                {
                    sb.Append(separator);
                }
                sb.Append(f.Name);
            }

            return sb.ToString();
        }

        public static bool QueryServerAndSaveToFile(string credentialsXml)
        {
            var transceiver = new XMLTransceiver(Config.ClientURL);

            var sb = new StringBuilder();
            sb.AppendFormat(queryXML, credentialsXml);
            transceiver.Transmit(sb.ToString());

            string result = transceiver.Receive();

            if (result.Contains("<datasources>"))
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(result);
                xmlDocument.Save(Path.Combine(Config.LocalApplicationData, "SharedDataSources.xml"));

                return true;
            }

            return false;
        }

        public static void LoadDataSourcesFromFile()
        {
            string dataSourcesFile = Path.Combine(Config.LocalApplicationData, "SharedDataSources.xml");
            if (File.Exists(dataSourcesFile))
            {
                try
                {
                    string xmlString = File.ReadAllText(dataSourcesFile);
                    IXmlElement element = new XmlElement(xmlString);
                    initialize(element.GetChild("datasources"));
                }
                catch
                {
                }
            }
        }

        #region Private

        private static FormList externalForms = new FormList();

        private static Collection<ExternalFieldProvider> providers = new Collection<ExternalFieldProvider>();

        private static void initialize(IXmlElement element)
        {
            providers = new Collection<ExternalFieldProvider>();
            externalForms = new FormList();

            Collection<XmlElement> dataSourceElements = element.GetChildren("datasource");

            foreach (IXmlElement dataSourceElement in dataSourceElements)
            {
                providers.Add(new ExternalFieldProvider(dataSourceElement));
            }

            foreach (ExternalFieldProvider provider in providers)
            {
                var externalForm = new ExternalForm(provider.Name);
                foreach (ExternalField field in provider.Fields)
                {
                    if (field.IsMultipleChoice)
                    {
                        var mcItem = new McqItem(externalForm, field.FieldName, field.ChoiceCount, field.SelectOnlyOne);
                        externalForm.ItemList.Add(mcItem);
                    }
                    else
                    {
                        var fibItem = new FibItem(externalForm, field.FieldName);
                        externalForm.ItemList.Add(fibItem);
                    }
                }
                externalForms.Add(externalForm);
            }

            initialized = true;
        }

        #endregion
    }
}