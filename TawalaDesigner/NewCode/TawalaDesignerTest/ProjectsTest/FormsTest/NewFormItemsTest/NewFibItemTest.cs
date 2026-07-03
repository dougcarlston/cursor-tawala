using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Factories;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest.Forms.NewFormItems
{
	/// <summary>
	/// Test class for the NewFibItem class
	/// </summary>
	[TestFixture]
	public class NewFibItemTest
	{
		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
		}

		[Test]
		public void NewFibItemIsAnIFormItem()
		{
			NewFibItem fibItem = new NewFibItem();

			Assert.IsTrue(fibItem is IFormItem);
		}

		[Test]
		public void CanGetContentsXml()
		{
			NewFibItem fibItem = new NewFibItem();

			string expectedXml =
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with your question. Underscores create blanks.] " +
				"<blank label=\"a\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"</paragraph>";

            Assert.AreEqual(expectedXml, fibItem.Contents.ToXml());
		}

		[Test]
		public void CanGetContentsXhtml()
		{
			IForm form = Project.Current.AddForm();
			NewFibItem fibItem = new NewFibItem();
			form.ItemList.Add(fibItem);

			string xhtmlFormat =
				@"<p style=""margin-left: 0pt"" align=""left"">" +
				@"[Replace this with your question. Underscores create blanks.] " +
				@"<t:Blank id=""{0}"">" +
				@"<input class=""blank"" size=""20"" style=""height:21px;"" value=""Q1:a"" />" +
				@"</t:Blank></p>";

            string xhtmlString = fibItem.Contents.ToXhtml(fibItem);
			string id = Regex.Match(xhtmlString, @"<t:Blank id=""(\d+)"">").Groups[1].Value;

			Assert.AreEqual(string.Format(xhtmlFormat, id), xhtmlString);
		}

		[Test]
		public void CanSetContentsFromXhtml()
		{
			IForm form = Project.Current.AddForm();
			NewFibItem fibItem = new NewFibItem();
			form.ItemList.Add(fibItem);

			string htmlString =
				@"<div>" +
				@"<p style=""margin-left: 0pt"">" +
				@"Question" +
				@"<t:Blank id=""1001"">" +
				@"<input class=""blank"" size=""40"" style=""height:21px;"" value=""Q1:a"">" +
				@"</t:Blank>" +
				@"</p>" +
				@"</div>";

			string expectedXml =
				"<paragraph indent=\"0\" align=\"left\">" +
				"Question" +
				"<blank label=\"a\" length=\"40\" height=\"1\" required=\"false\"/>" +
				"</paragraph>";

            fibItem.Contents = FormItemContentsFactory.MakeChildrenFromHtml(htmlString);

            Assert.AreEqual(expectedXml, fibItem.Contents.ToXml());
		}


		[Test]
		public void CanSetContentsFromMultiParagraphXhtml()
		{
			IForm form = Project.Current.AddForm();
			NewFibItem fibItem = new NewFibItem();
			form.ItemList.Add(fibItem);

			string htmlString =
				@"<div>" +
				@"<p style=""margin-left: 0pt"">" +
				@"Paragraph 1" +
				@"<t:Blank id=""1001"">" +
				@"<input class=""blank"" size=""40"" style=""height:21px;"" value=""Q1:a"">" +
				@"</t:Blank>" + 
				@"</p>" +
				@"<p style=""margin-left: 0pt"">" +
				@"Paragraph 2" +
				@"<t:Blank id=""1002"">" +
				@"<input class=""blank"" size=""30"" style=""height:21px;"" value=""Q1:b"">" +
				@"</t:Blank>" +
				@"</p>" +
				@"</div>";

            fibItem.Contents = FormItemContentsFactory.MakeChildrenFromHtml(htmlString);

			string expectedXml =
				"<paragraph indent=\"0\" align=\"left\">" +
				"Paragraph 1" +
				"<blank label=\"a\" length=\"40\" height=\"1\" required=\"false\"/>" +
				"</paragraph>" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Paragraph 2" +
				"<blank label=\"b\" length=\"30\" height=\"1\" required=\"false\"/>" +
				"</paragraph>";

            Assert.AreEqual(expectedXml, fibItem.Contents.ToXml());
		}

		[Test]
		public void CanGetDefaultXml()
		{
			NewFibItem fibItem = new NewFibItem();

			string expectedXml =
				"<fib label=\"Q1\" style=\"topLabels\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with your question. Underscores create blanks.] " +
				"<blank label=\"a\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"</paragraph>" +
				"</fib>" + Environment.NewLine; 

			Assert.AreEqual(expectedXml, fibItem.ToXml("Q1"));
		}

		[Test]
		public void FormItemListContainsFibItem()
		{
			NewFibItem fibItem = new NewFibItem();
			IForm form = Project.Current.AddForm();
			form.ItemList.Add(fibItem);

			Assert.IsTrue(Project.Current.FormList[0].ItemList.Contains(fibItem));
		}

		[Test]
		public void FormItemListYieldsFibItemDefaultLabel()
		{
			NewFibItem fibItem = new NewFibItem();
			IForm form = Project.Current.AddForm();
			form.ItemList.Add(fibItem);

			Assert.AreEqual("Q1", Project.Current.FormList[0].ItemList.GetDefaultLabel(fibItem));
		}

		[Test]
		public void CanGetBlanksFromFibItem()
		{
			NewFibItem fibItem = new NewFibItem();
			IForm form = Project.Current.AddForm();
			form.ItemList.Add(fibItem);

			string htmlString =
				@"<div>" +
				@"<p style=""margin-left: 0pt"">" +
				@"Paragraph 1" +
				@"<t:Blank id=""1001"">" +
				@"<input class=""blank"" size=""40"" style=""height:21px;"" value=""Q1:a"">" +
				@"</t:Blank>" +
				@"</p>" +
				@"<p style=""margin-left: 0pt"">" +
				@"Paragraph 2" +
				@"<t:Blank id=""1002"">" +
				@"<input class=""blank"" size=""30"" style=""height:21px;"" value=""Q1:b"">" +
				@"</t:Blank>" +
				@"</p>" +
				@"</div>";

            fibItem.Contents = FormItemContentsFactory.MakeChildrenFromHtml(htmlString);

			Assert.AreEqual("Q1:a", fibItem.BlankList[0].FieldName);
			Assert.AreEqual("Q1:b", fibItem.BlankList[1].FieldName);
		}

		[Test]
		public void BlankAlternateLabelOverridesFibAlternateLabel()
		{
			IFibItem fibItem = new NewFibItem();
			IForm form = Project.Current.AddForm();
			fibItem.AlternateLabel = "My FIB";
			fibItem.BlankList[0].AlternateLabel = "First Blank";
			form.ItemList.Add(fibItem as IFormItem);

			Assert.AreEqual("First Blank", fibItem.BlankList[0].AlternateLabel);
		}

		[Test]
		public void BlankWithAlternateLabelGeneratesExpectedFibXml()
		{
			NewFibItem fibItem = new NewFibItem();
			fibItem.BlankList[0].AlternateLabel = "BlankAlternateLabel";

			string expectedXml =
				"<fib label=\"Q1\" style=\"topLabels\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with your question. Underscores create blanks.] " +
				"<blank label=\"a\" length=\"20\" height=\"1\" required=\"false\" alternateLabel=\"BlankAlternateLabel\"/>" +
				"</paragraph>" +
				"</fib>" + Environment.NewLine; 

			Assert.AreEqual(expectedXml, fibItem.ToXml("Q1"));			
		}

		[Test]
		public void BlankWithAlternateLabelRegeneratesCorrectlyFromXml()
		{
			string xmlString =
				"<fib label=\"Q1\" style=\"topLabels\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with your question. Underscores create blanks.] " +
				"<blank label=\"a\" length=\"20\" height=\"1\" required=\"false\" alternateLabel=\"BlankAlternateLabel\"/>" +
				"</paragraph>" +
				"</fib>" + Environment.NewLine;

			IFibItem fibItem = new NewFibItem(new XmlElement(xmlString));

			Assert.AreEqual("BlankAlternateLabel", fibItem.BlankList[0].AlternateLabel);
		}

		[Test]
		public void BlankWithResponseRequiredGeneratesExpectedFibXml()
		{
			NewFibItem fibItem = new NewFibItem();
			fibItem.BlankList[0].Required = true;

			string expectedXml =
				"<fib label=\"Q1\" style=\"topLabels\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with your question. Underscores create blanks.] " +
				"<blank label=\"a\" length=\"20\" height=\"1\" required=\"true\"/>" +
				"</paragraph>" +
				"</fib>" + Environment.NewLine;

			Assert.AreEqual(expectedXml, fibItem.ToXml("Q1"));
		}

		[Test]
		public void BlankWithResponseRequiredRegeneratesCorrectlyFromXml()
		{
			string xmlString =
				"<fib label=\"Q1\" style=\"topLabels\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with your question. Underscores create blanks.] " +
				"<blank label=\"a\" length=\"20\" height=\"1\" required=\"true\"/>" +
				"</paragraph>" +
				"</fib>" + Environment.NewLine;

			IFibItem fibItem = new NewFibItem(new XmlElement(xmlString));

			Assert.IsTrue(fibItem.BlankList[0].Required);
		}

		[Test]
		public void BlankImplementsIOperatorDataSource()
		{
			string xmlString =
				"<fib label=\"Q1\" style=\"topLabels\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with your question. Underscores create blanks.] " +
				"<blank label=\"a\" length=\"20\" height=\"1\" required=\"true\"/>" +
				"</paragraph>" +
				"</fib>" + Environment.NewLine;

			IFibItem fibItem = new NewFibItem(new XmlElement(xmlString));
			IBlank blank = fibItem.BlankList[0];

			Assert.IsTrue(blank is IOperatorDataSource);
		}


		[Test]
		public void FibWithAlternateLabelGeneratesExpectedFibXml()
		{
			NewFibItem fibItem = new NewFibItem();
			fibItem.AlternateLabel = "FibAlternateLabel";

			string expectedXml =
				"<fib label=\"Q1\" alternateLabel=\"FibAlternateLabel\" style=\"topLabels\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with your question. Underscores create blanks.] " +
				"<blank label=\"a\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"</paragraph>" +
				"</fib>" + Environment.NewLine;

			Assert.AreEqual(expectedXml, fibItem.ToXml("Q1"));
		}

		[Test]
		public void FibWithAlternateLabelRegeneratesCorrectlyFromXml()
		{
			string xmlString =
				"<fib label=\"Q1\" alternateLabel=\"FibAlternateLabel\" style=\"topLabels\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with your question. Underscores create blanks.] " +
				"<blank label=\"a\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"</paragraph>" +
				"</fib>" + Environment.NewLine;

			IFibItem fibItem = new NewFibItem(new XmlElement(xmlString));

			Assert.AreEqual("FibAlternateLabel", fibItem.AlternateLabel);
		}
	}
}
