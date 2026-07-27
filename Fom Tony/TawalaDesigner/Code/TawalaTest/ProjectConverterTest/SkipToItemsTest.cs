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
	/// Class to test conversion of form items of type SkipInstructionsItem.
	/// </summary>
	[TestFixture]
	public class SkipToItemsTest : TestBase
	{
		TawalaProjectConverter converter;

		[SetUp]
		public void Setup()
		{
			converter = GetConverter("SkipInstructionsItems.xml");
			converter.ConvertXmlToProject();
		}

		[Test]
		public void XmlValidation()
		{
			converter = null;
			RoundtripProjectXml();
		}

		private SkipInstructionsItem getSkipInstructionsItem(int itemIndex)
		{
			// create XML reader
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.IgnoreWhitespace = true;

			XmlReader reader = XmlReader.Create(TawalaTest.TestSupport.Util.GetTestFilePath("SkipInstructionsItems.xml"), settings);

			int i = 0;

			// advance reader to specified item
			do
			{
				reader.ReadToFollowing("skipInstructions");

			} while (i++ < itemIndex);

			// get converter's private getSkipInstructionsItem method
			MethodInfo method = GetConverterMethodInfo("getSkipInstructionsItem");

			// create arguments appropriate for getSkipInstructionsItem method
			Object[] args = new object[1];
			args[0] = reader;

			// invoke getSkipInstructionsItem method
			SkipInstructionsItem skipInstructionsItem = (SkipInstructionsItem)method.Invoke(converter, args);

			return skipInstructionsItem;
		}


		/// <summary>
		/// Tests the getSkipInstructionsItem method of the TawalaProjectConverter class.
		/// </summary>
		[Test]
		public void GetSkipInstructionsItems()
		{
			SkipInstructionsItem skipInstructionsItem;

			skipInstructionsItem = getSkipInstructionsItem(0);
			Assert.IsNotNull(skipInstructionsItem, "SkipInstructions item should not be null");
			Assert.AreEqual("Skip Instructions Item", skipInstructionsItem.FieldName);

			skipInstructionsItem = getSkipInstructionsItem(1);
			Assert.IsNotNull(skipInstructionsItem, "SkipInstructions item should not be null");
			Assert.AreEqual("Skip Instructions Item", skipInstructionsItem.FieldName);

			skipInstructionsItem = getSkipInstructionsItem(2);
			Assert.IsNotNull(skipInstructionsItem, "SkipInstructions item should not be null");
			Assert.AreEqual("Skip Instructions Item", skipInstructionsItem.FieldName);

			skipInstructionsItem = getSkipInstructionsItem(3);
			Assert.IsNotNull(skipInstructionsItem, "SkipInstructions item should not be null");
			Assert.AreEqual("Skip Instructions Item", skipInstructionsItem.FieldName);
		}


		private void setSkipInstructionsVariables(int skipItemIndex, Process instructions)
		{
			// get converter's private setSkipInstructionsVariables method
			MethodInfo method = GetConverterMethodInfo("setSkipInstructionsVariables");

			// create arguments appropriate for setSkipInstructionsVariables method
			Object[] args = new object[3];
			args[0] = 0;
			args[1] = skipItemIndex;
			args[2] = instructions;

			// invoke setSkipInstructionsVariables method
			method.Invoke(converter, args);
		}

		/// <summary>
		/// Tests the setSkipInstructionsVariables method of the TawalaProjectConverter class.
		/// </summary>
		[Test]
		public void SetSkipInstructionsVariables()
		{
			SkipInstructionsItem skipInstructionsItem;

			int itemIndex = 0;
			skipInstructionsItem = getSkipInstructionsItem(itemIndex);
			Assert.AreEqual(0, skipInstructionsItem.Instructions.Variables.Count);
			setSkipInstructionsVariables(itemIndex, skipInstructionsItem.Instructions);
			Assert.AreEqual(0, skipInstructionsItem.Instructions.Variables.Count);

			itemIndex = 1;
			skipInstructionsItem = getSkipInstructionsItem(itemIndex);
			Assert.AreEqual(0, skipInstructionsItem.Instructions.Variables.Count);
			setSkipInstructionsVariables(itemIndex, skipInstructionsItem.Instructions);
			Assert.AreEqual(2, skipInstructionsItem.Instructions.Variables.Count);
			Assert.AreEqual("Var", skipInstructionsItem.Instructions.Variables[0].ToString());
			Assert.AreEqual("NewVar", skipInstructionsItem.Instructions.Variables[1].ToString());

			itemIndex = 2;
			skipInstructionsItem = getSkipInstructionsItem(itemIndex);
			Assert.AreEqual(0, skipInstructionsItem.Instructions.Variables.Count);
			setSkipInstructionsVariables(itemIndex, skipInstructionsItem.Instructions);
			Assert.AreEqual(2, skipInstructionsItem.Instructions.Variables.Count);
			Assert.AreEqual("Var", skipInstructionsItem.Instructions.Variables[0].ToString());
			Assert.AreEqual("NewVar", skipInstructionsItem.Instructions.Variables[1].ToString());

			itemIndex = 3;
			skipInstructionsItem = getSkipInstructionsItem(itemIndex);
			Assert.AreEqual(0, skipInstructionsItem.Instructions.Variables.Count);
			setSkipInstructionsVariables(itemIndex, skipInstructionsItem.Instructions);
			Assert.AreEqual(2, skipInstructionsItem.Instructions.Variables.Count);
			Assert.AreEqual("Var", skipInstructionsItem.Instructions.Variables[0].ToString());
			Assert.AreEqual("NewVar", skipInstructionsItem.Instructions.Variables[1].ToString());
		}


		/// <summary>
		/// Verifies that the converted Project contains a conditional skip to a question label
		/// </summary>
		[Test]
		public void SkipToQuestionConditional()
		{
			Form form = (Form)Project.Current.FormList[0];

			SkipInstructionsItem skipToItem = (SkipInstructionsItem)form.ItemList[1];
			Process skipInstructions = skipToItem.Instructions;

			Assert.AreEqual(4, skipInstructions.Lines.Count);
			Assert.AreEqual("If Form 1:Q1:a is blank", skipInstructions.Lines[0].ToString());
			Assert.AreEqual("(", skipInstructions.Lines[1].ToString());
			Assert.AreEqual("Skip to Q3", skipInstructions.Lines[2].ToString());
			Assert.AreEqual(")", skipInstructions.Lines[3].ToString());
		}


		/// <summary>
		/// Verifies that the converted Project contains an unconditional skip to end of form
		/// </summary>
		[Test]
		public void SkipToEndOfForm()
		{
			Form form = (Form)Project.Current.FormList[0];

			SkipInstructionsItem skipToItem = (SkipInstructionsItem)form.ItemList[5];
			Process skipInstructions = skipToItem.Instructions;

			Assert.AreEqual(1, skipInstructions.Lines.Count);
			Assert.AreEqual("Skip to End of Form", skipInstructions.Lines[0].ToString());
		}


		/// <summary>
		/// Tests the conversion of the XML file to a Tawala Project.
		/// </summary>
		[Test]
		public void ConvertXmlToProject()
		{
			IForm form;

			form = (Form)Project.Current.FormList[0];

			// verify that project contains 1 form
			Assert.AreEqual(2, Project.Current.FormList.Count);
			Assert.AreEqual("Form 1", form.Name);

			// verify that form contains 3 SkipInstructions items
			Assert.AreEqual(7, form.ItemList.Count);
			Assert.AreEqual(typeof(SkipInstructionsItem), form.ItemList[1].GetType());
			Assert.AreEqual(typeof(SkipInstructionsItem), form.ItemList[2].GetType());
			Assert.AreEqual(typeof(SkipInstructionsItem), form.ItemList[5].GetType());

			// verify that instructions are present
			SkipInstructionsItem item1 = (SkipInstructionsItem)form.ItemList[1];
			Assert.IsNotNull(item1.Instructions, "item1 is null");
			Assert.AreEqual("If Form 1:Q1:a is blank", item1.Instructions.Lines[0].ToString());

			SkipInstructionsItem item2 = (SkipInstructionsItem)form.ItemList[2];
			Assert.IsNotNull(item1.Instructions, "item2 is null");
			Assert.AreEqual("Set Var to 1", item2.Instructions.Lines[0].ToString());
			Assert.AreEqual("If Var equals 1", item2.Instructions.Lines[1].ToString());
			Assert.AreEqual("(", item2.Instructions.Lines[2].ToString());
			Assert.AreEqual("Add 1 to Var", item2.Instructions.Lines[3].ToString());
			Assert.AreEqual(")", item2.Instructions.Lines[4].ToString());
			Assert.AreEqual("Add 10 to NewVar", item2.Instructions.Lines[5].ToString());
			item2.Instructions.Lines.ValidateLines();
			Assert.IsTrue(item2.Instructions.Lines[5].IsValid, "Instructions line 5 is invalid.");

			SkipInstructionsItem item3 = (SkipInstructionsItem)form.ItemList[5];
			Assert.IsNotNull(item3.Instructions, "item3 is null");
			Assert.AreEqual("Skip to End of Form", item3.Instructions.Lines[0].ToString());

			form = (Form)Project.Current.FormList[1];

			SkipInstructionsItem item4 = (SkipInstructionsItem)form.ItemList[0];
			Assert.IsNotNull(item4.Instructions, "item4 is null");
			Assert.AreEqual("If GlobalVar is not blank", item4.Instructions.Lines[0].ToString());

			// verify that ProcessLines' Process property is set correctly
			Assert.AreEqual(item1.Instructions, item1.Instructions.Lines.Process);
			Assert.AreEqual(item2.Instructions, item2.Instructions.Lines.Process);
			Assert.AreEqual(item3.Instructions, item3.Instructions.Lines.Process);
			Assert.AreEqual(item4.Instructions, item4.Instructions.Lines.Process);
		}

	}
}
