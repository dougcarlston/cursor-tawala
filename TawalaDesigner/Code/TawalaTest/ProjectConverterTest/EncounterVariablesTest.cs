using System;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using System.Reflection;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Processes;
using TawalaTest.ProjectTest;

namespace ProjectConverterTest
{
	/// <summary>
	/// Class to test variables referenced across Processes and Skip Instructions.
	/// </summary>
	[TestFixture]
	public class EncounterVariablesTest : TestBase
	{
		TawalaProjectConverter converter;

		[SetUp]
		public void Setup()
		{
			converter = GetConverter("EncounterVariables.xml");
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
			Assert.AreEqual(2, Project.Current.FormList.Count);
			Form form1 = (Form)Project.Current.FormList[0];

			// verify valid Skip Instructions Items
			SkipInstructionsItem skipItem_1 = (SkipInstructionsItem)form1.ItemList[0];
			Assert.IsNotNull(skipItem_1, "SkipInstructions item should not be null");

			SkipInstructionsItem skipItem_2 = (SkipInstructionsItem)form1.ItemList[1];
			Assert.IsNotNull(skipItem_2, "SkipInstructions item should not be null");

			SkipInstructionsItem skipItem_3 = (SkipInstructionsItem)form1.ItemList[2];
			Assert.IsNotNull(skipItem_3, "SkipInstructions item should not be null");

			// verify valid Processes
			Assert.AreEqual(2, Project.Current.ProcessList.Count);
			Process process_1 = (Process)Project.Current.ProcessList[0];
			Assert.AreEqual("Set VariableFromProcess1 to 100", process_1.Lines[0].ToString());

			Process process_2 = (Process)Project.Current.ProcessList[1];
			Assert.AreEqual("Set VariableFromProcess2 to 100", process_2.Lines[0].ToString());

			// The first and third Skip Instructions in Form 1 create a variables to test
			Assert.AreEqual("Set EncounterVariable to 100", skipItem_1.Instructions.Lines[0].ToString());
			Assert.AreEqual("Set EncounterVariable2 to 100", skipItem_3.Instructions.Lines[0].ToString());

			// The second Skip Instructions in Form 1 test references to variables created elsewhere
			// test reference to variable created in previous Skip Instructions
			Assert.AreEqual("If EncounterVariable equals 100", skipItem_2.Instructions.Lines[0].ToString());

			// test reference to variable created in subsequent Skip Instructions
			Assert.AreEqual("If EncounterVariable2 equals 100", skipItem_2.Instructions.Lines[3].ToString());

			// test reference to variable created in a Process
			Assert.AreEqual("If VariableFromProcess1 equals 100", skipItem_2.Instructions.Lines[6].ToString());

			// REVISIT: add statement to second Skip Instructions in Form 1: "If VariableFromForm2 equals 100"
			//// test reference to variable created in Skip Instructions in another FOrm
			//Assert.AreEqual("If VariableFromForm2 equals 100", skipItem_2.Instructions.Lines[9].ToString());

			// verify that variable created in Skip Instructions can be referenced in Process
			Assert.AreEqual("If EncounterVariable equals 100", process_1.Lines[1].ToString());

			// verify that variable created in another Process can be referenced
			Assert.AreEqual("If VariableFromProcess2 equals 100", process_1.Lines[4].ToString());
		}
	}
}
