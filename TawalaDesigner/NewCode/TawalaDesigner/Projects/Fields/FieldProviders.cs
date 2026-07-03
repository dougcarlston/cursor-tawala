// $Workfile: FieldProviders.cs $
// $Revision: 17 $	$Date: 8/23/07 12:24p $
// Copyright © 2007 Tawala Systems, Inc. All rights reserved.using System;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Reflection;
using System.IO;

using Tawala.Common;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	public static class FieldProviders
	{
		private static volatile bool initialized = false;

		public static bool IsInitialized
		{
			get { return initialized; }
		}

		public static FormList ExternalForms
		{
			get { return externalForms; }
			set { externalForms = value; }
		}

		public static FormList CreateValidFormList(FormList unvalidated)
		{
			FormList current = Project.Current.AllForms;
			FormList validated = new FormList();

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

			StringBuilder sb = new StringBuilder();
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

		private static readonly string queryXML = 
			"<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
			"<request type=\"queryDataSources\" protocol=\"1.0\">" +
			    "{0}" +
			"</request>";


		public static bool QueryServerAndSaveToFile(string credentialsXml)
		{
			XMLTransceiver transceiver = new XMLTransceiver(Config.ClientURL);

			StringBuilder sb = new StringBuilder();
			sb.AppendFormat(queryXML, credentialsXml);
			transceiver.Transmit(sb.ToString());

			string result = transceiver.Receive();

			if (result.Contains("<datasources>"))
			{
				System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
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
				IExternalForm externalForm = new ExternalForm(provider.Name);
				foreach (ExternalField field in provider.Fields)
				{
					if (field.IsMultipleChoice)
					{
						var mcqItem = new NewMcqItem(externalForm, field.FieldName, field.ChoiceCount, field.SelectOnlyOne);
						externalForm.ItemList.Add(mcqItem);
					}
					else
					{
						var fibItem = new NewFibItem(externalForm, field.FieldName);
						externalForm.ItemList.Add(fibItem);
					}
				}
				externalForms.Add(externalForm);
			}

			initialized = true;
		}

		private static Collection<ExternalFieldProvider> providers = new Collection<ExternalFieldProvider>();

		#endregion
	}
}
