using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Tawala.FormDesigner;
using System.Windows.Forms;
using Tawala.Functions.Runtime;
using Tawala.Proj;
using Tawala.Proj.Forms.NewFormItems;
using Tawala.Proj.Forms.FormItemContents;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;
using Tawala.Interfaces;

namespace TawalaTest.FormDesignerTest.StoryTest
{
	[TestFixture]
	public class FunctionInTextItem2672 : FunctionTestBase
	{
		private IFormView formView;
		private WebBrowser browser;

		private IForm form;
		private ITextItem textItem;

		[SetUp]
		public void SetUp()
		{
			functionSetup();

			Util.NewTestProject();
			ComponentMaker.UseNewComponents(true);
		}

		[TearDown]
		public void TearDown()
		{
			functionTearDown();
			ComponentMaker.UseNewComponents(false);

            if (formView != null)
            {
                ((FormView)formView).Close();
                formView = null;
            }
            browser = null;
            form = null;
            textItem = null;
		}

		[Test]
		public void CanCreateFunctionObject()
		{
			IFunction functionRecordCount = createRecordCountFunction();

			Assert.IsNotNull(functionRecordCount);
		}

		[Test]
		public void InsertingFunctionIntoTextItemResultsInFunctionInHtml()
		{
			createFormViewWithTextItem();

			IFunction functionRecordCount = createRecordCountFunction();

			setBookmarkInTextItem();

			formView.Presenter.InsertFunction(functionRecordCount);

			Assert.IsTrue(browser.Document.Body.InnerHtml.Contains("<t:function"));
		}

		[Test]
		public void InsertedFunctionHtmlElementCanBeRetrievedById()
		{
			createFormViewWithTextItem();

			IFunction functionRecordCount = createRecordCountFunction();

			setBookmarkInTextItem();

			formView.Presenter.InsertFunction(functionRecordCount);

			HtmlElement functionElement = browser.Document.GetElementById("func_" + functionRecordCount.InstanceId);

			Assert.IsNotNull(functionElement);
		}

		[Test]
		public void InsertedFunctionHtmlElementTextIsDisplayString()
		{
			createFormViewWithTextItem();

			IFunction functionRecordCount = createRecordCountFunction();
			functionRecordCount.SetValue("form-name", form);

			setBookmarkInTextItem();

			formView.Presenter.InsertFunction(functionRecordCount);

			HtmlElement functionElement = browser.Document.GetElementById("func_" + functionRecordCount.InstanceId);

			Assert.AreEqual("FORM RECORD COUNT(Form 1)", functionElement.InnerText);
		}

		[Test]
		public void FunctionWithBlankParameterGeneratesCorrectXml()
		{
			createFormViewWithTextItem();
			formView.Presenter.InsertFibItem(0);

			IFibItem fibItem = form.ItemList[0] as IFibItem;
			IBlank blank = fibItem.BlankList[0];

			var functionSum = createSumFunction(FunctionBlank.Parse(blank), form);

			string expectedFunctionXml =
				@"<sum version=""1"">" +
				@"<field>Record:Form 1:Q1:a</field>" +
				@"<conditions><form name=""Form 1"" /></conditions>" +
				@"</sum>";

			Assert.AreEqual(expectedFunctionXml, functionSum.ToXml());
		}

		[Test]
		public void InsertedFunctionWithBlankParameterGeneratesCorrectTextItemXml()
		{
			createFormViewWithTextItem();

			formView.Presenter.InsertFibItem(1);

			IFibItem fibItem = form.ItemList[1] as IFibItem;
			IBlank blank = fibItem.BlankList[0];

			var functionSum = createSumFunction(FunctionBlank.Parse(blank), form);

			setBookmarkInTextItem();

			formView.Presenter.InsertFunction(functionSum);

			Project.Events.RaiseSynchronizeProjectEvent();

			string expectedXml = 
				@"<text label=""T1"" style=""normal"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<sum version=""1""><field>Record:Form 1:Q1:a</field><conditions><form name=""Form 1"" /></conditions></sum>" +
				@"</paragraph>" +
				@"</text>" + Environment.NewLine;

			Assert.AreEqual(expectedXml, textItem.ToXml("T1"));
		}

