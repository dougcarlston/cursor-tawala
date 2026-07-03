// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NMock2;
using NMock2.Matchers;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using Tawala.Functions.ViewPresenter;
using Tawala.Projects;
using Tawala.Projects.Forms;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.Functions
{
    [TestFixture]
    public class DesignerConfiguresInitialFunction2003 : FunctionTest
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

        [Test]
        public void CanSetColumnOneContentsPropertyValue()
        {
            IFunction functionObject = makePopularChoiceCorrelationTable();

            var blank = new Blank(new FibItem(), 1);
            var functionBlank = new FunctionBlank(new RecordField(new Record("Record"), blank));
            functionObject.SetValue("popular-choice-display-field-name", functionBlank);

            Assert.AreSame(functionBlank, functionObject.GetValue("popular-choice-display-field-name"));
        }

        [Test]
        public void CanSetMainQuestionPropertyValue()
        {
            IFunction functionObject = makePopularChoiceCorrelationTable();

            var mcItem = new McqItem();
            var functionMCItem = new FunctionMCItem(new RecordField(new Record("Record"), mcItem));
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

            var mcItem = new McqItem();
            var functionMCItem = new FunctionMCItem(new RecordField(new Record("Record"), mcItem));
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

        // jdf - replace with Mapping class tests in FunctionRuntime (?)
        //[Test]
        //public void ParameterOfXmlTypeEnumerationHasEnumerationBindingSource()
        //{
        //    int enumerationTypeCount = 0;
        //    foreach (IFunctionInfo functionInfo in functionRepository.Functions)
        //    {
        //        foreach (IParameterInfo parameterInfo in functionInfo.Parameters)
        //        {
        //            if (parameterInfo.MapInfo.XmlType == "enumeration")
        //            {
        //                enumerationTypeCount++;
        //                ParameterBindingSource pbs = parameterInfo.BindingSource;
        //                Assert.AreEqual("EnumerationBindingSource", pbs.GetType().Name);
        //            }
        //        }
        //    }

        //    Assert.IsTrue(enumerationTypeCount > 0, "No IParameterInfo has 'type' of 'enumeration' so this test is broken!");
        //}
    }
}