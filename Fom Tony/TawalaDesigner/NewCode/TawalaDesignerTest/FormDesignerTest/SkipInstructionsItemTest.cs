using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Tawala.FormDesigner;
using Tawala.Projects;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Properties;
using Tawala.XmlSupport;

namespace TawalaTest.FormDesignerTest
{
	[TestFixture]
	public class SkipInstructionsItemUITest : UIOrientedTestBase
	{
		[SetUp]
		public void SetUp()
		{
			UITestSetUp();
		}

		[TearDown]
		public void TearDown()
		{
			UITestTearDown();
		}

		[Test]
		public void AppendSkipInstructionsItem()
		{
			formEditPresenter.InsertSkipItem(FormPresenter.InsertAtEnd);

			VerifyItemTag("SkipItem");
			Assert.IsNotNull(GetFormItem<ISkipInstructionsItem>());
		}

		[Test]
		public void InsertSkipInstructionsItem()
		{
			formEditPresenter.InsertBreakItem(FormPresenter.InsertAtEnd);
			formEditPresenter.InsertBreakItem(FormPresenter.InsertAtEnd);
			formEditPresenter.InsertSkipItem(1);
			
			Assert.AreEqual(3, Project.Current.FormList[0].ItemList.Count);
			Assert.IsInstanceOfType(typeof(ISkipInstructionsItem), Project.Current.FormList[0].ItemList[1]);

			int breakItem1Index = body.InnerHtml.IndexOf("t:BreakItem");
			int skipItemIndex = body.InnerHtml.IndexOf("t:SkipItem");
			int breakItem2Index = body.InnerHtml.LastIndexOf("t:BreakItem");

			Assert.IsTrue(breakItem1Index > -1);
			Assert.IsTrue(skipItemIndex > breakItem1Index);
			Assert.IsTrue(skipItemIndex < breakItem2Index);
		}

		[Test]
		public void InsertedSkipInstructionsItemContainsDefaultText()
		{
			formEditPresenter.InsertSkipItem(FormPresenter.InsertAtEnd);

			Assert.IsTrue(body.InnerHtml.Replace("&nbsp;", " ").Contains(Resources.SkipSummaryEmpty));
		}

		[Test]
		public void SkipInstructionsItemGeneratesCorrectXml()
		{
			formEditPresenter.InsertSkipItem(FormPresenter.InsertAtEnd);

			string expectedXml =
				"<skipInstructions>" + Environment.NewLine +
				"</skipInstructions>" + Environment.NewLine;

			string actualSkipItemXml = Regex.Match(Project.Current.ToXml(), "<skipInstructions>\r\n</skipInstructions>\r\n").Value;

			Assert.AreEqual(expectedXml, actualSkipItemXml);
		}

		[Test]
		public void CanSetAndGetSkipInstructionsItemSummary()
		{
			formEditPresenter.InsertSkipItem(FormPresenter.InsertAtEnd);
			ISkipInstructionsItem skipItem = GetFormItem<ISkipInstructionsItem>();

			formView.SetAttribute(skipItem.Id.ToString(), "Summary", "Summary Text");

			Assert.AreEqual("Summary Text", formView.GetAttribute(skipItem.Id.ToString(), "Summary"));
		}
	}

	[TestFixture]
	public class SkipInstructionsItemModelTest : ModelOrientedTestBase
	{
		[SetUp]
		public void SetUp()
		{
			ModelTestSetUp();
		}

		[TearDown]
		public void TearDown()
		{
			ModelTestTearDown();
		}

		[Test]
		public void SkipItemInProjectGeneratesSkipItemInView()
		{
			ISkipInstructionsItem skipItem = new NewSkipInstructionsItem();
			form.ItemList.Add(skipItem);

			CreateViewFromForm();

			string html = XmlUtility.ToXhtml(body.InnerHtml);

			VerifyItemTag("SkipItem");
		}

		[Test]
		public void CanMakeSkipInstructionsItemWithSkipToFibItemFromXml()
		{
			string xmlString =
				"<skipInstructions>" + Environment.NewLine +
				"<skip to=\"Q1\"/>" + Environment.NewLine +
				"</skipInstructions>" + Environment.NewLine;

			ISkipInstructionsItem skipInstructionsItem = new NewSkipInstructionsItem(new XmlElement(xmlString));
			form.ItemList.Add(skipInstructionsItem);
			form.ItemList.Add(new NewFibItem());

			form.ItemList.ResolveFieldReferences();

			Assert.AreEqual(xmlString, skipInstructionsItem.ToXml());
		}

	}
}
