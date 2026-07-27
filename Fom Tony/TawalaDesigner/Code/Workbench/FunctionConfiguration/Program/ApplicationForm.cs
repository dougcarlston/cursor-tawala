using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Tawala.XmlSupport;
using Tawala.FunctionConfiguration;
using Tawala.Proj;
using Tawala.ProjectUI;
using Tawala.Common;

namespace Program
{
	public partial class ApplicationForm : System.Windows.Forms.Form
	{
		private Tawala.Proj.Form form1;
		private Tawala.Proj.Form form2;
		private MCItem mcItem1;
		private MCItem mcItem2;
		private FibItem fibItem;
		private Blank blank;
		private IConfigureFunctionPresenter presenter;
		private IXmlElement functionSpecificationElement;
		private IConfiguredFunction configuredFunction;

		public ApplicationForm()
		{
			InitializeComponent();

			setupFieldsPalette();
			setupProject();

			ConfigureFunctionPresenter.FunctionConfigurationCompleted += new EventHandler<ConfigurationCompletedEventArgs>(functionConfigurationCompleted);
			Application.Idle += new EventHandler(application_Idle);

			RepositoryXmlRetriever.RetrieveRepositoryXmlOnBackgroundThread();
		}

		private void setupFieldsPalette()
		{
			fieldsPalette.Dock = DockStyle.None;
		}

		private void setupProject()
		{
			Project.New();

			form1 = Project.Current.AddForm();
			form2 = Project.Current.AddForm();

			mcItem1 = new MCItem();
			form1.Add(mcItem1);
			mcItem2 = new MCItem();
			form1.Add(mcItem2);

			fibItem = new FibItem();
			form1.Add(fibItem);
			blank = fibItem.BlankList[0];
		}

		void application_Idle(object sender, EventArgs e)
		{
			buttonSelectFunction.Enabled = RepositoryXmlRetriever.XmlIsAvailable;
			buttonReconfigureLatest.Enabled = (functionSpecificationElement != null && configuredFunction != null);
		}

		void functionConfigurationCompleted(object sender, ConfigurationCompletedEventArgs e)
		{
			configuredFunction = e.ConfiguredFunction;

			labelOutputXml.Text = String.Format("Output XML for function with ID = {0}", configuredFunction.InstanceId);
			textBoxOutputXml.Text = configuredFunction.ToXml();
		}

		private void buttonSelectFunction_Click(object sender, EventArgs e)
		{
			SelectFunctionDialog selectFunctionDialog = new SelectFunctionDialog();
			DialogResult result = selectFunctionDialog.ShowDialog();

			if (result == DialogResult.OK)
			{
				functionSpecificationElement = selectFunctionDialog.FunctionSpecificationElement;
			}
		}

		private void buttonReconfigureLatest_Click(object sender, EventArgs e)
		{
			IXmlElement functionElement = new XmlElement(configuredFunction.ToXml());
			presenter = new ConfigureFunctionPresenter(new ConfigureFunctionDialogPhase2(), functionSpecificationElement, functionElement);

			presenter.ConfigureFunction();

		}
	}

	public static class RepositoryXmlRetriever
	{
		private static string repositoryXml = "";

		public static void Retrieve()
		{
			repositoryXml = FunctionRepositoryInfo.QueryServerRepository("", "");

			repositoryXml = ItemizationTableParameterFlattener.Flatten(repositoryXml);
		}

		public static bool XmlIsAvailable
		{
			get { return repositoryXml != ""; }
		}

		public static string RepositoryXml
		{
			get { return repositoryXml; }
		}

		private static Thread thread;

		public static void RetrieveRepositoryXmlOnBackgroundThread()
		{
			ThreadStart entry = new ThreadStart(RepositoryXmlRetriever.Retrieve);
			thread = new Thread(entry);
			thread.Priority = ThreadPriority.Normal;
			thread.IsBackground = true;
			thread.Start();
		}
	}

	public static class ItemizationTableParameterFlattener
	{
		private static int numberOfColumns;
		private static IXmlElement contentsElement;
		private static IXmlElement headerElement;

		public static string Flatten(string repositoryXmlString)
		{
			IXmlElement repositoryElement = new XmlElement(repositoryXmlString);
			
			string name = repositoryElement.Name;
			string signature = repositoryElement.GetAttribute("signature");

			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("<{0} xmlns:tr=\"http://www.tawala.com/component-repository\" signature=\"{1}\">", name, signature);

			IXmlElement element = new XmlElement(repositoryXmlString);

			Collection<XmlElement> childElements = element.GetChildren();

			foreach (IXmlElement childElement in childElements)
			{
				if (childElement.Name == "tr:display-component" && childElement.GetAttribute("name") == "ITEMIZATION TABLE")
				{
					numberOfColumns = getNumberOfColumns(element);
					contentsElement = getContentsElement(element);
					headerElement = getHeaderElement(element);

					sb.Append(toOutputXml());
				}
				else
				{
					sb.Append(childElement.OuterXml);
				}
			}

			sb.AppendFormat("</{0}>", name);

			return sb.ToString();
		}

		private static int getNumberOfColumns(IXmlElement element)
		{
			return Convert.ToInt32(element.GetDescendants("tr:maximum")[0].GetAttribute("value"));
		}

		/// <summary>
		/// Extracts a &lt;contents&gt; XML element from the specified XML element
		/// </summary>
		private static IXmlElement getContentsElement(IXmlElement element)
		{
			Collection<XmlElement> parameterElements = element.GetDescendants("tr:parameter");

			foreach (IXmlElement parameterElement in parameterElements)
			{
				if (parameterElement.GetAttribute("id") == "contents")
				{
					return parameterElement;
				}
			}

			return null;
		}

		/// <summary>
		/// Extracts a &lt;header&gt; XML element from the specified XML element
		/// </summary>
		private static IXmlElement getHeaderElement(IXmlElement element)
		{
			Collection<XmlElement> parameterElements = element.GetDescendants("tr:parameter");

			foreach (IXmlElement parameterElement in parameterElements)
			{
				if (parameterElement.GetAttribute("id") == "header")
				{
					return parameterElement;
				}
			}

			return null;
		}

		private static string toOutputXml()
		{
			StringBuilder sb = new StringBuilder();

			string id = "itemization-table";
			string name = "ITEMIZATION TABLE";
			string version = "1";

			sb.AppendFormat("<tr:display-component id=\"{0}\" name=\"{1}\" version=\"{2}\">", id, name, version);

			for (int i = 0; i < numberOfColumns; i++)
			{
				sb.Append(contentsElement.OuterXml);
				sb.Append(headerElement.OuterXml);
				sb.Replace("$n$", String.Format("{0}", i + 1));
			}

			sb.Append("</tr:display-component>");

			return sb.ToString();
		}
	}
}