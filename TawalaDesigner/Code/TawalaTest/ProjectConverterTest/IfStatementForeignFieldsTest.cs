using System;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using TawalaTest.ProjectTest;

namespace ProjectConverterTest
{
	/// <summary>
	/// Class to test conversion of IF statements containing foreign fields.
	/// </summary>
	[TestFixture]
	public class IfStatementForeignFieldsTest : TestBase
	{
		private TawalaProjectConverter converter;
		private Process process;
		private IForm form1;
		private IForm form2;
		private Blank form1Blank1;
		private Blank form2Blank1;

		[SetUp]
		public void Setup()
		{
			converter = GetConverter("IfStatementsForeignFields.xml");
			converter.ConvertXmlToProject();

			process = (Process)Project.Current.ProcessList[0];
			form1 = (Form)Project.Current.FormList[0];
			form2 = (Form)Project.Current.FormList[1];

			form1Blank1 = ((FibItem)form1.ItemList[0]).BlankList[0];
			form2Blank1 = ((FibItem)form2.ItemList[0]).BlankList[0];
		}

		[Test]
		public void XmlValidation()
		{
			converter = null;
			RoundtripProjectXml();
		}

		[Test]
		public void ConvertXmlToProject()
		{
			Assert.AreEqual(2, Project.Current.FormList.Count);
			Assert.AreEqual(1, Project.Current.ProcessList.Count);
			Assert.AreEqual("Form 1", ((Form)Project.Current.FormList[0]).Name);
			Assert.AreEqual("Form 2", ((Form)Project.Current.FormList[1]).Name);
			Assert.AreEqual("Process 1", ((Process)Project.Current.ProcessList[0]).Name);
		}


		[Test]
		public void QualifiedBlankEqualsQualifiedBlank()
		{
			Assert.AreEqual("If Form 1:Q1:a equals Form 2:Q1:a", process.Lines[0].ToString());
			process.Lines.ValidateLines();
			Assert.IsTrue(process.Lines[0].IsValid);
		}

		[Test]
		public void MappedFields()
		{
			Assert.AreEqual(4, process.MappedFields.Count);
			Assert.AreSame(form1Blank1, process.MappedFields["Q1:a"]);
			Assert.AreSame(form1Blank1, process.MappedFields["Form 1:Q1:a"]);
			Assert.AreSame(form2Blank1, process.MappedFields["Form 2:Q1:a"]);
		}

	}
}