		[Test]
		public void InsertedFunctionWithBlankParameterGeneratesCorrectXml()
		{
			createFormViewWithTextItem();

			formView.Presenter.InsertFibItem(1);

			IFibItem fibItem = form.ItemList[1] as IFibItem;
			IBlank blank = fibItem.BlankList[0];

			var functionSum = createSumFunction(FunctionBlank.Parse(blank), form);

			setBookmarkInTextItem();

			formView.Presenter.InsertFunction(functionSum);

			string expectedProjectXml =
				@"<project name=""Untitled"" themePath=""default"" format=""1.10"" designerBuild=""0"">" + Environment.NewLine +
				@"<forms>" + Environment.NewLine +
				@"<form name=""Form 1"" startPoint=""true""><items>" + Environment.NewLine +
				@"<text label=""T1"" style=""normal"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<sum version=""1""><field>Record:Form 1:Q1:a</field><conditions><form name=""Form 1"" /></conditions></sum>" +
				@"</paragraph>" +
				@"</text>" + Environment.NewLine +
				@"<fib label=""Q1"" style=""topLabels"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"[Replace this with your question. Underscores create blanks.] " +
				@"<blank label=""a"" length=""20"" height=""1"" required=""false""/>" +
				@"</paragraph>" +
				@"</fib>" + Environment.NewLine +
				@"</items>" + Environment.NewLine +
				@"</form></forms>" + Environment.NewLine +
				@"</project>" + Environment.NewLine;

			Assert.AreEqual(expectedProjectXml, Project.Current.ToXml());
		}

		[Test]
		public void InsertedFunctionWithHiddenFieldParameterGeneratesCorrectXml()
		{
			createFormViewWithTextItem();

			formView.Presenter.InsertFieldItem(1);

			var hiddenField = form.ItemList[1] as IHiddenField;
			var functionSum = createSumFunction(FunctionBlank.Parse(hiddenField), form);

			setBookmarkInTextItem();

			formView.Presenter.InsertFunction(functionSum);

			string expectedProjectXml =
				@"<project name=""Untitled"" themePath=""default"" format=""1.10"" designerBuild=""0"">" + Environment.NewLine +
				@"<forms>" + Environment.NewLine +
				@"<form name=""Form 1"" startPoint=""true""><items>" + Environment.NewLine +
				@"<text label=""T1"" style=""normal"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<sum version=""1""><field>Record:Form 1:Field1</field><conditions><form name=""Form 1"" /></conditions></sum>" +
				@"</paragraph>" +
				@"</text>" + Environment.NewLine +
				@"<field name=""Field1""/>" +
				Environment.NewLine +
				@"</items>" + Environment.NewLine +
				@"</form></forms>" + Environment.NewLine +
				@"</project>" + Environment.NewLine;

			Assert.AreEqual(expectedProjectXml, Project.Current.ToXml());
		}

		[Test]
		public void InsertedFunctionWithMcqParameterGeneratesCorrectXml()
		{
			createFormViewWithTextItem();

			formView.Presenter.InsertMcqItem(1);

			IFunction function = createResponseBarGraphFunction();
			IMcqItem mcqItem = form.ItemList[1] as IMcqItem;
			function.SetValue("field", FunctionMCItem.Parse(mcqItem, null));
			FunctionConditions conditions = new FunctionConditions();
			conditions.Forms.Add(form);
			function.SetValue("conditions", conditions);

			setBookmarkInTextItem();

			formView.Presenter.InsertFunction(function);

			string expectedProjectXml =
				@"<project name=""Untitled"" themePath=""default"" format=""1.10"" designerBuild=""0"">" + Environment.NewLine +
				@"<forms>" + Environment.NewLine +
				@"<form name=""Form 1"" startPoint=""true""><items>" + Environment.NewLine +
				@"<text label=""T1"" style=""normal"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<choice-tally-table version=""1""><field>Record:Form 1:Q1</field><conditions><form name=""Form 1"" /></conditions></choice-tally-table>" +
				@"</paragraph>" +
				@"</text>" + Environment.NewLine +
				@"<mc label=""Q1"" onlyone=""true"" required=""false"" style=""vertical""><question><paragraph indent=""0"" " +
				@"align=""left""><font face=""Arial"" size=""200"" color=""000000"">[Replace this with your question. Use Enter key to " +
				@"add choices below.]</font></paragraph></question><choice label=""a""></choice></mc>" + Environment.NewLine +
				@"</items>" + Environment.NewLine +
				@"</form></forms>" + Environment.NewLine +
				@"</project>" + Environment.NewLine;

			Assert.AreEqual(expectedProjectXml, Project.Current.ToXml());
		}

