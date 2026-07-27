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
    public class ConditionalDisplayOfMCQs2990
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
        }

        #endregion

        private static readonly string mcqXmlWithConditions =
            @"<mc label=""Q1"" onlyone=""true"" required=""false""" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
            @"<question>" +
            @"<paragraph indent=""0"" align=""left"">" +
            @"<font face=""Arial"" size=""200"" color=""000000"">Favorite fruit:</font>" +
            @"</paragraph>" +
            @"</question>" +
            @"<choice label=""a"">" +
            @"<paragraph indent=""0"" align=""left"">" +
            @"<font face=""Arial"" size=""200"" color=""000000"">Apples</font>" +
            @"</paragraph>" +
            @"</choice>" +
            @"<choice label=""b"">" +
            @"<paragraph indent=""0"" align=""left"">" +
            @"<font face=""Arial"" size=""200"" color=""000000"">Oranges</font>" +
            @"</paragraph>" +
            @"</choice>" +
            @"<displayConditions>" + Environment.NewLine +
            @"<equals field=""var"">" + Environment.NewLine +
            @"<string value=""foo""/>" + Environment.NewLine +
            @"</equals>" + Environment.NewLine +
            @"</displayConditions>" +
            @"</mc>" + Environment.NewLine;

        private static readonly string mcqXmlWithoutConditions =
            @"<mc label=""Q1"" onlyone=""true"" required=""false""" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
            @"<question>" +
            @"<paragraph indent=""0"" align=""left"">" +
            @"<font face=""Arial"" size=""200"" color=""000000"">Favorite fruit:</font>" +
            @"</paragraph>" +
            @"</question>" +
            @"<choice label=""a"">" +
            @"<paragraph indent=""0"" align=""left"">" +
            @"<font face=""Arial"" size=""200"" color=""000000"">Apples</font>" +
            @"</paragraph>" +
            @"</choice>" +
            @"<choice label=""b"">" +
            @"<paragraph indent=""0"" align=""left"">" +
            @"<font face=""Arial"" size=""200"" color=""000000"">Oranges</font>" +
            @"</paragraph>" +
            @"</choice>" +
            @"</mc>" + Environment.NewLine;

        private static McqItem createMcqItemWithDisplayConditions()
        {
            var mcqItem = new McqItem(new XmlElement(mcqXmlWithoutConditions));

            IField variable = new Variable("var");
            var conditions = new Conditions(new Condition(variable, HybridOperator.List[HybridOperator.Ops.equals], new Expression("foo")));
            mcqItem.DisplayConditions = conditions;

            return mcqItem;
        }

        private static void removeDisplayConditions(IFormItem formItem)
        {
            formItem.DisplayConditions = new Conditions();
        }

        [Test]
        public void McqItemWithDisplayConditionsGeneratesExpectedXml()
        {
            IForm form = Project.Current.AddForm();
            McqItem mcqItem = createMcqItemWithDisplayConditions();
            form.ItemList.Add(mcqItem);

            Assert.IsTrue(mcqItem.HasDisplayConditions);
            Assert.AreEqual(mcqXmlWithConditions, mcqItem.ToXml("Q1"));
        }

        [Test]
        public void McqItemWithDisplayConditionsIsConstructedFromXml()
        {
            var mcqItem = new McqItem(new XmlElement(mcqXmlWithConditions));

            Assert.IsTrue(mcqItem.HasDisplayConditions);

            Conditions conditions = mcqItem.DisplayConditions;
            Assert.AreEqual("var equals \"foo\"", conditions.ToString());
        }

        [Test]
        public void McqItemWithDisplayConditionsRemovedGeneratesExpectedXml()
        {
            IForm form = Project.Current.AddForm();
            McqItem mcqItem = createMcqItemWithDisplayConditions();
            form.ItemList.Add(mcqItem);

            removeDisplayConditions(mcqItem);

            Assert.IsFalse(mcqItem.HasDisplayConditions);
            Assert.AreEqual(mcqXmlWithoutConditions, mcqItem.ToXml("Q1"));
        }
    }
}