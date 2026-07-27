using System;
using System.Text;
using NUnit.Framework;
using NMock2;
using TawalaTest.TestSupport;
using Tawala.XmlSupport;
using Tawala.Proj;
using Tawala.ConfigurableFunction;
using Tawala.Controls;

namespace TawalaTest.AcceptanceTest
{
	[TestFixture]
	public class ConfigurableFunctionTest
	{
		private Mockery mocks;
		private IConfigureFunctionViewPhase2 mockView;

		private Form form;
		private MCItem mcItem1;
		private MCItem mcItem2;
		private FibItem fibItem;
		private Blank blank;

		protected string repositoryXmlString =
			"<tr:component-repository xmlns:tr=\"http://www.tawala.com/component-repository\" signature=\"-736646183\">" +
			"<tr:display-component-categories>" +
			"<tr:category name=\"Popular Choice\">" +
			"<tr:element-id>popular-choice-correlation-table</tr:element-id>" +
			"</tr:category>" +
			"</tr:display-component-categories>" +
			"<tr:display-component id=\"popular-choice-correlation-table\" name=\"POPULAR CHOICE CORRELATION TABLE\" version=\"1\">" +
			"<tr:description>Displays a table showing the most popular choice for a given multiple choice question, and correlates that question's choices with those from a second multiple choice question.</tr:description>" +
			"<tr:parameter id=\"rank\" type=\"enumeration\" name=\"Rank\" required=\"true\">" +
			"<tr:description>Indicates the ranking of the popular choice, e.g. first, second, etc.</tr:description>" +
			"<tr:choice value=\"1\" description=\"first\" />" +
			"<tr:choice value=\"2\" description=\"second\" />" +
			"<tr:choice value=\"3\" description=\"third\" />" +
			"</tr:parameter>" +
			"<tr:parameter id=\"choice-available-field-name\" type=\"tawala-mcq\" name=\"Main Question\" required=\"true\">" +
			"<tr:description>The multiple choice question to display popular choice information for.</tr:description>" +
			"</tr:parameter>" +
			"<tr:parameter id=\"choice-preferred-field-name\" type=\"tawala-mcq\" name=\"Second Question\" required=\"true\">" +
			"<tr:description>The multiple choice question to correlate with the main question.</tr:description>" +
			"</tr:parameter>" +
			"<tr:parameter id=\"popular-choice-display-field-name\" type=\"tawala-blank\" name=\"Column One Contents\" required=\"true\">" +
			"<tr:description>The blank whose values will be shown in the first column of the table.</tr:description>" +
			"</tr:parameter>" +
			"</tr:display-component>" +
			"<tr:function-categories>" +
			"<tr:category name=\"Form Access\">" +
			"<tr:element-id>record-count</tr:element-id>" +
			"</tr:category>" +
			"</tr:function-categories>" +
			"<tr:function id=\"record-count\" name=\"RECORD COUNT\" version=\"1\" return-type=\"int\">" +
			"<tr:description>Returns the number of submitted records in a form.</tr:description>" +
			"<tr:parameter id=\"form-name\" type=\"tawala-form\" name=\"Form\" required=\"true\">" +
			"<tr:description>Name of one of the forms in the project.</tr:description>" +
			"</tr:parameter>" +
			"</tr:function>" +
			"</tr:component-repository>";

		protected string displayComponentXmlString =
			"<tr:display-component id=\"popular-choice-correlation-table\" name=\"POPULAR CHOICE CORRELATION TABLE\" version=\"1\" xmlns:tr=\"http://www.tawala.com/component-repository\">" +
			"<tr:description>Displays a table showing the most popular choice for a given multiple choice question, and correlates that question's choices with those from a second multiple choice question.</tr:description>" +
			"<tr:parameter id=\"rank\" type=\"enumeration\" name=\"Rank\" required=\"true\">" +
			"<tr:description>Indicates the ranking of the popular choice, e.g. first, second, etc.</tr:description>" +
			"<tr:choice value=\"1\" description=\"first\" />" +
			"<tr:choice value=\"2\" description=\"second\" />" +
			"<tr:choice value=\"3\" description=\"third\" />" +
			"</tr:parameter>" +
			"<tr:parameter id=\"choice-available-field-name\" type=\"tawala-mcq\" name=\"Main Question\" required=\"true\">" +
			"<tr:description>The multiple choice question to display popular choice information for.</tr:description>" +
			"</tr:parameter>" +
			"<tr:parameter id=\"choice-preferred-field-name\" type=\"tawala-mcq\" name=\"Second Question\" required=\"true\">" +
			"<tr:description>The multiple choice question to correlate with the main question.</tr:description>" +
			"</tr:parameter>" +
			"<tr:parameter id=\"popular-choice-display-field-name\" type=\"tawala-blank\" name=\"Column One Contents\" required=\"true\">" +
			"<tr:description>The blank whose values will be shown in the first column of the table.</tr:description>" +
			"</tr:parameter>" +
			"</tr:display-component>";