		[Test]
		public void CanConstructProjectWithFunctionReferencingFormFromXml()
		{
			string projectXml =
				@"<project name=""test"" themePath=""default"" format=""1.10"" designerBuild=""0"">" + Environment.NewLine +
				@"<forms>" + Environment.NewLine +
				@"<form name=""Form 1"" startPoint=""true""><items>" + Environment.NewLine +
				@"<text label=""T1"" style=""normal"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<record-count version=""3""><form-name>Form 1</form-name><conditions><form name=""Form 1"" /></conditions></record-count>" +
				@"</paragraph>" + 
				@"</text>" + Environment.NewLine +
				@"</items>" + Environment.NewLine +
				@"</form></forms>" + Environment.NewLine +
				@"</project>" + Environment.NewLine;
	
			Project.Create(new XmlElement(projectXml));

			Assert.AreEqual(projectXml, Project.Current.ToXml());
		}

		[Test]
		public void CanConstructProjectWithFunctionReferencingBlankFromXml()
		{
			string projectXml =
				@"<project name=""Sum-FormBlank"" themePath=""default"" format=""1.10"" designerBuild=""0"">" + Environment.NewLine +
				@"<forms>" + Environment.NewLine +
				@"<form name=""Form 1"" startPoint=""true""><items>" + Environment.NewLine +
				@"<text label=""T1"" style=""normal"">" + 
				@"<paragraph indent=""0"" align=""left"">[Replace this with text of your own.]" +
				@"<sum version=""1""><field>Record:Form 1:Q1:a</field><conditions><form name=""Form 1"" /></conditions></sum>" + 
				@"</paragraph></text>" + Environment.NewLine +
				@"<fib label=""Q1"" style=""topLabels""><paragraph indent=""0"" align=""left"">[Replace this with your question. " +
				@"Underscores create blanks.] <blank label=""a"" length=""20"" height=""1"" required=""false""/></paragraph></fib>" + Environment.NewLine +
				@"<mc label=""Q2"" onlyone=""true"" required=""false"" style=""vertical""><question><paragraph indent=""0"" " +
				@"align=""left""><font face=""Arial"" size=""200"" color=""000000"">[Replace this with your question. Use Enter key to " +
				@"add choices below.]</font></paragraph></question><choice label=""a""></choice></mc>" + Environment.NewLine +
				@"<field name=""Field1""/>" + Environment.NewLine +
				@"</items>" + Environment.NewLine +
				@"</form></forms>" + Environment.NewLine +
				@"</project>" + Environment.NewLine;

			Project.Create(new XmlElement(projectXml));

			Assert.AreEqual(projectXml, Project.Current.ToXml());
		}

		[Test]
		public void CanConstructProjectWithFunctionReferencingHiddenFieldFromXml()
		{
			string projectXml =
				@"<project name=""test"" themePath=""default"" format=""1.10"" designerBuild=""0"">" + Environment.NewLine +
				@"<forms>" + Environment.NewLine +
				@"<form name=""Form 1"" startPoint=""true""><items>" + Environment.NewLine +
				@"<text label=""T1"" style=""normal""><paragraph indent=""0"" align=""left"">[Replace this with text of your own.]" +
				@"<sum version=""1""><field>Record:Form 1:Field1</field><conditions><form name=""Form 1"" /></conditions></sum>" +
				@"</paragraph></text>" + Environment.NewLine +
				@"<field name=""Field1""/>" + Environment.NewLine +
				@"</items>" + Environment.NewLine +
				@"</form></forms>" + Environment.NewLine +
				@"</project>" + Environment.NewLine;

			Project.Create(new XmlElement(projectXml));

			Assert.AreEqual(projectXml, Project.Current.ToXml());
		}

