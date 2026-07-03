using NUnit.Framework;
using Rhino.Mocks;
using Tawala.FormDesigner;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.StoryTest
{
	[TestFixture]
	public class McqChoicesFormattingTest2874
	{
        private MockRepository mocks;
		private IMcqItem mcqItem;
        private IMcqChoicesView view;
        private IMcqChoicesPresenter presenter;

		[SetUp]
		public void SetUp()
		{
			mocks = new MockRepository();
			view = mocks.DynamicMock<IMcqChoicesView>();
			mcqItem = new NewMcqItem();
			presenter = new McqChoicesPresenter(view, mcqItem);

			Util.NewTestProject();
			ComponentMaker.UseNewComponents(true);
		}

		[TearDown]
		public void TearDown()
		{
			ComponentMaker.UseNewComponents(false);
		}

		[Test]
		public void UnformattedChoicesInViewProduceUnformattedChoicesInModel()
		{
			const string choicesHtml =
				@"<BODY>" +
				@"<P>One</P>" +
				@"<P>Two</P>" +
				@"<P>Three</P>" +
				@"</BODY>";

			Expect.Call(view.ChoicesHtml).Return(choicesHtml);
			mocks.ReplayAll();

			presenter.ChoicesAccepted();

			Assert.AreEqual(@"<choice label=""a"">One</choice>", mcqItem.Choices[0].ToXml("a"));
			Assert.AreEqual(@"<choice label=""b"">Two</choice>", mcqItem.Choices[1].ToXml("b"));
			Assert.AreEqual(@"<choice label=""c"">Three</choice>", mcqItem.Choices[2].ToXml("c"));
		}

		[Test]
		public void UnformattedChoicesInViewProduceUnformattedChoicesInXml()
		{
			const string choicesHtml =
				@"<BODY>" +
				@"<P>One</P>" +
				@"<P>Two</P>" +
				@"<P>Three</P>" +
				@"</BODY>";

			Expect.Call(view.ChoicesHtml).Return(choicesHtml);
			mocks.ReplayAll();

			presenter.ChoicesAccepted();

			Assert.AreEqual(@"<choice label=""a"">One</choice>", mcqItem.Choices[0].ToXml("a"));
			Assert.AreEqual(@"<choice label=""b"">Two</choice>", mcqItem.Choices[1].ToXml("b"));
			Assert.AreEqual(@"<choice label=""c"">Three</choice>", mcqItem.Choices[2].ToXml("c"));
		}

		[Test]
		public void BoldChoiceInViewProducesBoldChoiceInModel()
		{
			const string choicesHtml =
				@"<BODY>" +
				@"<P><STRONG>One</STRONG></P>" +
				@"<P>Two</P>" +
				@"<P>Three</P>" +
				@"</BODY>";

			Expect.Call(view.ChoicesHtml).Return(choicesHtml);
			mocks.ReplayAll();

			presenter.ChoicesAccepted();

			Assert.AreEqual(@"<choice label=""a""><b>One</b></choice>", mcqItem.Choices[0].ToXml("a"));
			Assert.AreEqual(@"<choice label=""b"">Two</choice>", mcqItem.Choices[1].ToXml("b"));
			Assert.AreEqual(@"<choice label=""c"">Three</choice>", mcqItem.Choices[2].ToXml("c"));
		}

		[Test]
		public void ItalicChoiceInViewProducesItalicChoiceInModel()
		{
			const string choicesHtml =
				@"<BODY>" +
				@"<P>One</P>" +
				@"<P><EM>Two</EM></P>" +
				@"<P>Three</P>" +
				@"</BODY>";

			Expect.Call(view.ChoicesHtml).Return(choicesHtml);
			mocks.ReplayAll();

			presenter.ChoicesAccepted();

			Assert.AreEqual(@"<choice label=""a"">One</choice>", mcqItem.Choices[0].ToXml("a"));
			Assert.AreEqual(@"<choice label=""b""><i>Two</i></choice>", mcqItem.Choices[1].ToXml("b"));
			Assert.AreEqual(@"<choice label=""c"">Three</choice>", mcqItem.Choices[2].ToXml("c"));
		}

		[Test]
		public void UnderlineChoiceInViewProducesUnderlineChoiceInModel()
		{
			const string choicesHtml =
				@"<BODY>" +
				@"<P>One</P>" +
				@"<P>Two</P>" +
				@"<P><U>Three</U></P>" +
				@"</BODY>";

			Expect.Call(view.ChoicesHtml).Return(choicesHtml);
			mocks.ReplayAll();

			presenter.ChoicesAccepted();

			Assert.AreEqual(@"<choice label=""a"">One</choice>", mcqItem.Choices[0].ToXml("a"));
			Assert.AreEqual(@"<choice label=""b"">Two</choice>", mcqItem.Choices[1].ToXml("b"));
			Assert.AreEqual(@"<choice label=""c""><u>Three</u></choice>", mcqItem.Choices[2].ToXml("c"));
		}

		[Test]
		public void BoldChoiceInXmlProducesBoldChoiceInModel()
		{
			const string xmlString =
				@"<mc label=""Q1"" onlyone=""true"" required=""false"" style=""vertical"">" +
				@"<question>" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">Question Text</font>" +
				@"</paragraph>" +
				@"</question>" +
				@"<choice label=""a""><b>One</b></choice>" +
				@"<choice label=""b"">Two</choice>" +
				@"<choice label=""c"">Three</choice>" +
				@"</mc>";
	
			mcqItem = new NewMcqItem(new XmlElement(xmlString));

			Assert.AreEqual(@"<choice label=""a""><b>One</b></choice>", mcqItem.Choices[0].ToXml("a"));
			Assert.AreEqual(@"<choice label=""b"">Two</choice>", mcqItem.Choices[1].ToXml("b"));
			Assert.AreEqual(@"<choice label=""c"">Three</choice>", mcqItem.Choices[2].ToXml("c"));
		}

		[Test]
		public void ClickingOkInViewUpdatesStaticChoicesWithFieldInModel()
		{
			IForm form = Project.Current.AddForm();
			form.ItemList.Add(new NewFibItem());

			const string choicesHtml =
				@"<BODY>" +
				@"<P>&lt;&lt;Form 1:Q1:a&gt;&gt;</P>" +
				@"</BODY>";

			Expect.Call(view.ChoicesHtml).Return(choicesHtml);
			mocks.ReplayAll();

			presenter.ChoicesAccepted();

			Assert.AreEqual(1, mcqItem.Choices.Count);
			Assert.AreEqual("<<Form 1:Q1:a>>", mcqItem.Choices[0].Text);

			string expectedXml =
				@"<choice label=""a"">" +
				@"<field name=""Form 1:Q1:a""/>" +
				@"</choice>";

			Assert.AreEqual(expectedXml, mcqItem.Choices[0].ToXml("a"));
		}

		[Test]
		public void ClickingOkInViewUpdatesStaticChoicesWithFieldInView()
		{
			IForm form = Project.Current.AddForm();

			IFibItem fibItem = new NewFibItem();
			form.ItemList.Add(fibItem);

			const string choicesHtml =
				@"<BODY>" +
				@"<P><t:field name=""Form 1:Q1:a""/></P>" +
				@"</BODY>";

			Expect.Call(view.ChoicesHtml).Return(choicesHtml);
			mocks.ReplayAll();

			presenter.ChoicesAccepted();

			const string expectedXhtmlFormat =
				@"<t:Choice><span class=""choice"">a</span><input type=""radio"" />" +
				@"<span>" +
				@"<t:field fieldID=""{0}"">Form 1:Q1:a</t:field>" +
				@"</span>" +
				@"<br />" +
				@"</t:Choice>";

			Assert.AreEqual(string.Format(expectedXhtmlFormat, fibItem.BlankList[0].Id), mcqItem.Choices.ToXhtml(mcqItem));
		}
	}
}
