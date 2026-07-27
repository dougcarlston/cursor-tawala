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
	/// Class to test conversion of IF statements.
	/// </summary>
	[TestFixture]
	public class IfStatementTest : TestBase
	{
		TawalaProjectConverter converter;

		[SetUp]
		public void Setup()
		{
			converter = GetConverter("IfStatements.xml");
			converter.ConvertXmlToProject();
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
			// verify that project contains 1 form and 1 process
			Assert.AreEqual(1, Project.Current.FormList.Count);
			Assert.AreEqual(1, Project.Current.ProcessList.Count);
			Assert.AreEqual("Form 1", ((Form)Project.Current.FormList[0]).Name);
			Assert.AreEqual("Process 1", ((Process)Project.Current.ProcessList[0]).Name);

		}

		[Test]
		public void ConvertHybrid()
		{
			Process process = (Process)Project.Current.ProcessList[0];

			// verify that form and process are connected
            Assert.AreEqual(process, ((Form)Project.Current.FormList[0]).ConnectedPostProcess);

			// verify that process line is correct
			Assert.AreEqual("If Name is blank", process.Lines[0].ToString());
			Assert.AreEqual("If Name is not blank", process.Lines[3].ToString());
			Assert.AreEqual("If Form 1:Q2:a equals 1", process.Lines[6].ToString());
			Assert.AreEqual("If Form 1:Q2:a does not equal 2", process.Lines[9].ToString());
			Assert.AreEqual("If Form 1:Q2:a contains \"3\"", process.Lines[12].ToString());
			Assert.AreEqual("If Form 1:Q2:a does not contain \"4\"", process.Lines[15].ToString());
			Assert.AreEqual("If Form 1:Q2:a begins with \"5\"", process.Lines[18].ToString());
			Assert.AreEqual("If Form 1:Q2:a ends with \"6\"", process.Lines[21].ToString());
			Assert.AreEqual("If Form 1:Q2:a is less than 7", process.Lines[24].ToString());
			Assert.AreEqual("If Form 1:Q2:a is less than or equal to 8", process.Lines[27].ToString());
			Assert.AreEqual("If Form 1:Q2:a is greater than 9", process.Lines[30].ToString());
			Assert.AreEqual("If Form 1:Q2:a is greater than or equal to 10", process.Lines[33].ToString());

		}


		/// <summary>
		/// Tests the conversion of Multiple Choice operators in the XML file.
		/// </summary>
		[Test]
		public void ConvertMC()
		{
			Process process = (Process)Project.Current.ProcessList[0];

			// verify that form and process are connected
            Assert.AreEqual(process, ((Form)Project.Current.FormList[0]).ConnectedPostProcess);

			// verify that process line is correct
			Assert.AreEqual("If Q3 equals a", process.Lines[36].ToString());
			Assert.AreEqual("If Q3 does not equal b", process.Lines[39].ToString());
			Assert.AreEqual("If Q3 contains c", process.Lines[42].ToString());
			Assert.AreEqual("If Q3 does not contain d", process.Lines[45].ToString());

		}


		/// <summary>
		/// Tests the use of operators to compare fields (rather than literals).
		/// </summary>
		[Test]
		public void CompareFields()
		{
			Process process = (Process)Project.Current.ProcessList[0];

			// verify that form and process are connected
            Assert.AreEqual(process, ((Form)Project.Current.FormList[0]).ConnectedPostProcess);

			// verify that process lines are correct
			Assert.AreEqual("If IQ is less than Form 1:Q2:a", process.Lines[48].ToString());
			Assert.AreEqual("(", process.Lines[49].ToString());
			Assert.AreEqual("Show Document Document 1", process.Lines[50].ToString());
			Assert.AreEqual(")", process.Lines[51].ToString());

		}


		/// <summary>
		/// Tests the conversion of nested IF statements.
		/// </summary>
		[Test]
		public void NestedIfs()
		{
			Process process = (Process)Project.Current.ProcessList[0];

			// verify that form and process are connected
            Assert.AreEqual(process, ((Form)Project.Current.FormList[0]).ConnectedPostProcess);

			// verify that process lines are correct
			Assert.AreEqual("If Name is not blank", process.Lines[52].ToString());
			Assert.AreEqual("(", process.Lines[53].ToString());
			Assert.AreEqual("If Form 1:Q2:a is greater than or equal to 21", process.Lines[54].ToString());
			Assert.AreEqual("(", process.Lines[55].ToString());
			Assert.AreEqual("Show Document Document 2", process.Lines[56].ToString());
			Assert.AreEqual(")", process.Lines[57].ToString());
			Assert.AreEqual(")", process.Lines[58].ToString());

		}


		/// <summary>
		/// Verifies that all choices have been converted.
		/// </summary>
		[Test]
		public void Choices()
		{
			Form form = (Form)Project.Current.FormList[0];

			McqItem mcItem = (McqItem)form.ItemList[2];

			Assert.AreEqual(4, mcItem.Choices.Count);
			Assert.AreEqual("Apples", mcItem.Choices[0].Text);
			Assert.AreEqual("Bananas", mcItem.Choices[1].Text);
			Assert.AreEqual("Oranges", mcItem.Choices[2].Text);
			Assert.AreEqual("Kiwis", mcItem.Choices[3].Text);
		}


	}
}
