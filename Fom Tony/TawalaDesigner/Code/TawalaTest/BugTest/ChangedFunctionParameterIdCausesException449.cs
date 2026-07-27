// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Function;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.BugTest
{
    [TestFixture]
    public class ChangedFunctionParameterIdCausesException449
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            form1 = Project.Current.AddForm();
        }

        #endregion

        private IForm form1;

        [Test]
        public void PersistenceXmlWithInvalidParameterResultsInValidFunction()
        {
            string xmlString =
                "<record-count version=\"1\">" +
                "<record-count-form-name>Form 1</record-count-form-name>" +
                "</record-count>";

            DocumentFunctionField functionField = new DocumentPersistedFunctionField(new XmlElement(xmlString));

            IFunction function = Project.FunctionMapById[functionField.FunctionInstanceId];
            Assert.IsNotNull(function, "Function is Null!");

            var functionConditions = function.Info.Parameters[0].GetValue(function) as FunctionFilterConditions;
            Assert.IsNull(functionConditions);
        }

        [Test]
        public void PersistenceXmlWithInvalidParameterThrowsException()
        {
            const string xmlString = "<record-count version=\"1\">" +
                                     "<record-count-form-name>Form 1</record-count-form-name>" +
                                     "</record-count>";

            try
            {
                DocumentFunctionField functionField = new DocumentPersistedFunctionField(new XmlElement(xmlString));
            }
            catch
            {
                Assert.Fail("Exception thrown because of invalid parameter!");
            }
        }
    }
}