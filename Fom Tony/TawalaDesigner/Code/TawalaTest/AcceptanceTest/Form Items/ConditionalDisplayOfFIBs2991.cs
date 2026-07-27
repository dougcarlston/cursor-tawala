// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FormItems
{
    [TestFixture]
    public class ConditionalDisplayOfFIBs2991
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
        }

        #endregion

        private static readonly string fibXmlWithConditions =
            @"<fib label=""Q1"" style=""topLabels"">" +
            @"<paragraph indent=""0"" align=""left"">" +
            @"<font face=""Arial"" size=""180"" color=""000000"">Name: </font><blank label=""a"" length=""10"" required=""false""></blank>" +
            @"</paragraph>" +
            @"<displayConditions>" + Environment.NewLine +
            @"<equals field=""var"">" + Environment.NewLine +
            @"<string value=""foo""/>" + Environment.NewLine +
            @"</equals>" + Environment.NewLine +
            @"</displayConditions>" +
            @"</fib>" + Environment.NewLine;

        private static readonly string fibXmlWithoutConditions =
            @"<fib label=""Q1"" style=""topLabels"">" +
            @"<paragraph indent=""0"" align=""left"">" +
            @"<font face=""Arial"" size=""180"" color=""000000"">Name: </font><blank label=""a"" length=""10"" required=""false""></blank>" +
            @"</paragraph>" +
            @"</fib>" + Environment.NewLine;

        private static FibItem createFIBItemWithDisplayConditions()
        {
            var fibItem = new FibItem(new XmlElement(fibXmlWithoutConditions));

            IField variable = new Variable("var");
            var conditions = new Conditions(new Condition(variable, HybridOperator.List[HybridOperator.Ops.equals], new Expression("foo")));
            fibItem.DisplayConditions = conditions;

            return fibItem;
        }

        private static void removeDisplayConditions(IFormItem formItem)
        {
            formItem.DisplayConditions = new Conditions();
        }

        [Test]
        public void FibItemWithDisplayConditionsGeneratesExpectedXml()
        {
            IForm form = Project.Current.AddForm();
            FibItem fibItem = createFIBItemWithDisplayConditions();
            form.ItemList.Add(fibItem);

            Assert.IsTrue(fibItem.HasDisplayConditions);
            Assert.AreEqual(fibXmlWithConditions, fibItem.ToXml("Q1"));
        }

        [Test]
        public void FibItemWithDisplayConditionsIsConstructedFromXml()
        {
            var fibItem = new FibItem(new XmlElement(fibXmlWithConditions));

            Assert.IsTrue(fibItem.HasDisplayConditions);

            Conditions conditions = fibItem.DisplayConditions;
            Assert.AreEqual("var equals \"foo\"", conditions.ToString());
        }

        [Test]
        public void FibItemWithDisplayConditionsRemovedGeneratesExpectedXml()
        {
            IForm form = Project.Current.AddForm();
            FibItem fibItem = createFIBItemWithDisplayConditions();
            form.ItemList.Add(fibItem);

            removeDisplayConditions(fibItem);

            Assert.IsFalse(fibItem.HasDisplayConditions);
            Assert.AreEqual(fibXmlWithoutConditions, fibItem.ToXml("Q1"));
        }
    }
}