// $Workfile: SkipInstructionsTest.cs $
// $Revision: 9 $	$Date: 8/29/06 11:11p $
// Copyright © 2005 Douglas G. Carlston. All rights reserved.using System;

using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Summary description for SkipInstructionsTest.
	/// </summary>
	[TestFixture]
	public class SkipInstructionsTest
	{
		[Test]
		public void AddStatement()
		{
			SkipInstructions instructions = new SkipInstructions();

			SkipToStatement statement = new SkipToStatement();
			instructions.Lines.Add(new ProcessLineList(statement));

			Assert.AreEqual(1, instructions.Lines.Count);
		}

		[Test]
		public void RemoveStatement()
		{
			SkipInstructions instructions = new SkipInstructions();

			// add a Set statement
			SetStatement statement1 = new SetStatement();
			statement1.Variable = new Variable("variable1");
			statement1.Expression = new Expression("value1");
			SetLine line1 = new SetLine(statement1);
			instructions.Lines.Add(line1);

			// add a second statement
			SetStatement statement2 = new SetStatement();
			statement2.Variable = new Variable("variable2");
			statement2.Expression = new Expression("value2");
			SetLine line2 = new SetLine(statement2);
			instructions.Lines.Add(line2);

			Assert.AreEqual(2, instructions.Lines.Count);
			Assert.AreEqual(statement1, instructions.Lines[0].Statement);
			Assert.AreEqual(statement2, instructions.Lines[1].Statement);

			// remove first statement
			instructions.Lines.Remove(line1);

			Assert.AreEqual(1, instructions.Lines.Count);
			Assert.AreEqual(statement2, instructions.Lines[0].Statement);
		}

		[Test]
		public void InsertStatement()
		{
			SkipInstructions instructions = new SkipInstructions();

			// add a Set statement
			SetStatement statement1 = new SetStatement();
			statement1.Variable = new Variable("variable1");
			statement1.Expression = new Expression("value1");
			SetLine line1 = new SetLine(statement1);
			instructions.Lines.Add(line1);

			// add a second statement
			SetStatement statement2 = new SetStatement();
			statement2.Variable = new Variable("variable2");
			statement2.Expression = new Expression("value2");
			SetLine line2 = new SetLine(statement2);
			instructions.Lines.Add(line2);

			Assert.AreEqual(2, instructions.Lines.Count);
			Assert.AreEqual(statement1, instructions.Lines[0].Statement);
			Assert.AreEqual(statement2, instructions.Lines[1].Statement);

			// insert third statement
			SetStatement statement3 = new SetStatement();
			statement3.Variable = new Variable("variable3");
			statement3.Expression = new Expression("value3");
			SetLine line3 = new SetLine(statement3);
			instructions.Lines.Insert(1, line3);

			Assert.AreEqual(3, instructions.Lines.Count);
			Assert.AreEqual(statement1, instructions.Lines[0].Statement);
			Assert.AreEqual(statement3, instructions.Lines[1].Statement);
			Assert.AreEqual(statement2, instructions.Lines[2].Statement);
		}

		[Test]
		public void GetXml()
		{
			Util.NewTestProject();
			IForm form = Project.Current.AddForm();

			SkipInstructions instructions = new SkipInstructions();

			// add a Set statement
			SetStatement statement1 = new SetStatement();
			statement1.Variable = new Variable("a variable");
			statement1.Expression = new Expression("a value");
			SetLine line1 = new SetLine(statement1);
			instructions.Lines.Add(line1);

			// add a form item to skip to
			form.ItemList.Add(new McqItem());
			SkipToDestinationItem label = new SkipToDestinationItem(((McqItem)form.ItemList[0]));

			// add a Skip statement
			SkipToStatement statement2 = new SkipToStatement(label);
			SkipToLine line2 = new SkipToLine(statement2);
			instructions.Lines.Add(line2);

			string expectedString = "<skipInstructions>\r\n" +
									"<set field=\"a variable\" arithmeticAsText=\"false\">\r\n" +
									"<string value=\"a value\"/>\r\n" +
									"</set>\r\n" +
									"<skip to=\"Q1\"/>\r\n" +
									"</skipInstructions>\r\n";

			//Assertions 
			Assert.AreEqual(expectedString, instructions.ToXml());
		}

		[Test]
		public void CanConstructSimpleSkipInstructionsFromIXmlElement()
		{
			Util.NewTestProject();
			IForm form = Project.Current.AddForm();
			
			string xmlString = 
				"<skipInstructions>" +
				"<comment>This is a comment.</comment>" +
				"</skipInstructions>";

			SkipInstructions skipInstructions = new SkipInstructions(new XmlElement(xmlString));

			string expectedXml =
				"<skipInstructions>\r\n" +
				"<comment>This is a comment.</comment>\r\n" +
				"</skipInstructions>\r\n";

			Assert.AreEqual(expectedXml, skipInstructions.ToXml());
		}

	}
}