using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Tawala.FormDesigner;
using System.Windows.Forms;
using TawalaTest.TestSupport;
using Tawala.Proj;
using Tawala.Proj.Forms.NewFormItems;
using Tawala.Proj.Forms.FormItemContents;
using Tawala.XmlSupport;

namespace TawalaTest.FormDesignerTest
{
	[TestFixture]
	public class McqItemUITest : UIOrientedTestBase
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
		public void AppendMcqItem()
		{
			Assert.IsFalse(body.InnerHtml.Contains("<TD"));

			formEditPresenter.InsertMcqItem(-1);

			VerifyItemTag("McqItem");

			Assert.IsNotNull(GetFormItem<IMcqItem>());
		}

		[Test]
		public void InsertMcqItem()
		{
			formEditPresenter.InsertBreakItem(-1);
			formEditPresenter.InsertBreakItem(-1);
			formEditPresenter.InsertMcqItem(1);

			Assert.AreEqual(3, Project.Current.FormList[0].ItemList.Count);
			Assert.IsInstanceOfType(typeof(IMcqItem), Project.Current.FormList[0].ItemList[1]);

			int breakItem1Index = body.InnerHtml.IndexOf("t:BreakItem");
			int mcqItemIndex = body.InnerHtml.IndexOf("t:McqItem");
			int breakItem2Index = body.InnerHtml.LastIndexOf("t:BreakItem");

			Assert.IsTrue(breakItem1Index > -1);
			Assert.IsTrue(mcqItemIndex > breakItem1Index);
			Assert.IsTrue(mcqItemIndex < breakItem2Index);
		}

		[Test]
		public void InsertingMcqItemAtIndexOfNegativeOneAppends()
		{
			formEditPresenter.InsertBreakItem(-1);
			formEditPresenter.InsertMcqItem(-1);

			int breakItem1Index = body.InnerHtml.IndexOf("t:BreakItem");
			int mcqItemIndex = body.InnerHtml.IndexOf("t:McqItem");

			Assert.IsTrue(breakItem1Index > -1);
			Assert.IsTrue(mcqItemIndex > breakItem1Index);
		}

		[Test]
		public void InsertedMcqItemsHaveCorrectDefaultLabels()
		{
			formEditPresenter.InsertMcqItem(-1);
			formEditPresenter.InsertMcqItem(0);

			int mcqItem1Label = body.InnerHtml.IndexOf("Q2</DIV>");
			int mcqItem2Label = body.InnerHtml.IndexOf("Q1</DIV>");

			Assert.IsTrue(mcqItem1Label > 0);
			Assert.IsTrue(mcqItem2Label > 0);
			Assert.IsTrue(mcqItem1Label > mcqItem2Label);
		}

		[Test]
		public void InsertedMcqItemContainsDefaultText()
		{
			formEditPresenter.InsertMcqItem(-1);

			Assert.IsTrue(body.InnerHtml.Contains("[Replace this with your question. Use Enter key to add choices below.]"));
		}

		[Test]
		public void McqItemGeneratesCorrectXml()
		{
			string mcqItemXml =
				"<mc label=\"Q1\" onlyone=\"true\" required=\"false\" style=\"vertical\">" +
				"<question>" + 
				"<paragraph indent=\"0\" align=\"left\">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">" +
				"[Replace this with your question. Use Enter key to add choices below.]" +
				"</font>" +
				"</paragraph>" +
				"</question>" +
				"<choice label=\"a\">" +
				"</choice>" +
				"</mc>";

			formEditPresenter.InsertMcqItem(-1);

			string projectXml = Project.Current.ToXml();
			string actualXml = Regex.Match(projectXml, @"<mc.*</mc>").Value;

			Assert.AreEqual(mcqItemXml, actualXml);
		}

		[Test]
		public void ModifyingQuestionTextUpdatesProjectMcqItem()
		{
			string mcqItemXml =
				"<mc label=\"Q1\" onlyone=\"true\" required=\"false\" style=\"vertical\">" +
				"<question><paragraph indent=\"0\" align=\"left\">" +
				"New question text:" +
				"</paragraph>" +
				"</question>" +
				"<choice label=\"a\">" +
				"</choice>" +
				"</mc>";

			formEditPresenter.InsertMcqItem(-1);
			IMcqItem mcqItem = GetFormItem<IMcqItem>();

			formView.SetAttribute(mcqItem.Id.ToString(), "Question", "<p style=\"margin-left: 0pt\">New question text:</p>");

			string projectXml = Project.Current.ToXml();
			string actualXml = Regex.Match(projectXml, @"<mc.*</mc>").Value;

			Assert.AreEqual(mcqItemXml, actualXml);
		}