		[SetUp]
		public void SetUp()
		{
			mocks = new Mockery();
			mockView = mocks.NewMock<IConfigureFunctionViewPhase2>();

			Util.NewTestProject();

			form = Project.Current.AddForm();

			mcItem1 = new MCItem();
			form.Add(mcItem1);

			mcItem2 = new MCItem();
			form.Add(mcItem2);

			fibItem = new FibItem();
			form.Add(fibItem);
			blank = fibItem.BlankList[0];
		}
		
		[Test]
		[Ignore("Under development - SB")]
		public void ConfiguredFunctionProducesConfiguredPersistenceXml()
		{
			IXmlElement functionElement = new XmlElement(displayComponentXmlString);

			Expect.Once.On(mockView).SetProperty("Presenter").To(Is.NotNull);
			Expect.Once.On(mockView).Method("MakeVisible").WithNoArguments();
			Expect.Once.On(mockView).SetProperty("FunctionName").To("POPULAR CHOICE CORRELATION TABLE");
			Expect.Once.On(mockView).SetProperty("FunctionDescription").To("Displays a table showing the most popular choice for a given multiple choice question, and correlates that question's choices with those from a second multiple choice question.");
			Expect.Once.On(mockView).SetProperty("Parameters").To(Is.NotNull);

			string expectedXml =
				"<popular-choice-correlation-table version=\"1\">" +
				"<rank>2</rank>" +
				"<choice-available-field-name>Form 1:Q1</choice-available-field-name>" +
				"<choice-preferred-field-name>Form 1:Q2</choice-preferred-field-name>" +
				"<popular-choice-display-field-name>Form 1:Q3:a</popular-choice-display-field-name>" +
				"</popular-choice-correlation-table>";

			IConfigureFunctionPresenter presenter = new ConfigureFunctionPresenter(mockView, functionElement);
			presenter.ConfigureFunction();

			Assert.AreEqual(expectedXml, presenter.ToXml());

			mocks.VerifyAllExpectationsHaveBeenMet();
		}

		// "view" shows function name
		// "view" shows function description
		// "view" shows function parameters
		// "view" shows selected parameter name
		// "view" shows selected parameter description
		// OK button disabled until all parameters configured
		// OK button signals configuration complete
		// MCQ control only allows drop of MCQ
		// Blank control only allows drop of Blank

		[Test]
		public void RankParameterXmlProducesEnumerationParameter()
		{
			string enumerationParameterXmlString =
				"<tr:parameter id=\"rank\" type=\"enumeration\" name=\"Rank\" required=\"true\" xmlns:tr=\"http://www.tawala.com/component-repository\">" +
				"<tr:description>Indicates the ranking of the popular choice, e.g. first, second, etc.</tr:description>" +
				"<tr:choice value=\"1\" description=\"first\" />" +
				"<tr:choice value=\"2\" description=\"second\" />" +
				"<tr:choice value=\"3\" description=\"third\" />" +
				"</tr:parameter>";

			IXmlElement element = new XmlElement(enumerationParameterXmlString);
			EnumerationParameter parameter = new EnumerationParameter(element);

			Assert.AreEqual("rank", parameter.Id);
			Assert.AreEqual("Rank", parameter.Name);
			Assert.AreEqual("enumeration", parameter.Type);
			Assert.AreEqual(true, parameter.Required);
			Assert.AreEqual("Indicates the ranking of the popular choice, e.g. first, second, etc.", parameter.Description);

			Assert.AreEqual(3, parameter.Choices.Count);
			Assert.AreEqual("first", parameter.Choices[0].Name);
			Assert.AreEqual(1, parameter.Choices[0].Value);

			Assert.AreEqual("second", parameter.Choices[1].Name);
			Assert.AreEqual(2, parameter.Choices[1].Value);

			Assert.AreEqual("third", parameter.Choices[2].Name);
			Assert.AreEqual(3, parameter.Choices[2].Value);
		}

	}

}
