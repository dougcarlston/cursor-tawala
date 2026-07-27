using System;
using NMock2;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using Tawala.Functions.ViewPresenter;
using Tawala.Projects;
using TawalaTest.TestingSupport;

namespace TawalaTest.StoryTest.Functions
{
    [TestFixture]
    public class DesignerSeesFunctionsTest2001 : FunctionTestBase
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            mocks = new Mockery();
            mockView = mocks.NewMock<IInsertFunctionView>();
        }

        #endregion

        private Mockery mocks;
        private IInsertFunctionView mockView;

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

        private string popularChoiceTableDescription =
            "Computes the most common response to a multiple-choice question and displays a list of the contents of an associated field for users who have chosen that response. It also compares the response choice of one multiple-choice question with a second multiple-choice question.";

        [Test]
        public void CreatingTwoFunctionObjectsFromSameTypeProducesUniqueObjects()
        {
            try
            {
                IFunction functionObject1 = functions["popular-choice-correlation-table"].CreateInstance();
                IFunction functionObject2 = functions["popular-choice-correlation-table"].CreateInstance();
                Assert.AreNotSame(functionObject1, functionObject2);
                Assert.AreSame(functionObject1.GetType(), functionObject2.GetType());
            }
            catch (Exception e)
            {
                Assert.Fail(e.ToString());
            }
        }

        [Test]
        public void FunctionAssemblyGeneratedFromRepositoryXmlHasComponentTypes()
        {
            Type sumType = functions["sum"].Type;
            Type recordCountType = functions["record-count"].Type;

            Assert.IsNotNull(sumType);
            Assert.IsNotNull(recordCountType);
        }

        [Test]
        public void InsertFunctionPresenterConstructorCallsViewSetup()
        {
            Expect.Once.On(mockView).Method("Setup").With(functionAssembly.Categories);

            IInsertFunctionPresenter presenter = new InsertFunctionPresenter(mockView);

            mocks.VerifyAllExpectationsHaveBeenMet();
        }

        [Test]
        public void PopularChoiceTableClassHasFunctionInfo()
        {
            IFunctionInfo functionInfo = functions["popular-choice-correlation-table"];

            Assert.AreEqual("popular-choice-correlation-table", functionInfo.Id);
            Assert.AreEqual("RANKED MULTIQUESTION RESPONSE LIST", functionInfo.Name);
            Assert.AreEqual(popularChoiceTableDescription, functionInfo.Description);
            Assert.AreEqual("1", functionInfo.Version);
        }

        [Test]
        public void PopularChoiceTableObjectHasFunctionInfo()
        {
            IFunction functionObject = functions["popular-choice-correlation-table"].CreateInstance();
            IFunctionInfo functionInfo = functionObject.Info;

            Assert.AreEqual("popular-choice-correlation-table", functionInfo.Id);
            Assert.AreEqual("RANKED MULTIQUESTION RESPONSE LIST", functionInfo.Name);
            Assert.AreEqual(popularChoiceTableDescription, functionInfo.Description);
            Assert.AreEqual("1", functionInfo.Version);
        }

        [Test]
        public void PopularChoiceTableXmlProducesPopularChoiceTableObject()
        {
            IFunction functionObject = functions["popular-choice-correlation-table"].CreateInstance();
            Assert.AreEqual("popular-choice-correlation-table", functionObject.Info.Id);
        }

        [Test]
        public void XmlParameterBecomesObjectProperty()
        {
            IFunction functionObject = functions["popular-choice-correlation-table"].CreateInstance();
            IParameterInfo parameterInfo = functionObject.Info.Parameters["choice-available-field-name"];

            Assert.AreEqual("choice-available-field-name", parameterInfo.Id);
            Assert.AreSame(typeof(FunctionMCItem), parameterInfo.PropertyType);
            Assert.AreEqual("Main Question", parameterInfo.Name);
        }
    }
}