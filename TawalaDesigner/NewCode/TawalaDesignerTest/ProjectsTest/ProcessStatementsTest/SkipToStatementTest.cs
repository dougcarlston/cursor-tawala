// $Workfile: SkipToStatementTest.cs $
// $Revision: 8 $	$Date: 2/11/06 8:50a $
// Copyright © 2005 Douglas G. Carlston. All rights reserved.

using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest
{
    [Ignore("REQUIRES Fix-ups for new classes")]
    [TestFixture]
	public class SkipToStatementTest
	{
#if FIXED
        private IForm form;
		private FibItem fibItem;
		private SkipToDestinationItem fibDestItem;

		[SetUp]
		public void SetUp()
		{
            Util.NewTestProject();

			// create form
			form = Project.Current.AddForm();

			form.ItemList.Add(new FibItem());
			fibItem = ((FibItem)form.ItemList[0]);
			fibDestItem = new SkipToDestinationItem(fibItem);
		}

		[Test]
		public void Instantiate()
		{
			SkipToStatement statement = new SkipToStatement();

			Assert.IsNotNull(statement);
		}

		[Test]
		public void ConstructSkipToQuestionFromXml()
		{
			SkipInstructionsItem skipInstructionsItem = new SkipInstructionsItem();
			form.ItemList.Add(skipInstructionsItem);

			skipInstructionsItem.Instructions.MappedFields.Fields.Add(form.SkipToDestinations);
			skipInstructionsItem.Instructions.MappedFields.Map();

			string xmlString =
				"<skip to=\"Q1\"/>\r\n";

			IXmlElement element = new XmlElement(xmlString);
			SkipToStatement statement = new SkipToStatement(element, skipInstructionsItem.Instructions);

			Assert.AreEqual("Skip to Q1", statement.ToString());

			// assert that question and statement value reference same field
			Assert.AreEqual(((FibItem)form.ItemList[0]).Id, statement.Destination.ItemId);
		}

		[Test]
		public void ConstructSkipToTextFromXml()
		{
			SkipInstructionsItem skipInstructionsItem = new SkipInstructionsItem();
			form.ItemList.Add(skipInstructionsItem);

			TextItem textItem = new TextItem();
			form.ItemList.Add(textItem);

			skipInstructionsItem.Instructions.MappedFields.Fields.Add(form.SkipToDestinations);
			skipInstructionsItem.Instructions.MappedFields.Map();

			string xmlString =
				"<skip to=\"T1\"/>\r\n";

			IXmlElement element = new XmlElement(xmlString);
			SkipToStatement statement = new SkipToStatement(element, skipInstructionsItem.Instructions);

			Assert.AreEqual("Skip to T1", statement.ToString());

			// assert that question and statement value reference same field
			Assert.AreEqual(((TextItem)form.ItemList[2]).Id, statement.Destination.ItemId);
		}

		[Test]
		public void Name()
		{
			SkipToStatement statement = new SkipToStatement();
			Assert.AreEqual("Skip", statement.Name);
		}

		[Test]
		public void DestinationItem()
		{
			SkipToStatement statement = new SkipToStatement(fibDestItem);
			Assert.AreEqual(fibDestItem, statement.Destination);
		}

		[Test]
		public void GetText()
		{
			SkipToStatement statement = new SkipToStatement(fibDestItem);
			Assert.AreEqual("Skip to Q1", statement.ToString());

			SkipToDestinationItem endOfFormItem = new SkipToDestinationItem();
			statement = new SkipToStatement(endOfFormItem);
			Assert.AreEqual("Skip to End of Form", statement.ToString());
		}

		[Test]
		public void GetXml()
		{
			SkipToStatement statement = new SkipToStatement(fibDestItem);
			Assert.AreEqual("<skip to=\"Q1\"/>", statement.ToXml());

			SkipToDestinationItem endOfFormItem = new SkipToDestinationItem();
			statement = new SkipToStatement(endOfFormItem);
			Assert.AreEqual("<skip to=\"__EndOfForm__\"/>", statement.ToXml());

			// check for invalid XML characters
			form.ItemList.Add(new TextItem());
			TextItem textItem = ((TextItem)form.ItemList[1]);
			textItem.AlternateLabel = "&<\"bad\" statement's label>";
			SkipToDestinationItem textDestItem = new SkipToDestinationItem(textItem);
			statement = new SkipToStatement(textDestItem);
			Assert.AreEqual("<skip to=\"&amp;&lt;&quot;bad&quot; statement&apos;s label&gt;\"/>", statement.ToXml());
		}
#endif
	}
}
