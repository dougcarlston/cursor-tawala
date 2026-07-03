using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using TawalaTest.ProjectTest;

namespace ProjectConverterTest
{
	/// <summary>
	/// Class to test conversion of SET statements.
	/// </summary>
	[TestFixture]
	public class SetStatementTest : TestBase
	{
		TawalaProjectConverter converter;

		[SetUp]
		public void Setup()
		{
			converter = GetConverter("SetStatements.xml");
			converter.ConvertXmlToProject();
		}

		[Test]
		public void XmlValidation()
		{
			converter = null;
			RoundtripProjectXml();
		}

		[Test]
		public void CheckProcessComponents()
		{
			Assert.AreEqual(2, Project.Current.ProcessList.Count);
			Assert.AreEqual("Process 1", ((Process)Project.Current.ProcessList[0]).Name);
			Assert.AreEqual("Process 1", Project.Current.GetProcess("Process 1").Name);
			Assert.AreEqual("Process 2", ((Process)Project.Current.ProcessList[1]).Name);
			Assert.AreEqual("Process 2", Project.Current.GetProcess("Process 2").Name);

			IForm form = Project.Current.GetForm("Form 1");

			FibItem fibItem1 = (FibItem)form.ItemList[0];
			Assert.AreEqual("Age", fibItem1.BlankList[0].AlternateLabel);
			Assert.AreEqual("IQ", fibItem1.BlankList[1].AlternateLabel);

			FibItem fibItem2 = (FibItem)form.ItemList[1];
			Assert.AreEqual("FirstName", fibItem2.BlankList[0].AlternateLabel);
			Assert.AreEqual("LastName", fibItem2.BlankList[1].AlternateLabel);
		}


		/// <summary>
		/// Gets process lines from the XML file.
		/// </summary>
		[Test]
		public void GetProcessLines()
		{
			Assert.AreEqual(2, Project.Current.ProcessList.Count);
			Collection<string> processNames = converter.GetProcessNames();

			ProcessLineList processLines = converter.GetProcessLines(processNames[0]);
			Assert.AreEqual(12, processLines.Count);
			Assert.AreEqual("Set x to a + b", processLines[0].ToString());
			Assert.AreEqual("Set x to a - b", processLines[1].ToString());
			Assert.AreEqual("Set x to a * b", processLines[2].ToString());
			Assert.AreEqual("Set x to a / b", processLines[3].ToString());
			Assert.AreEqual("Set x to (a + b) * c", processLines[4].ToString());
			Assert.AreEqual("Set x to ((a + b) * c) / d", processLines[5].ToString());
			Assert.AreEqual("Set Name to \"Steve\"", processLines[6].ToString());

			Assert.AreEqual("Set age to Age", processLines[7].ToString());
			Assert.AreEqual("Set FullName to \"<<FirstName>> <<LastName>>\"", processLines[8].ToString());
			Assert.AreEqual("Set AgePlusIQ to <<Age>> + <<IQ>>", processLines[9].ToString());
			Assert.AreEqual("Set AgePlusIQDividedBy10 to (<<Age>> + <<IQ>>) / 10", processLines[10].ToString());

			processLines = converter.GetProcessLines(processNames[1]);
			Assert.AreEqual(7, processLines.Count);
			Assert.AreEqual("Set Proc2Var1 to 10", processLines[0].ToString());
			Assert.AreEqual("Get Record List from Form 1", processLines[1].ToString());
			Assert.AreEqual("For Each record in Record List", processLines[2].ToString());
			Assert.AreEqual("(", processLines[3].ToString());
			Assert.AreEqual("Set record:Proc2Var1 to 20", processLines[4].ToString());
			Assert.AreEqual("Set record:Proc2Var2 to 30", processLines[5].ToString());
			Assert.AreEqual(")", processLines[6].ToString());
		}

