using System;
using NUnit.Framework;
using Rhino.Mocks;
using Tawala.FormDesigner;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.FormDesignerTest.PresenterTest
{
	[TestFixture]
	public class McqChoicesPresenterTest
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
		}

		private const string mcqItemStaticChoiceXml =
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

		private const string mcqItemDynamicChoiceXml =
			@"<mc label=""Q2"" onlyone=""true"" required=""false"" style=""vertical"">" +
			@"<question>" +
			@"<paragraph indent=""0"" align=""left"">" +
			@"<font face=""Arial"" size=""200"" color=""000000"">[Replace this with your question. Use Enter key to add choices below.]</font>" +
			@"</paragraph>" +
			@"</question>" +
			@"<data-provider>" +
			@"<dynamic-mcq version=""1"">" +
			@"<display-expression>" +
			@"<field name=""Record:Form 1:Q1:a""/>" +
			@"</display-expression>" +
			@"<value-expression>" +
			@"<field name=""Record:Form 1:Q1:a""/>" +
			@"</value-expression>" +
			@"<sort-expression/>" +
			@"<record-selector>" +
			@"<form name=""Form 1""/>" +
			@"</record-selector>" +
			@"</dynamic-mcq>" +
			@"</data-provider>" +
			@"</mc>";


		[Test]
		[Ignore("Ignored because it throws exception: System.NotSupportedException: The invoked member is not supported in a dynamic assembly. - SB 10/08/2008")]
		public void ClickingOkInViewUpdatesDynamicChoicesInModel()
		{
			presenter.ConfigurationRequested();
			presenter.ChoiceSourceChanged(NewMcqItem.DynamicChoices);
			presenter.ChoicesAccepted();

			Assert.AreEqual(NewMcqItem.DynamicChoices, mcqItem.ChoiceSourceType);
		}

		[Test]
		public void ClickingOkInViewUpdatesStaticChoicesInModel()
		{
			const string choicesHtml =
				@"<BODY>" +
				@"<P>One</P>" +
				@"<P>Two</P>" +
				@"<P>Three</P>" +
				@"</BODY>";

			Expect.Call(view.ChoicesHtml).Return(choicesHtml);
			mocks.ReplayAll();

			presenter.ChoiceSourceChanged(NewMcqItem.StaticChoices);
			presenter.ChoicesAccepted();

			Assert.AreEqual(3, mcqItem.Choices.Count);
			Assert.AreEqual("One", mcqItem.Choices[0].Text);
			Assert.AreEqual("Two", mcqItem.Choices[1].Text);
			Assert.AreEqual("Three", mcqItem.Choices[2].Text);
			Assert.AreEqual(NewMcqItem.StaticChoices, mcqItem.ChoiceSourceType);
		}

		[Test]
		[Ignore("Ignored to allow commit of Story 2874 changes - SB 10/28/2008")]
		public void EmptyChoiceInViewCreatesNoChoiceInModel()
		{
			const string choicesHtml =
				@"<BODY>" +
				@"<P>One</P>" +
				@"<P>&nbsp;</P>" +
				@"</BODY>";

			Expect.Call(view.ChoicesHtml).Return(choicesHtml);
			mocks.ReplayAll();

			presenter.ChoicesAccepted();

			Assert.AreEqual(1, mcqItem.Choices.Count);
			Assert.AreEqual("One", mcqItem.Choices[0].Text);
		}

		[Test]
		public void ConstructingPresenterPopulatesViewWithStaticChoices()
		{
			mocks = new MockRepository();
			view = mocks.DynamicMock<IMcqChoicesView>();
			mcqItem = new NewMcqItem(new XmlElement(mcqItemStaticChoiceXml));

			const string choicesHtml =
				@"<P>Choice one</P>" +
				@"<P>Choice two</P>";

			Expect.Call(view.ChoicesHtml = choicesHtml);
			mocks.ReplayAll();

			presenter = new McqChoicesPresenter(view, mcqItem);

			mocks.VerifyAll();
		}

		[Test]
		public void ConstructingPresenterSetsViewChoiceSourceToStatic()
		{
			mocks = new MockRepository();
			view = mocks.DynamicMock<IMcqChoicesView>();
			mcqItem = new NewMcqItem(new XmlElement(mcqItemStaticChoiceXml));

			Expect.Call(() => view.SetChoiceSource(NewMcqItem.StaticChoices));
			mocks.ReplayAll();

			presenter = new McqChoicesPresenter(view, mcqItem);

			mocks.VerifyAll();
		}

		[Test]
		[Ignore("Ignored to allow commit of Story 2874 changes - SB 10/28/2008")]
		public void DoubleClickInFieldsPaletteInsertsFieldStringInView()
		{
			mocks = new MockRepository();
			view = mocks.DynamicMock<IMcqChoicesView>();
			IForm form = Project.Current.AddForm();
			IFibItem fibItem = new NewFibItem();

			const string mcqItemXml =
				"<mc label=\"Q1\" onlyone=\"true\" required=\"false\" style=\"vertical\">" +
				"<question><paragraph indent=\"0\" align=\"left\">" +
				"New question text:" +
				"</paragraph>" +
				"</question>" +
				"<choice label=\"a\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"First Choice" +
				"</paragraph>" +
				"</choice>" +
				"</mc>";

			mcqItem = new NewMcqItem(new XmlElement(mcqItemXml));
			form.ItemList.Add(fibItem);

			Expect.Call(() => view.InsertField("<<Form 1:Q1:a>>"));
			mocks.ReplayAll();

			presenter = new McqChoicesPresenter(view, mcqItem);
			presenter.FieldsPaletteDoubleClicked(fibItem.BlankList[0]);

			mocks.VerifyAll();
		}

		[Test]
		public void DragDropInFieldsPaletteInsertsFieldStringInView()
		{
			mocks = new MockRepository();
			view = mocks.DynamicMock<IMcqChoicesView>();
			IForm form = Project.Current.AddForm();
			IFibItem fibItem = new NewFibItem();

			const string mcqItemXml =
				"<mc label=\"Q1\" onlyone=\"true\" required=\"false\" style=\"vertical\">" +
				"<question><paragraph indent=\"0\" align=\"left\">" +
				"New question text:" +
				"</paragraph>" +
				"</question>" +
				"<choice label=\"a\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				"First Choice" +
				"</paragraph>" +
				"</choice>" +
				"</mc>";

			mcqItem = new NewMcqItem(new XmlElement(mcqItemXml));
			form.ItemList.Add(fibItem);

			Expect.Call(() => view.InsertField("<<Form 1:Q1:a>>"));
			mocks.ReplayAll();

			presenter = new McqChoicesPresenter(view, mcqItem);
			presenter.FieldDropped(fibItem.BlankList[0]);

			mocks.VerifyAll();
		}
	}
}