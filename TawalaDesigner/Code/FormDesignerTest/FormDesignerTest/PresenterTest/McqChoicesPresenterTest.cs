using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using NUnit.Framework;
using Tawala.Proj;
using Tawala.Proj.Forms.FormItemContents;
using Tawala.Proj.Forms.NewFormItems;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;
using Tawala.FormDesigner;

namespace TawalaTest.FormDesignerTest.PresenterTest
{
	[TestFixture]
	public class McqChoicesPresenterTest
	{
		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
		}

		private Collection<IChoice> makeChoices()
		{
			return new Collection<IChoice>();
		}

		private string mcqItemStaticChoiceXml =
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

		string mcqItemDynamicChoiceXml =
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
		public void ConstructingPresenterPopulatesViewWithStaticChoices()
		{
			IMcqItem mcqItem = new NewMcqItem(new XmlElement(mcqItemStaticChoiceXml));

			IMcqChoicesView view = new DummyView(mcqItem);

			Assert.AreEqual("Choice one\r\nChoice two\r\n", view.ChoiceStrings);
		}

		[Test]
		public void ConstructingPresenterSetsViewChoiceSourceToStatic()
		{
			IMcqItem mcqItem = new NewMcqItem(new XmlElement(mcqItemStaticChoiceXml));

			IMcqChoicesView view = new DummyView(mcqItem);

			Assert.AreEqual(NewMcqItem.StaticChoices, ((DummyView)view).ChoiceSourceIndex);
		}

		[Test]
		public void SettingPresentChoiceSourceToStaticSetsModelChoiceSourceToStatic()
		{
			IMcqItem mcqItem = new NewMcqItem();
			IMcqChoicesView view = new DummyView(mcqItem);
			IMcqChoicesPresenter presenter = getPresenter(view);

			presenter.ChoiceSourceChanged(NewMcqItem.StaticChoices);

			Assert.AreEqual(NewMcqItem.StaticChoices, mcqItem.ChoiceSourceIndex);
		}

		[Test]
		public void ConstructingPresenterSetsViewChoiceSourceToDynamic()
		{
			IMcqItem mcqItem = new NewMcqItem(new XmlElement(mcqItemDynamicChoiceXml));

			IMcqChoicesView view = new DummyView(mcqItem);

			Assert.AreEqual(NewMcqItem.DynamicChoices, ((DummyView)view).ChoiceSourceIndex);
		}

		[Test]
		public void ClickingOkInViewUpdatesStaticChoicesInModel()
		{
			IMcqItem mcqItem = new NewMcqItem();

			IMcqChoicesView view = new DummyView(mcqItem);

			view.ChoiceStrings = "One\r\nTwo\r\nThree\r\n";

			IMcqChoicesPresenter presenter = getPresenter(view);
			presenter.ChoicesAccepted();

			Assert.AreEqual(3, mcqItem.Choices.Count);
			Assert.AreEqual("One", mcqItem.Choices[0].Text);
			Assert.AreEqual("Two", mcqItem.Choices[1].Text);
			Assert.AreEqual("Three", mcqItem.Choices[2].Text);
		}

		[Test]
		public void ClickingOkInViewUpdatesDynamicChoicesInModel()
		{
			IMcqItem mcqItem = new NewMcqItem();
			IMcqChoicesView view = new DummyView(mcqItem);
			IMcqChoicesPresenter presenter = getPresenter(view);

			presenter.ChoicesAccepted();
		}

		[Test]
		public void DoubleClickInFieldsPaletteInsertsFieldStringInView()
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
				"</mc>";

			IForm form = Project.Current.AddForm();
			IFibItem fibItem = new NewFibItem();
			IMcqItem mcqItem = new NewMcqItem(new XmlElement(mcqItemXml));
			form.ItemList.Add(fibItem as IFormItem);

			IMcqChoicesView view = new DummyView(mcqItem);
			IMcqChoicesPresenter presenter = getPresenter(view);

			presenter.FieldsPaletteDoubleClicked(fibItem.BlankList[0]);

			Assert.AreEqual("Choice one\r\n<<Form 1:Q1:a>>\r\n", view.ChoiceStrings);
		}

		[Test]
		public void DragDropInFieldsPaletteInsertsFieldStringInView()
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
				"</mc>";

			IForm form = Project.Current.AddForm();
			IFibItem fibItem = new NewFibItem();
			IMcqItem mcqItem = new NewMcqItem(new XmlElement(mcqItemXml));
			form.ItemList.Add(fibItem as IFormItem);

			IMcqChoicesView view = new DummyView(mcqItem);
			IMcqChoicesPresenter presenter = getPresenter(view);

			presenter.FieldDropped(fibItem.BlankList[0]);

			Assert.AreEqual("Choice one\r\n<<Form 1:Q1:a>>\r\n", view.ChoiceStrings);
		}

		private static IMcqChoicesPresenter getPresenter(IMcqChoicesView view)
		{
			IMcqChoicesPresenter presenter = Reflect<DummyView>.GetField<IMcqChoicesPresenter>("presenter", ((DummyView)view));
			return presenter;
		}
	}

	internal class DummyView : IMcqChoicesView
	{
		private IMcqChoicesPresenter presenter;
		private string choiceStrings;
		public int ChoiceSourceIndex;

		internal DummyView(IMcqItem mcqItem)
		{
			presenter = new McqChoicesPresenter(this, mcqItem);
		}


		#region IMcqChoicesView Members

		public string ChoiceStrings
		{
			get { return choiceStrings; }
			set { choiceStrings = value; }
		}


		public void InsertFieldString(string fieldString)
		{
			choiceStrings += fieldString + "\r\n";
		}

		public void SetChoiceSource(int choiceSourceIndex)
		{
			this.ChoiceSourceIndex = choiceSourceIndex;
		}

		public void EnableChoiceConfiguration(bool enable)
		{
			
		}

		#endregion
	}
}