		/// <summary>
		/// Tests the conversion of the XML file to a Tawala Project.
		/// </summary>
		[Test]
		public void ConvertXmlToProject()
		{
			// verify that project contains 1 form and 1 process
			Assert.AreEqual(1, Project.Current.FormList.Count);
			Assert.AreEqual(2, Project.Current.ProcessList.Count);
			Assert.AreEqual("Form 1", ((Form)Project.Current.FormList[0]).Name);
			Assert.AreEqual("Process 1", ((Process)Project.Current.ProcessList[0]).Name);
			Assert.AreEqual("Process 2", ((Process)Project.Current.ProcessList[1]).Name);

			Process process1 = (Process)Project.Current.ProcessList[0];

			// verify that form and process are connected
            Assert.AreEqual(process1, ((Form)Project.Current.FormList[0]).ConnectedPostProcess);

			Assert.AreEqual("Set x to a + b", process1.Lines[0].ToString());
			Assert.AreEqual("Set x to a - b", process1.Lines[1].ToString());
			Assert.AreEqual("Set x to a * b", process1.Lines[2].ToString());
			Assert.AreEqual("Set x to a / b", process1.Lines[3].ToString());
			Assert.AreEqual("Set x to (a + b) * c", process1.Lines[4].ToString());
			Assert.AreEqual("Set x to ((a + b) * c) / d", process1.Lines[5].ToString());
			Assert.AreEqual("Set Name to \"Steve\"", process1.Lines[6].ToString());

			Assert.AreEqual("Set age to Age", process1.Lines[7].ToString());
			Assert.AreEqual("Set FullName to \"<<FirstName>> <<LastName>>\"", process1.Lines[8].ToString());
			Assert.AreEqual("Set AgePlusIQ to <<Age>> + <<IQ>>", process1.Lines[9].ToString());
			Assert.AreEqual("Set AgePlusIQDividedBy10 to (<<Age>> + <<IQ>>) / 10", process1.Lines[10].ToString());

			Assert.AreEqual("Set Cost to 3.95", process1.Lines[11].ToString());

			Process process2 = (Process)Project.Current.ProcessList[1];
			Assert.AreEqual("Set Proc2Var1 to 10", process2.Lines[0].ToString());
			Assert.AreEqual("Get Record List from Form 1", process2.Lines[1].ToString());
			Assert.AreEqual("For Each record in Record List", process2.Lines[2].ToString());
			Assert.AreEqual("(", process2.Lines[3].ToString());
			Assert.AreEqual("Set record:Proc2Var1 to 20", process2.Lines[4].ToString());
			Assert.AreEqual("Set record:Proc2Var2 to 30", process2.Lines[5].ToString());
			Assert.AreEqual(")", process2.Lines[6].ToString());
		}

		/// <summary>
		/// Verifies that all variables have been properly regenerated.
		/// </summary>
		[Test]
		public void CheckVariables()
		{
			Process process1 = (Process)Project.Current.ProcessList[0];
			Process process2 = (Process)Project.Current.ProcessList[1];
			Assert.AreEqual(7, process1.Variables.Count);
			Assert.AreEqual(1, process2.Variables.Count);
			Assert.AreEqual(process1.Variables.Count + process2.Variables.Count, Project.Current.AllVariables.Count - 1);

			ArrayList variableNames1 = new ArrayList();
			variableNames1.Add("x");
			variableNames1.Add("Name");
			variableNames1.Add("age");
			variableNames1.Add("FullName");
			variableNames1.Add("AgePlusIQ");
			variableNames1.Add("AgePlusIQDividedBy10");
			variableNames1.Add("Cost");

			foreach (Variable var in process1.Variables)
			{
				Assert.IsTrue(variableNames1.Contains(var.FieldName), "\"{0}\" unexpected in Process 1 Variables list.", var.FieldName);
			}
			Assert.AreEqual(process1.Variables.Count, variableNames1.Count);

			Assert.AreEqual("Proc2Var1", process2.Variables[0].FieldName);
			
			Form form = (Form)Project.Current.FormList[0];
			//Assert.AreEqual(2, form.RecordVariables.Count);
			//Assert.AreEqual("Proc2Var1", form.RecordVariables[0].FieldName);
			//Assert.AreEqual("Proc2Var2", form.RecordVariables[1].FieldName);
		}
	}
}
