// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FormItemStyles
{
    [TestFixture]
    public class DesignerControlsPaddingOfMcqItem3030
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
        }

        #endregion

        private readonly string xmlStringWithNoPaddingAttribute =
            @"<mc label=""Q1"" onlyone=""true"" required=""false"" style=""vertical"">" +
            @"<question>" +
            @"<paragraph indent=""0"" align=""left"">" +
            XmlConstants.DefaultTabsXml +
            XmlConstants.FullBeginFont + @"MCQ 1" + XmlConstants.EndFont +
            @"</paragraph>" +
            @"</question>" +
            @"<choice label=""a"">" +
            @"<paragraph indent=""0"" align=""left"">" +
            XmlConstants.DefaultTabsXml +
            XmlConstants.FullBeginFont + @"Choice" + XmlConstants.EndFont +
            @"</paragraph>" +
            @"</choice>" +
            @"</mc>" + Environment.NewLine;

        private readonly string xmlStringWithFalsePaddingAttribute =
            @"<mc label=""Q1"" onlyone=""true"" required=""false"" style=""vertical"" paddingBottom=""false"">" +
            @"<question>" +
            @"<paragraph indent=""0"" align=""left"">" +
            XmlConstants.DefaultTabsXml +
            XmlConstants.FullBeginFont + @"MCQ 1" + XmlConstants.EndFont +
            @"</paragraph>" +
            @"</question>" +
            @"<choice label=""a"">" +
            @"<paragraph indent=""0"" align=""left"">" +
            XmlConstants.DefaultTabsXml +
            XmlConstants.FullBeginFont + @"Choice" + XmlConstants.EndFont +
            @"</paragraph>" +
            @"</choice>" +
            @"</mc>" + Environment.NewLine;

        private IMcqItem mcqItem1;
        private IMcqItem mcqItem2;
        private IMcqItem mcqItem3;

        private void setupForms()
        {
            IForm form1 = Project.Current.AddForm();
            IForm form2 = Project.Current.AddForm();

            mcqItem1 = new McqItem();
            form1.ItemList.Add(mcqItem1);

            mcqItem2 = new McqItem();
            form2.ItemList.Add(mcqItem2);

            mcqItem3 = new McqItem();
            form2.ItemList.Add(mcqItem3);
        }

        [Test]
        public void ProjectSetAllMcqMulticolumnStylesModifiesPaddingInAllTextItems()
        {
            setupForms();

            Project.Current.SetAllMCQStyles("multicolumn", 2, false);

            Assert.IsFalse(mcqItem1.PaddingBottom);
            Assert.IsFalse(mcqItem2.PaddingBottom);
            Assert.IsFalse(mcqItem3.PaddingBottom);
        }

        [Test]
        public void ProjectSetAllMcqStylesModifiesPaddingInAllTextItems()
        {
            setupForms();

            Project.Current.SetAllMCQStyles("vertical", false);

            Assert.IsFalse(mcqItem1.PaddingBottom);
            Assert.IsFalse(mcqItem2.PaddingBottom);
            Assert.IsFalse(mcqItem3.PaddingBottom);
        }

        [Test]
        public void SettingPaddingPropertyToFalseProducesXmlWithFalsePaddingAttribute()
        {
            IMcqItem mcqItem = new McqItem(new XmlElement(xmlStringWithNoPaddingAttribute));

            mcqItem.PaddingBottom = false;

            Assert.AreEqual(xmlStringWithFalsePaddingAttribute, mcqItem.ToXml("Q1"));
        }

        [Test]
        public void SettingPaddingPropertyToTrueProducesXmlWithNoPaddingAttribute()
        {
            IMcqItem mcqItem = new McqItem(new XmlElement(xmlStringWithFalsePaddingAttribute));

            mcqItem.PaddingBottom = true;

            Assert.AreEqual(xmlStringWithNoPaddingAttribute, mcqItem.ToXml("Q1"));
        }

        [Test]
        public void XmlWithFalsePaddingProducesXmlWithFalsePaddingAttribute()
        {
            var mcqItem = new McqItem(new XmlElement(xmlStringWithFalsePaddingAttribute));

            Assert.AreEqual(xmlStringWithFalsePaddingAttribute, mcqItem.ToXml("Q1"));
        }

        [Test]
        public void XmlWithNoPaddingAttributeProducesXmlWithNoPaddingAttribute()
        {
            var mcqItem = new McqItem(new XmlElement(xmlStringWithNoPaddingAttribute));

            Assert.AreEqual(xmlStringWithNoPaddingAttribute, mcqItem.ToXml("Q1"));
        }
    }
}