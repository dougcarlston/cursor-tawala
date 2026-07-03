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
    public class ConditionalDisplayOfHeadingItems3013
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
        }

        #endregion

        private static readonly string headingItemXmlWithConditions =
            @"<heading label=""H1"" type=""Main"">" +
            @"<paragraph indent=""0"" align=""left"">" +
            @"<tabPositions><tabStop position=""2880""/></tabPositions>" +
            @"<font face=""Arial"" size=""360"" color=""000000"">Heading</font>" +
            @"</paragraph>" +
            @"<displayConditions>" + Environment.NewLine +
            @"<equals field=""var"">" + Environment.NewLine +
            @"<string value=""foo""/>" + Environment.NewLine +
            @"</equals>" + Environment.NewLine +
            @"</displayConditions>" +
            @"</heading>" + Environment.NewLine;

        private static readonly string headingItemXmlWithoutConditions =
            @"<heading label=""H1"" type=""Main"">" +
            @"<paragraph indent=""0"" align=""left"">" +
            @"<tabPositions><tabStop position=""2880""/></tabPositions>" +
            @"<font face=""Arial"" size=""360"" color=""000000"">Heading</font>" +
            @"</paragraph>" +
            @"</heading>" + Environment.NewLine;

        private static HeadingItem createHeadingItemItemWithDisplayConditions()
        {
            var headingItem = new HeadingItem(new XmlElement(headingItemXmlWithoutConditions));

            IField variable = new Variable("var");
            var conditions = new Conditions(new Condition(variable, HybridOperator.List[HybridOperator.Ops.equals], new Expression("foo")));
            headingItem.DisplayConditions = conditions;

            return headingItem;
        }

        private static void removeDisplayConditions(IFormItem formItem)
        {
            formItem.DisplayConditions = new Conditions();
        }

        [Test]
        public void HeadingItemWithDisplayConditionsGeneratesExpectedXml()
        {
            IForm form = Project.Current.AddForm();
            HeadingItem headingItem = createHeadingItemItemWithDisplayConditions();
            form.ItemList.Add(headingItem);

            Assert.IsTrue(headingItem.HasDisplayConditions);
            Assert.AreEqual(headingItemXmlWithConditions, headingItem.ToXml("H1"));
        }

        [Test]
        public void HeadingItemWithDisplayConditionsIsConstructedFromXml()
        {
            var headingItem = new HeadingItem(new XmlElement(headingItemXmlWithConditions));

            Assert.IsTrue(headingItem.HasDisplayConditions);

            Conditions conditions = headingItem.DisplayConditions;
            Assert.AreEqual("var equals \"foo\"", conditions.ToString());
        }

        [Test]
        public void HeadingItemWithDisplayConditionsRemovedGeneratesExpectedXml()
        {
            IForm form = Project.Current.AddForm();
            HeadingItem headingItem = createHeadingItemItemWithDisplayConditions();
            form.ItemList.Add(headingItem);

            removeDisplayConditions(headingItem);

            Assert.IsFalse(headingItem.HasDisplayConditions);
            Assert.AreEqual(headingItemXmlWithoutConditions, headingItem.ToXml("H1"));
        }
    }
}