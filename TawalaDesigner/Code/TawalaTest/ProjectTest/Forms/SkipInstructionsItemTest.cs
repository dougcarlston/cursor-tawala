// $Workfile: SkipInstructionsItemTest.cs $
// $Revision: 19 $	$Date: 8/29/06 11:11p $
// Copyright © 2005 Douglas G. Carlston. All rights reserved.

using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the TestFormItem class
	/// </summary>
	[TestFixture]
	public class SkipInstructionsItemTest
	{
		[Test]
		public void NewSkipInstructionsItem()
		{
			SkipInstructionsItem item = new SkipInstructionsItem();

			//Assertions 
			Assert.IsNotNull(item.Instructions);
			Assert.AreEqual(0, item.Instructions.Lines.Count);
			Assert.AreEqual(false, item.IsTextItem);
			Assert.AreEqual(false, item.IsQuestionItem);
		}

		[Test]
		public void EditInstructions()
		{
			SkipInstructionsItem item = new SkipInstructionsItem();

			// add a Set statement
			SetStatement statement1 = new SetStatement();
			statement1.Variable = new Variable("a variable");
			statement1.Expression = new Expression("a value");
			item.Instructions.Lines.Add(new SetLine(statement1));

			Assert.AreEqual(1, item.Instructions.Lines.Count);

			// add another Set statement
			SetStatement statement2 = new SetStatement();
			statement2.Variable = new Variable("another variable");
			statement2.Expression = new Expression("a different value");
			item.Instructions.Lines.Add(new SetLine(statement2));

			Assert.AreEqual(2, item.Instructions.Lines.Count);

			// add another Set statement
			SetStatement statement3 = new SetStatement();
			statement3.Variable = new Variable("yet another variable");
			statement3.Expression = new Expression("and yet a different value");
			item.Instructions.Lines.Add(new SetLine(statement3));

			Assert.AreEqual(3, item.Instructions.Lines.Count);
		}

		[Test]
		public void InstructionsOwnerForm()
		{
			// create a Project
			Project.NewTestProject();

			// add a Form and make it current
			IForm form = Project.Current.AddForm();

			// add a SkipItem to the Form
			SkipInstructionsItem item = new SkipInstructionsItem();
			form.ItemList.Add(item);

			Assert.AreEqual(form.Name, item.Instructions.Name);
		}

		[Test]
		public void GetXml()
		{
			SkipInstructionsItem item = new SkipInstructionsItem();

			// add a Set statement
			SetStatement statement1 = new SetStatement();
			statement1.Variable = new Variable("a variable");
			statement1.Expression = new Expression("a value");
			SetLine line1 = new SetLine(statement1);
			item.Instructions.Lines.Add(line1);

			//Assertion - SkipToItem XML should just be the Instructions' XML
			Assert.AreEqual(item.Instructions.ToXml(), item.ToXml());
		}

		[Test]
		public void AlternateLabel()
		{
			SkipInstructionsItem item = new SkipInstructionsItem();

			//Assertions 
			Assert.AreEqual(string.Empty, item.AlternateLabel);

			// assignment should have no effect
			item.AlternateLabel = "Foo";

			Assert.AreEqual(string.Empty, item.AlternateLabel);
		}

		/// <summary>
		/// Just verify we get a different summary when there are no skips, 1 skip and many skips.
		/// Otherwise we would be checking the exact summary format and the strings are in
		/// Strongly Typed Resources so we would have to go through hoops to get at them.
		/// </summary>
		/// <remarks>Punted on most off test -- too hard to build an IfStatement with code inside it.</remarks>
		[Test]
		public void GetSummary()
		{
			// create a Project
			Project.NewTestProject();

			// add a Form and make it current
			IForm form = Project.Current.AddForm();
			Project.Current.SetCurrentComponent(form);
			SkipInstructionsItem item = new SkipInstructionsItem();

			string emptySummary = item.GetSummary();

			Assert.IsNotNull(emptySummary);
			Assert.IsTrue(emptySummary.Length != 0);

			TextItem textItem = new TextItem();
			textItem.AlternateLabel = "Text Item Label";
			form.ItemList.Add(textItem);
			SkipToDestinationItem dest = new SkipToDestinationItem(textItem); // default end of form
			SkipToStatement skipTo = new SkipToStatement();
			skipTo.Destination = dest;
			item.Instructions.Lines.Add(new SkipToLine(skipTo));

			string newSummary = item.GetSummary();

			Assert.IsNotNull(newSummary);
			Assert.IsTrue(newSummary.Length != 0);
			Assert.IsTrue(emptySummary.CompareTo(newSummary) != 0);

			Console.WriteLine(newSummary);
			Assert.IsTrue(newSummary.Contains(textItem.AlternateLabel));
		}
	}
}