		[Test]
		public void ModifyingQuestionChoicesUpdatesProjectMcqItem()
		{
			formEditPresenter.InsertMcqItem(-1);
			IMcqItem mcqItem = GetFormItem<IMcqItem>();
			mcqItem.ChoiceContents = new NewChoiceList(new string[] { "Red", "Green", "Blue" });

			formEditPresenter.UpdateMcqChoicesInView(mcqItem.Id.ToString());

			Assert.AreEqual(3, mcqItem.Choices.Count);
			Assert.AreEqual("Red", mcqItem.Choices[0].Text);
			Assert.AreEqual("Green", mcqItem.Choices[1].Text);
			Assert.AreEqual("Blue", mcqItem.Choices[2].Text);
		}

		[Test]
		public void SettingFieldInChoiceProducesFieldInChoiceHtml()
		{
			formEditPresenter.InsertFibItem(-1);
			formEditPresenter.InsertMcqItem(-1);
			IFibItem fibItem = GetFormItem<IFibItem>(0);
			IMcqItem mcqItem = GetFormItem<IMcqItem>(1);

			mcqItem.ChoiceContents = new NewChoiceList(new string[] { "<<Form 1:Q1:a>>" });

			formEditPresenter.UpdateMcqChoicesInView(mcqItem.Id.ToString());

			string xhtmlFormatString =
				"<span class=\"choice\">a</span>" +
				"<input type=\"radio\" />" +
				"<span><field fieldID=\"{0}\">Form 1:Q1:a</field></span><br />";

			string expectedXhtml = string.Format(xhtmlFormatString, fibItem.BlankList[0].Id);
			string actualXhtml = XmlUtility.ToXhtml(Regex.Match(body.InnerHtml, @"<t:Choice id=[^>]+>(.*)</t:Choice>").Groups[1].Value);

			Assert.AreEqual(expectedXhtml, actualXhtml);
		}

		[Test]
		public void RepositioningFormItemsUpdatesFieldInChoiceHtml()
		{
			formEditPresenter.InsertFibItem(-1);
			formEditPresenter.InsertMcqItem(-1);
			IFibItem fibItem = GetFormItem<IFibItem>(0);
			IMcqItem mcqItem = GetFormItem<IMcqItem>(1);

			mcqItem.ChoiceContents = new NewChoiceList(new string[] { "<<Form 1:Q1:a>>" });

			formEditPresenter.InsertFibItem(0);

			formEditPresenter.UpdateMcqChoicesInView(mcqItem.Id.ToString());

			string xhtmlFormatString =
				"<span class=\"choice\">a</span>" +
				"<input type=\"radio\" />" +
				"<span><field fieldID=\"{0}\">Form 1:Q2:a</field></span><br />";

			string expectedXhtml = string.Format(xhtmlFormatString, fibItem.BlankList[0].Id);
			string actualXhtml = XmlUtility.ToXhtml(Regex.Match(body.InnerHtml, @"<t:Choice id=[^>]+>(.*)</t:Choice>").Groups[1].Value);

			Assert.AreEqual(expectedXhtml, actualXhtml);
		}
	}

	[TestFixture]
	public class McqItemModelTest : ModelOrientedTestBase
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
		public void McqItemInProjectGeneratesMcqItemInView()
		{
			IMcqItem mcqItem = new NewMcqItem();
			form.ItemList.Add(mcqItem);

			CreateViewFromForm();

			string html = body.InnerHtml;

			VerifyItemTag("McqItem");
			Assert.IsTrue(html.Contains("[Replace this with your question. Use Enter key to add choices below.]"));
		}

		[Test]
		public void McqItemChoicesInProjectGeneratesMcqItemChoicesInView()
		{
			string mcqItemXml =
				"<mc label=\"Q1\" onlyone=\"true\" required=\"false\" style=\"vertical\">" +
				"<question><paragraph indent=\"0\" align=\"left\">" +
				"New question text:" +
				"</paragraph>" +
				"</question>" +
				"<choice label=\"a\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Choice one" +
				"</paragraph>" +
				"</choice>" +
				"<choice label=\"b\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"Choice two" +
				"</paragraph>" +
				"</choice>" +
				"</mc>";

			IMcqItem mcqItem = new NewMcqItem(new XmlElement(mcqItemXml));
			form.ItemList.Add(mcqItem);

			CreateViewFromForm();

			string html = body.InnerHtml;

			VerifyItemTag("McqItem");
			VerifyItemTag("Choice");
			Assert.IsTrue(html.Contains(mcqItem.Choices[0].Text));
			Assert.IsTrue(html.Contains(mcqItem.Choices[1].Text));
		}
	}
}
