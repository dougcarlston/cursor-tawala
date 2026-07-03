using System;
using NMock2;
using NMock2.Matchers;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using Tawala.Functions.ViewPresenter;
using Tawala.Projects;
using Tawala.Projects.Forms.NewFormItems;
using TawalaTest.TestingSupport;

namespace TawalaTest.StoryTest.Functions
{
    [TestFixture]
    public class DesignerConfiguresInitialFunction2003 : FunctionTestBase
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            mocks = new Mockery();
            mockView = mocks.NewMock<IConfigureFunctionView>();
        }

        #endregion

        private Mockery mocks;
        private IConfigureFunctionView mockView;

        private IFunction makePopularChoiceCorrelationTable()
        {
            return functions["popular-choice-correlation-table"].CreateInstance();
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            functionSetup();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            functionTearDown();
        }

        private void functionConfiguredHandler(object o, FunctionConfiguredEventArgs args)
        {
        }

        [Test, Ignore("Fix this test?")]
        public void CanSetColumnOneContentsPropertyValue()
        {
            IFunction functionObject = makePopularChoiceCorrelationTable();
#if FIXED
            IBlank blank = new NewBlank(new FibItem(), 1);
			FunctionBlank functionBlank = new FunctionBlank();
			functionBlank.Field = new RecordField(new Record("Record"), blank);
			functionObject.SetValue("popular-choice-display-field-name", functionBlank);

			Assert.AreSame(functionBlank, functionObject.GetValue("popular-choice-display-field-name"));
#endif
        }

        [Test]
        public void CanSetMainQuestionPropertyValue()
        {
            IFunction functionObject = makePopularChoiceCorrelationTable();

            IMcqItem mcItem = new NewMcqItem();
            var functionMCItem = new FunctionMCItem();
            functionMCItem.Field = new RecordField(new Record("Record"), mcItem);
            functionObject.SetValue("choice-available-field-name", functionMCItem);

            Assert.AreSame(functionMCItem, functionObject.GetValue("choice-available-field-name"));
        }

        [Test]
        public void CanSetRankPropertyValue()
        {
            IFunction functionObject = makePopularChoiceCorrelationTable();

            functionObject.SetValue("rank", "1");

            Assert.AreEqual("1", functionObject.GetValue("rank"));
        }

        [Test]
        public void CanSetSecondQuestionPropertyValue()
        {
            IFunction functionObject = makePopularChoiceCorrelationTable();

            IMcqItem mcItem = new NewMcqItem();
            var functionMCItem = new FunctionMCItem();
            functionMCItem.Field = new RecordField(new Record("Record"), mcItem);
            functionObject.SetValue("choice-preferred-field-name", functionMCItem);

            Assert.AreSame(functionMCItem, functionObject.GetValue("choice-preferred-field-name"));
        }

        [Test]
        public void PopularChoiceCorrelationTableHasMainQuestionProperty()
        {
            IFunction functionObject = makePopularChoiceCorrelationTable();
            IParameterInfo parameterInfo = functionObject.Info.Parameters["choice-available-field-name"];

            Assert.AreEqual("choice-available-field-name", parameterInfo.Id);
            Assert.AreEqual("Main Question", parameterInfo.Name);
            Assert.AreEqual(typeof(FunctionMCItem), parameterInfo.PropertyType);
        }

        [Test]
        public void PresenterCreateFunctionCallsViewSetFunctionWithTrue()
        {
            IFunctionInfo info = functions["popular-choice-correlation-table"];

            Expect.Once.On(mockView).Method("SetFunction").With(Has.Property("Info", Is.Same(info)), Is.EqualTo(true));
            IConfigureFunctionPresenter presenter = new ConfigureFunctionPresenter(mockView);
            presenter.CreateFunction(info, functionConfiguredHandler);

            mocks.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void PresenterCreateFunctionWithNullDoesNotCallViewsSetFunction()
        {
            Expect.Never.On(mockView).Method("SetFunction").With(new NullMatcher());
            IConfigureFunctionPresenter presenter = new ConfigureFunctionPresenter(mockView);
            presenter.CreateFunction(null, functionConfiguredHandler);

            mocks.VerifyAllExpectationsHaveBeenMet();
        }
    }
}