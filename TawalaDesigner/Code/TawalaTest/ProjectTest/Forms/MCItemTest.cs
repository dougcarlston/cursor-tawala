using System;
using System.Collections;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using Tawala.RtfSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the MCChoiceItem class
	/// </summary>
	[TestFixture]
	public class MCItemTest
	{
		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();
		}

		[Test]
		public void NewMCItem() 
		{ 
			McqItem item = new McqItem();

			//Assertions 
			Assert.IsNotNull(item);
			Assert.AreEqual(Tawala.Projects.Properties.Resources.MCItemDefaultText, item.Text);
			Assert.AreEqual(true, item.SelectOnlyOne);
			Assert.AreEqual(false, item.RequireAtLeastOne);
			Assert.AreEqual(false, item.IsTextItem);
			Assert.AreEqual(true, item.IsQuestionItem);
		}

		[Test]
		public void ConstructFromXml()
		{
			string xmlString =
				"<mc label=\"Q1\" onlyone=\"true\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + "><question>Make a choice:</question><choice label=\"a\">Choice One</choice><choice label=\"b\">Choice Two</choice></mc>";

			IXmlElement element = new XmlElement(xmlString);
			McqItem item1 = new McqItem(element);

			Assert.AreEqual("Make a choice:", item1.Text);
			Assert.AreEqual("", item1.AlternateLabel);
			Assert.IsTrue(item1.SelectOnlyOne, "SelectOnlyOne should be true");
			Assert.IsFalse(item1.RequireAtLeastOne, "RequireAtLeastOne should be false");
			Assert.AreEqual(2, item1.Choices.Count);
			Assert.AreEqual("Choice One", ((Choice)item1.Choices[0]).Text);
			Assert.AreEqual("Choice Two", ((Choice)item1.Choices[1]).Text);
			
			xmlString =
				"<mc label=\"Q1\" alternateLabel=\"MCQ 1\" onlyone=\"false\" required=\"true\"" + XmlConstants.DefaultMcqItemStyleAttribute + "><question>Make choices:</question><choice label=\"a\">Choice One</choice><choice label=\"b\">Choice Two</choice></mc>";
			element = new XmlElement(xmlString);
			McqItem item2 = new McqItem(element);

			Assert.AreEqual("Make choices:", item2.Text);
			Assert.AreEqual("MCQ 1", item2.AlternateLabel);
			Assert.IsFalse(item2.SelectOnlyOne, "SelectOnlyOne should be true");
			Assert.IsTrue(item2.RequireAtLeastOne, "RequireAtLeastOne should be false");
		}
		
		[Test]
		public void ConstructFromXmlNoQuestionText()
		{
			string xmlString =
				"<mc label=\"Q1\" onlyone=\"true\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + "><question></question><choice label=\"a\">Choice One</choice></mc>";

			IXmlElement element = new XmlElement(xmlString);
			McqItem item = new McqItem(element);

			Assert.IsNotNull(item.Text, "MCQ question text should not be null.");
			Assert.AreEqual("", item.Text);
			Assert.AreEqual(1, item.Choices.Count);
			Assert.AreEqual("Choice One", ((Choice)item.Choices[0]).Text);
		}

		[Test]
		public void ConstructFromXmlNoChoiceText()
		{
			string xmlString =
				"<mc label=\"Q1\" onlyone=\"true\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + "><question>Make a choice:</question><choice label=\"a\"></choice></mc>";

			IXmlElement element = new XmlElement(xmlString);
			McqItem item = new McqItem(element);

			Assert.AreEqual("Make a choice:", item.Text);
			Assert.AreEqual(1, item.Choices.Count);
			Assert.IsNotNull(item.Text, "MCQ choice text should not be null.");
			Assert.AreEqual("", ((Choice)item.Choices[0]).Text);
		}

		[Test]
		public void ChoiceList() 
		{ 
			McqItem item = new McqItem();

			IChoice choice1 = (Choice)(item.Choices[0]);
			choice1.Text = "Choice Number 1";
			IChoice choice2 = new Choice();
			choice2.Text = "Choice Number 2";

			item.Choices.Add(choice2);

			//Assertion
			Assert.AreEqual(2, item.Choices.Count);

			IChoice retrievedChoice = (Choice)(item.Choices[1]);

			//Assertion
			Assert.AreEqual("Choice Number 2", retrievedChoice.Text);
		}

		[Test]
		public void GetDefaultXml() 
		{ 
			McqItem item = new McqItem();

			string expString =
				"<mc label=\"Q1\" onlyone=\"true\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
				"<question>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.DefaultTabsXml +
				XmlConstants.FullBeginFont +
				Tawala.Projects.Properties.Resources.MCItemDefaultText +
				XmlConstants.EndFont +
				"</paragraph>" +
				"</question>" +
				"<choice label=\"a\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.DefaultTabsXml +
				"</paragraph>" +
				"</choice>" +
				"</mc>\r\n";

			Assert.AreEqual(expString, item.ToXml("Q1"));
		} 

		[Test]
		public void GetXmlWithLabel() 
		{ 
			McqItem item = new McqItem();
			item.SelectOnlyOne = true;
			item.RequireAtLeastOne = true;

			IChoice choice1 = (Choice)(item.Choices[0]);
			choice1.Text = "Choice Number 1";
			IChoice choice2 = new Choice();
			choice2.Text = "Choice Number 2";
			IChoice choice3 = new Choice();
			choice3.Text = "Choice Number 3";

			item.Choices.Add(choice2);
			item.Choices.Add(choice3);

			string expString =
				"<mc label=\"Q1\" onlyone=\"true\" required=\"true\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
				"<question>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.DefaultTabsXml +
				XmlConstants.FullBeginFont +
				Tawala.Projects.Properties.Resources.MCItemDefaultText +
				XmlConstants.EndFont +
				"</paragraph>" +
				"</question>" +
				"<choice label=\"a\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont +
				"Choice Number 1" +
				XmlConstants.EndFont +
				"</paragraph>" +
				"</choice>" +
				"<choice label=\"b\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont +
				"Choice Number 2" +
				XmlConstants.EndFont +
				"</paragraph>" +
				"</choice>" +
				"<choice label=\"c\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont +
				"Choice Number 3" +
				XmlConstants.EndFont +
				"</paragraph>" +
				"</choice>" +
				"</mc>\r\n";

			Assert.AreEqual(expString, item.ToXml("Q1"));
		}

		[Test]
		public void GetXmlWithAlternateLabel()
		{
			McqItem item = new McqItem();
			item.AlternateLabel = "MyMCQ";
			item.SelectOnlyOne = true;
			item.RequireAtLeastOne = true;

			IChoice choice1 = (Choice)(item.Choices[0]);
			choice1.Text = "Choice Number 1";
			IChoice choice2 = new Choice();
			choice2.Text = "Choice Number 2";
			IChoice choice3 = new Choice();
			choice3.Text = "Choice Number 3";

			item.Choices.Add(choice2);
			item.Choices.Add(choice3);

			string expString =
				"<mc label=\"Q1\" alternateLabel=\"MyMCQ\" onlyone=\"true\" required=\"true\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
				"<question>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.DefaultTabsXml +
				XmlConstants.FullBeginFont +
				Tawala.Projects.Properties.Resources.MCItemDefaultText +
				XmlConstants.EndFont +
				"</paragraph>" +
				"</question>" +
				"<choice label=\"a\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont +
				"Choice Number 1" +
				XmlConstants.EndFont +
				"</paragraph>" +
				"</choice>" +
				"<choice label=\"b\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont +
				"Choice Number 2" +
				XmlConstants.EndFont +
				"</paragraph>" +
				"</choice>" +
				"<choice label=\"c\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont +
				"Choice Number 3" +
				XmlConstants.EndFont +
				"</paragraph>" +
				"</choice>" +
				"</mc>\r\n";


			//Assertions 
			Assert.AreEqual(expString, item.ToXml("Q1"));
		}

		[Test]
		public void FieldName()
		{
			Project.NewTestProject();
			IForm form = Project.Current.AddForm();

			McqItem mcItem1 = new McqItem();
			form.ItemList.Add(mcItem1);

			Assert.AreEqual("Q1", mcItem1.FieldName);
		}

		[Test]
		public void FieldString()
		{
			Project.NewTestProject();
			IForm form = Project.Current.AddForm();

			McqItem mcItem1 = new McqItem();
			form.ItemList.Add(mcItem1);

			Assert.AreEqual("<<Form 1:Q1>>", mcItem1.FieldString);
		}

		[Test]
		public void OperatorDataSource()
		{
			McqItem mcItem = new McqItem();
			Assert.AreEqual(MCOneOperator.List.DataSource, mcItem.OperatorDataSource);

			mcItem.SelectOnlyOne = false;
			Assert.AreEqual(MCManyOperator.List.DataSource, mcItem.OperatorDataSource);
		}

		[Test]
		public void MaximumChoiceIndex()
		{
			McqItem mcItem = new McqItem();

			Assert.AreEqual(0, mcItem.MaximumChoiceIndex);
		}

		private static void verifyObjectsFieldInQuestionText(McqItem mcItem)
		{
			Assert.AreEqual("MCQ with a Field <<Form 1:Q1:a>>", mcItem.Text);

			Assert.AreEqual(1, mcItem.Contents.Count);
			Assert.IsInstanceOfType(typeof(MCItemQuestion), mcItem.Contents[0]);

			MCItemQuestion question = (MCItemQuestion)mcItem.Contents[0];
			Assert.AreEqual(1, question.Contents.Count);

			Assert.IsInstanceOfType(typeof(Paragraph), question.Contents[0]);

			Paragraph paragraph = (Paragraph)question.Contents[0];
			Assert.AreEqual(2, paragraph.Contents.Count);

			Assert.IsInstanceOfType(typeof(FormItemFontAttributes), paragraph.Contents[0]);
			FormItemFontAttributes fontAttributes = (FormItemFontAttributes)paragraph.Contents[0];
			Assert.AreEqual("MCQ with a Field ", fontAttributes.Text);

			Assert.IsInstanceOfType(typeof(FormItemFontAttributes), paragraph.Contents[1]);
			fontAttributes = (FormItemFontAttributes)paragraph.Contents[1];
			Assert.AreEqual("<<Form 1:Q1:a>>", fontAttributes.Text);
		}

		[Test]
		public void QuestionXmlWithFieldProducesMCItemWithField()
		{
			IForm form = Project.Current.AddForm();
			FibItem referenceField = new FibItem();
			form.ItemList.Add(referenceField);

			string xmlQuestionWithFieldString =
				"<mc label=\"Q2\" onlyone=\"true\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
				"<question>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"<font face=\"Arial\" size=\"200\" color=\"000000\">" +
				"MCQ with a Field " +
				"</font>" +
				"<font face=\"Arial\" size=\"200\" color=\"000000\">" +
				"<field name=\"Form 1:Q1:a\"/>" +
				"</font>" +
				"</paragraph>" +
				"</question>" +
				"</mc>\r\n";

			IXmlElement element = new XmlElement(xmlQuestionWithFieldString);
			McqItem mcItem = new McqItem(element);

			verifyObjectsFieldInQuestionText(mcItem);
		}

		[Test]
		public void QuestionRtfWithFieldProducesMCItemWithField()
		{
			IForm form = Project.Current.AddForm();
			FibItem referenceField = new FibItem();
			form.ItemList.Add(referenceField);

			string rtfString =
				RtfDocument.RtfStringPrefix +
				@"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20 " +
				@"MCQ with a Field " +
				@"\plain\f0\fs20{\*\txfieldstart\txfieldtype0\txfieldflags216" +
				string.Format(@"\txfielddataval{0}", referenceField.BlankList[0].Id) +
				@"\txfielddata " + RtfUtility.EncodeHexString("TF$Form 1:Q1:a") + "}" +
				@"<<Form 1:Q1:a>>{" +
				@"\*\txfieldend}\par }";

			McqItem mcItem = new McqItem();
			mcItem.Rtf = rtfString;

			verifyObjectsFieldInQuestionText(mcItem);
		}
	}
}
