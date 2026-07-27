using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using Tawala.Processes;
using Tawala.Projects;
using Tawala.Common;
using Tawala.ProjectUI;
using Tawala.XmlSupport;

using TawalaTest.TestSupport;

using NUnit.Framework;

namespace TawalaTest.ProcessesTest
{
	[TestFixture]
	public class DeleteStatementViewTest
	{
		DeleteStatementView testView;

		[SetUp]
		public void Setup()
		{
			Util.NewTestProject();
			Project.Current.AddForm();
			testView = new DeleteStatementView();
			testView.CreateControl();
		}

		[TearDown]
		public void TearDown()
		{
			testView.Dispose();
		}

		[Test]
		public void ComboBoxDefaultsToFirstForm()
		{
			ComboBox comboBoxForms = Reflect<DeleteStatementView>.GetField<ComboBox>("comboBoxForms", testView);
			Assert.IsNotNull(comboBoxForms);
			Assert.AreEqual("Form 1", comboBoxForms.Text);
		}
	}
}
