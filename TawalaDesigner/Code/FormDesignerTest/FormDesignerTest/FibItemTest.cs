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

namespace TawalaTest.FormDesignerTest
{
	[TestFixture]
	public class FibItemUITest : UIOrientedTestBase
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
		public void AppendFibItem()
		{
			Assert.IsFalse(body.InnerHtml.Contains("<TD"));

			formEditPresenter.InsertFibItem(-1);

			VerifyItemTag("FibItem");
			Assert.IsNotNull(GetFormItem<NewFibItem>());
		}

		[Test]
		public void InsertFibItem()
		{
			formEditPresenter.InsertBreakItem(-1);
			formEditPresenter.InsertBreakItem(-1);
			formEditPresenter.InsertFibItem(1);

			Assert.AreEqual(3, Project.Current.FormList[0].ItemList.Count);
			Assert.IsInstanceOfType(typeof(NewFibItem), Project.Current.FormList[0].ItemList[1]);

			int breakItem1Index = body.InnerHtml.IndexOf("t:BreakItem");
			int fibItemIndex = body.InnerHtml.IndexOf("t:FibItem");
			int breakItem2Index = body.InnerHtml.LastIndexOf("t:BreakItem");

			Assert.IsTrue(breakItem1Index > -1);
			Assert.IsTrue(fibItemIndex > breakItem1Index);
			Assert.IsTrue(fibItemIndex < breakItem2Index);
		}

		[Test]
		public void InsertingFibItemAtIndexOfNegativeOneAppends()
		{
			formEditPresenter.InsertBreakItem(-1);
			formEditPresenter.InsertFibItem(-1);

			int breakItem1Index = body.InnerHtml.IndexOf("t:BreakItem");
			int fibItemIndex = body.InnerHtml.IndexOf("t:FibItem");

			Assert.IsTrue(breakItem1Index > -1);
			Assert.IsTrue(fibItemIndex > breakItem1Index);
		}

		[Test]
		public void InsertedFibItemsHaveCorrectDefaultLabels()
		{
			formEditPresenter.InsertFibItem(-1);
			formEditPresenter.InsertFibItem(0);

			int fibItem1Label = body.InnerHtml.IndexOf("Q2</DIV>");
			int fibItem2Label = body.InnerHtml.IndexOf("Q1</DIV>");

			Assert.IsTrue(fibItem1Label > 0);
			Assert.IsTrue(fibItem2Label > 0);
			Assert.IsTrue(fibItem1Label > fibItem2Label);
		}

		[Test]
		public void InsertedFibItemContainsDefaultText()
		{
			formEditPresenter.InsertFibItem(-1);

		    Assert.IsTrue(body.InnerHtml.Contains("[Replace this with your question. Underscores create blanks.]"));
		}

		[Test]
		public void FibItemGeneratesCorrectXml()
		{
			string fibItemXml =
				"<fib label=\"Q1\" style=\"topLabels\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"[Replace this with your question. Underscores create blanks.] " +
				"<blank label=\"a\" length=\"20\" height=\"1\" required=\"false\"/>" +
				"</paragraph>" +
				"</fib>";

			formEditPresenter.InsertFibItem(-1);

			string projectXml = Project.Current.ToXml();

			Assert.IsTrue(projectXml.Contains(fibItemXml));
		}

		[Test]
		public void NewFibItemBlankIdIsOneGreaterThanNewFibItemId()
		{
			IFibItem fibItem = new NewFibItem();

			Assert.AreEqual(fibItem.Id + 1, fibItem.BlankList[0].Id);
		}

		[Test]
		public void InsertingFibItemConsumesTwoConsecutiveUniqueIds()
		{
			int beforeId = Reflect<Project>.GetField<int>("nextUniqueID", Project.Current);
			formEditPresenter.InsertFibItem(-1);
			int afterId = Reflect<Project>.GetField<int>("nextUniqueID", Project.Current);

			Assert.AreEqual(beforeId + 2, afterId);
		}