		[Test]
		public void FunctionContentsFieldAcceptsNewTypes()
		{
			string projectXml =
				@"<project name=""Sum-FormBlank"" themePath=""default"" format=""1.10"" designerBuild=""0"">" + Environment.NewLine +
				@"<forms>" + Environment.NewLine +
				@"<form name=""Form 1"" startPoint=""true""><items>" + Environment.NewLine +
				@"<fib label=""Q1"" style=""topLabels""><paragraph indent=""0"" align=""left"">[Replace this with your question. " +
				@"Underscores create blanks.] <blank label=""a"" length=""20"" height=""1"" required=""false""/></paragraph></fib>" + Environment.NewLine +
				@"<mc label=""Q2"" onlyone=""true"" required=""false"" style=""vertical""><question><paragraph indent=""0"" " +
				@"align=""left""><font face=""Arial"" size=""200"" color=""000000"">[Replace this with your question. Use Enter key to " +
				@"add choices below.]</font></paragraph></question><choice label=""a""></choice></mc>" + Environment.NewLine +
				@"<field name=""Field1""/>" + Environment.NewLine +
				@"</items>" + Environment.NewLine +
				@"</form></forms>" + Environment.NewLine +
				@"</project>" + Environment.NewLine;

			Project.Create(new XmlElement(projectXml));

			IFibItem fibItem = Project.Current.FormList[0].ItemList[0] as IFibItem;
			IBlank blank = fibItem.BlankList[0];
			IMcqItem mcqItem = Project.Current.FormList[0].ItemList[1] as IMcqItem;
			IHiddenField hiddenField = Project.Current.FormList[0].ItemList[2] as IHiddenField;

			Assert.IsTrue(FunctionContentsField.AcceptedType(blank));
			Assert.IsTrue(FunctionContentsField.AcceptedType(mcqItem));
			Assert.IsTrue(FunctionContentsField.AcceptedType(hiddenField));
			Assert.IsTrue(FunctionContentsField.AcceptedType(new RecordField(new Record("Record1"),blank)));
			Assert.IsTrue(FunctionContentsField.AcceptedType(new RecordField(new Record("Record1"),mcqItem)));
			Assert.IsTrue(FunctionContentsField.AcceptedType(new RecordField(new Record("Record1"),hiddenField)));
			
		}

		[Test]
		public void NewFieldTypesImplementIDeserializedFieldSoConditionsWork()
		{
			string projectXml =
				@"<project name=""Sum-FormBlank"" themePath=""default"" format=""1.10"" designerBuild=""0"">" + Environment.NewLine +
				@"<forms>" + Environment.NewLine +
				@"<form name=""Form 1"" startPoint=""true""><items>" + Environment.NewLine +
				@"<fib label=""Q1"" style=""topLabels""><paragraph indent=""0"" align=""left"">[Replace this with your question. " +
				@"Underscores create blanks.] <blank label=""a"" length=""20"" height=""1"" required=""false""/></paragraph></fib>" + Environment.NewLine +
				@"<mc label=""Q2"" onlyone=""true"" required=""false"" style=""vertical""><question><paragraph indent=""0"" " +
				@"align=""left""><font face=""Arial"" size=""200"" color=""000000"">[Replace this with your question. Use Enter key to " +
				@"add choices below.]</font></paragraph></question><choice label=""a""></choice></mc>" + Environment.NewLine +
				@"<field name=""Field1""/>" + Environment.NewLine +
				@"</items>" + Environment.NewLine +
				@"</form></forms>" + Environment.NewLine +
				@"</project>" + Environment.NewLine;

			Project.Create(new XmlElement(projectXml));

			IFibItem fibItem = Project.Current.FormList[0].ItemList[0] as IFibItem;
			IBlank blank = fibItem.BlankList[0];
			IMcqItem mcqItem = Project.Current.FormList[0].ItemList[1] as IMcqItem;
			IHiddenField hiddenField = Project.Current.FormList[0].ItemList[2] as IHiddenField;

			Assert.AreSame(((IDeserializedField)blank).DeserializedFieldReference, blank);
			Assert.AreSame(((IDeserializedField)mcqItem).DeserializedFieldReference, mcqItem);
			Assert.AreSame(((IDeserializedField)hiddenField).DeserializedFieldReference, hiddenField);

		}

		private IFunction createRecordCountFunction()
		{
			IFunctionInfo functionInfo = functions["record-count"];
			return functionInfo.CreateInstance();
		}

		private IFunction createSumFunction()
		{
			IFunctionInfo functionInfo = functions["sum"];
			return functionInfo.CreateInstance();
		}

		private IFunction createSumFunction(object field, IForm formForConditions)
		{
			var function = createSumFunction();
			function.SetValue("field", field);

			var conditions = new FunctionConditions();
			conditions.Forms.Add(formForConditions);

			function.SetValue("conditions", conditions);

			return function;
		}

		private IFunction createResponseBarGraphFunction()
		{
			IFunctionInfo functionInfo = functions["choice-tally-table"];
			return functionInfo.CreateInstance();
		}

		private void createFormViewWithTextItem()
		{
			form = Project.Current.AddForm();
			textItem = new NewTextItem();
			form.ItemList.Add(textItem);

			formView = new FormView(form);

			browser = TestUtil.PumpMessagesUntilUIReady(formView);
		}

		private void setBookmarkInTextItem()
		{
			TestUtil.SelectFormItem(formView, textItem);
			TestUtil.SetBookmark(formView, "[Replace this with text of your own.]");
		}
	}
}
