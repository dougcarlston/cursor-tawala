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
using NUnit.Framework;

namespace TawalaTest.ProcessesTest
{
	[TestFixture]
	public class GetStatementViewTest
	{
		GetStatementView testView;

		BindingFlags flags =
			BindingFlags.NonPublic |
			BindingFlags.Public |
			BindingFlags.Static |
			BindingFlags.Instance;

		Type tDetails = typeof(GetStatementView);


		[SetUp]
		public void Setup()
		{
			Project.NewTestProject();
			testView = new GetStatementView();
		}

		[TearDown]
		public void TearDown()
		{
			testView.Dispose();
		}

		private TextBox textBoxRecordSets()
		{
			FieldInfo controlInfo = tDetails.GetField("textBoxRecordSet", flags);
			return ((TextBox)controlInfo.GetValue(testView));
		}


		[Test]
		public void RecordListNameAppearsByDefault()
		{
			Assert.AreEqual("Record List 1", textBoxRecordSets().Text);
		}
	}
}
