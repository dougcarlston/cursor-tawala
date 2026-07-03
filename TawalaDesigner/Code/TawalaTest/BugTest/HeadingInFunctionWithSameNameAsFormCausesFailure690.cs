// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Reflection;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.BugTest
{
    [TestFixture]
    public class HeadingInFunctionWithSameNameAsFormCausesFailure690 : FunctionTestBase
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            form1 = Project.Current.AddForm();
            form2 = Project.Current.AddForm();

            form1.Name = "Owner";
            form2.Name = "Dog";

            fibItem = new FibItem();

            form1.ItemList.Add(fibItem);

            Assert.AreEqual(1, fibItem.BlankList.Count);
        }

        [TearDown]
        public void TearDown()
        {
            form1 = form2 = null;
            fibItem = null;
        }

        #endregion

        private IForm form1, form2;
        private FibItem fibItem;

        private IFunction setupFunction()
        {
            IFunction function = FunctionLoader.Repository.Functions["itemization-table"].CreateInstance();

            IParameterInfo compositeCollection = function.Info.Parameters["column"];
            var collection = Activator.CreateInstance(compositeCollection.PropertyType) as ICompositeParameterCollection;

            ICompositeParameter composite1 = collection.CreateItem();
            composite1["header"] = new FunctionCompoundExpression(new XmlElement("<container><string value=\"Dog\"/></container>"));
            composite1["contents"] = new FunctionContentsField(fibItem.BlankList[0]);

            collection.Add(composite1);

            Assert.AreEqual(1, collection.Count);

            compositeCollection.SetValue(function, collection);

            return function;
        }

        [Test]
        public void CorrectFormsAreInferredFromFunctionParameters()
        {
            Type formsHelperType = getReferencedFormsHelperType();
            Assert.IsNotNull(formsHelperType);

            IFunction function = setupFunction();
            Assert.IsNotNull(function);

            var formCollection =
                formsHelperType.InvokeMember("Get", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null,
                                             new object[] {function}) as FunctionFormCollection;

            Assert.AreEqual(1, formCollection.Count);
            Assert.AreEqual("Owner", formCollection[0].Name);
        }
    }
}