		[Test]
		public void ChangingBlankAlternateLabelUpdatesLabelInView()
		{
			formEditPresenter.InsertFibItem(-1);
			NewFibItem fibItem = GetFormItem<NewFibItem>();
			fibItem.BlankList[0].AlternateLabel = "BlankAlternateLabel";

			HtmlElement blank = document.GetElementById(fibItem.BlankList[0].Id.ToString());

			string actualAlternateLabel = Regex.Match(body.InnerHtml, @"<INPUT [^>]+value=([^ >]+)").Groups[1].Value;

			Assert.AreEqual("BlankAlternateLabel", actualAlternateLabel);
		}

	}

	[TestFixture]
	public class FibItemModelTest : ModelOrientedTestBase
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
		public void FibItemInModelGeneratesFibItemInView()
		{
			NewFibItem fibItem = new NewFibItem();
			form.ItemList.Add(fibItem);

			CreateViewFromForm();

			VerifyItemTag("FibItem");
		}

		[Test]
		public void BlankLengthInModelCreatesCorrectInputSize()
		{
			NewFibItem fibItem = new NewFibItem();
			fibItem.BlankList[0].Length = 13;
			form.ItemList.Add(fibItem);

			CreateViewFromForm();

			string expectedHtml = "<INPUT class=blank contentEditable=false style=\"HEIGHT: 21px\" size=13";
			string actualHtml = Regex.Match(body.InnerHtml, @"<INPUT.+ size=\d+").Value;

			Assert.AreEqual(expectedHtml, actualHtml);
		}

		[Test]
		public void DefaultBlankLabelsAreDisplayedInInputBoxes()
		{
			form.ItemList.Add(new NewFibItem());
			form.ItemList.Add(new NewFibItem());

			CreateViewFromForm();

			string html = body.InnerHtml;

			MatchCollection matches = Regex.Matches(html, @"<INPUT [^>]+>", RegexOptions.Singleline);

			string expectedHtml = "<INPUT class=blank contentEditable=false style=\"HEIGHT: 21px\" value=Q1:a>";

			Assert.AreEqual(expectedHtml, matches[0].Value);

			expectedHtml = "<INPUT class=blank contentEditable=false style=\"HEIGHT: 21px\" value=Q2:a>";

			Assert.AreEqual(expectedHtml, matches[1].Value);
		}

		[Test]
		public void AlternateFibItemLabelIsDisplayedInInputBox()
		{
			NewFibItem fibItem = new NewFibItem();
			fibItem.AlternateLabel = "My FIB";
			form.ItemList.Add(fibItem);

			CreateViewFromForm();

			string expectedHtml = "<INPUT class=blank contentEditable=false style=\"HEIGHT: 21px\" value=\"My FIB:a\">";

			string inputPattern = @"<INPUT[^>]+>";
			string actualHtml = Regex.Match(body.InnerHtml, inputPattern).Value;

			Assert.AreEqual(expectedHtml, actualHtml);
		}

		[Test]
		public void AlternateBlankLabelOverridesAlternateQuestionLabelDisplayedInInputBox()
		{
			IFibItem fibItem = new NewFibItem();
			fibItem.AlternateLabel = "My FIB";
			fibItem.BlankList[0].AlternateLabel = "First Blank";
			form.ItemList.Add(fibItem as IFormItem);

			CreateViewFromForm();

			string expectedHtml = "<INPUT class=blank contentEditable=false style=\"HEIGHT: 21px\" value=\"First Blank\">";

			string inputPattern = @"<INPUT[^>]+>";
			string actualHtml = Regex.Match(body.InnerHtml, inputPattern).Value;

			Assert.AreEqual(expectedHtml, actualHtml);
		}

		[Test]
		public void AlternateBlankLabelTheSameAfterProjectSynchronize()
		{
			IFibItem fibItem = new NewFibItem();
			fibItem.AlternateLabel = "My FIB";
			fibItem.BlankList[0].AlternateLabel = "First Blank";
			form.ItemList.Add(fibItem as IFormItem);

			CreateViewFromForm();

			Project.Events.RaiseSynchronizeProjectEvent();

			Assert.AreEqual("First Blank", fibItem.BlankList[0].AlternateLabel);
		}
	}
}
