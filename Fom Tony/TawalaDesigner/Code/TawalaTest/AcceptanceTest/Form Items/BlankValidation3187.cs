// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Xml.XPath;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Forms;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FormItems
{
    [TestFixture]
    public class BlankValidation3187
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
        }

        #endregion

        [Test]
        public void BlankXmlContainsValidationFunctionXml()
        {
            var fib = new FibItem();
            var blank = new Blank(fib, 20);
            blank.ValidationFunction = FunctionLoader.Repository.Functions["email-validator"].CreateInstance();

            Assert.IsTrue(blank.ToXml().Contains("<email-validator"));
        }

        [Test]
        public void BlankXmlEndsWithClosingBlankXml()
        {
            var fib = new FibItem();
            var blank = new Blank(fib, 20);

            Assert.IsTrue(blank.ToXml().EndsWith("</blank>"));
        }

        [Test]
        public void FindBlankValidatorFunction()
        {
            IFunctionInfo functionInfo = FunctionLoader.Repository.Functions["email-validator"];

            Assert.IsNotNull(functionInfo);
        }

        [Test]
        public void ProjectXmlContainsValidationFunctionXml()
        {
            IForm form = Project.Current.AddForm();
            var fib = new FibItem();
            Blank blank = fib.BlankList[0];
            blank.ValidationFunction = FunctionLoader.Repository.Functions["email-validator"].CreateInstance();
            form.ItemList.Add(fib);

            Assert.IsTrue(Project.Current.ToXmlForSaving().Contains("<email-validator"));
        }

        [Test]
        public void ValidatorFunctionCount()
        {
            XPathNodeIterator iterator = FunctionLoader.Repository.Xml.Select("/component-repository/blank-validator");
            Assert.AreEqual(2, iterator.Count);
        }
    }
}