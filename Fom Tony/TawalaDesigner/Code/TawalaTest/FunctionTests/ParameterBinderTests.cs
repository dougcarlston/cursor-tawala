using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Net;
using System.Xml.XPath;
using System.IO;
using System.Windows.Forms;
using NUnit.Framework;
using Tawala.Projects.Forms;
using TawalaTest.TestSupport;
using Tawala.Functions.Runtime;
using Tawala.Functions.Controls;
using Tawala.Functions.ViewPresenter;
using Tawala.Projects;
using Tawala.XmlSupport;

namespace TawalaTest.FunctionTests
{
    [TestFixture]
	public class ParameterBinderTests : FunctionTestBase
    {
		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			FixtureSetup();
		}

		private IForm form;
		private FibItem fibItem;
		private NMock2.Mockery mockery;

		private const string NEWLINE = "\r\n";

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();

			form = Project.Current.AddForm();

			fibItem = new FibItem();
			form.ItemList.Add(fibItem);

			mockery = new NMock2.Mockery();

			initializeTestDataSources();
		}

		//[Test]
		//public void FormBinderContainsFormsAndSharedDataSources()
		//{
		//    try
		//    {
		//        IFunctionInfo functionInfo = functionRepository.Functions["record-count"];
		//        ConfigureFunctionDialog.Presenter.CreateFunction(functionInfo, configureFunctionCallback);
		//        ConfigureFunctionControl configure = ConfigureFunctionDialog.Current.Controls[0] as ConfigureFunctionControl;
		//        Assert.That(configure != null);

		//        ComboBox found = findControlByTypeName(configure, "ParameterComboBox") as ComboBox;
		//        Assert.That(found != null);
		//    }
		//    finally
		//    {
		//        ConfigureFunctionDialog.Current.Close();
		//    }
		//}

		private void configureFunctionCallback(object sender, FunctionConfiguredEventArgs args)
		{
		}

		private Control findControlByTypeName(Control parent, string typeName)
		{
			Control found = null;

			foreach (Control c in parent.Controls)
			{
				if (c.GetType().Name.CompareTo(typeName) == 0)
				{
					found = c;
					break;
				}
				else
				{
					found = findControlByTypeName(c, typeName);
					break;
				}
			}

			return found;
		}

		private const string testDataSourceXml =
		"<datasources>" +
		"<datasource name=\"ClientInfo\">" +
		"<field name=\"Q1:a\" type=\"string\"/>" +
		"<field name=\"name\" type=\"string\"/>" +
		"<field name=\"Q3\" type=\"mcq\" choices=\"2\" onlyone=\"true\"/>" +
		"</datasource>" +
		"<datasource name=\"DataSource2\">" +
		"<field name=\"Q1:a\" type=\"string\"/>" +
		"<field name=\"Q2\" type=\"mcq\" choices=\"3\" onlyone=\"false\"/>" +
		"</datasource>" +
		"</datasources>";

		private void initializeTestDataSources()
		{
			initialize(testDataSourceXml);
		}

		private void initialize(string dataSourcesXml)
		{
			MethodInfo init = typeof(FieldProviders).GetMethod("initialize", BindingFlags.NonPublic | BindingFlags.Static);
			init.Invoke(null, new object[] { new XmlElement(dataSourcesXml) });
		}
	}
}
