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
    public class ConditionalDisplayOfTextItems3000
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
        }

        #endregion

        private static readonly string textItemXmlWithConditions =
            //@"<text label=""T1"">" +
            @"<text label=""T1""" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
            @"<paragraph indent=""0"" align=""left"">" +
            @"<tabPositions><tabStop position=""2880""/></tabPositions>" +
            @"<font>Text</font>" +
            @"</paragraph>" +
            @"<displayConditions>" + Environment.NewLine +
            @"<equals field=""var"">" + Environment.NewLine +
            @"<string value=""foo""/>" + Environment.NewLine +
            @"</equals>" + Environment.NewLine +
            @"</displayConditions>" +
            @"</text>" + Environment.NewLine;

        private static readonly string textItemXmlWithoutConditions =
            @"<text label=""T1""" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
            @"<paragraph indent=""0"" align=""left"">" +
            @"<tabPositions><tabStop position=""2880""/></tabPositions>" +
            @"<font>Text</font>" +
            @"</paragraph>" +
            @"</text>" + Environment.NewLine;

        private static TextItem createTextItemItemWithDisplayConditions()
        {
            var textItem = new TextItem(new XmlElement(textItemXmlWithoutConditions));

            IField variable = new Variable("var");
            var conditions = new Conditions(new Condition(variable, HybridOperator.List[HybridOperator.Ops.equals], new Expression("foo")));
            textItem.DisplayConditions = conditions;

            return textItem;
        }

        private static void removeDisplayConditions(IFormItem formItem)
        {
            formItem.DisplayConditions = new Conditions();
        }

        [Test]
        public void TextItemWithDisplayConditionsGeneratesExpectedXml()
        {
            IForm form = Project.Current.AddForm();
            TextItem textItem = createTextItemItemWithDisplayConditions();
            form.ItemList.Add(textItem);

            Assert.IsTrue(textItem.HasDisplayConditions);
            Assert.AreEqual(textItemXmlWithConditions, textItem.ToXml("T1"));
        }

        [Test]
        public void TextItemWithDisplayConditionsIsConstructedFromXml()
        {
            var textItem = new TextItem(new XmlElement(textItemXmlWithConditions));

            Assert.IsTrue(textItem.HasDisplayConditions);

            Conditions conditions = textItem.DisplayConditions;
            Assert.AreEqual("var equals \"foo\"", conditions.ToString());
        }

        [Test]
        public void TextItemWithDisplayConditionsRemovedGeneratesExpectedXml()
        {
            IForm form = Project.Current.AddForm();
            TextItem textItem = createTextItemItemWithDisplayConditions();
            form.ItemList.Add(textItem);

            removeDisplayConditions(textItem);

            Assert.IsFalse(textItem.HasDisplayConditions);
            Assert.AreEqual(textItemXmlWithoutConditions, textItem.ToXml("T1"));
        }
    }
}