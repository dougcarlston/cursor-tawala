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
    public class DesignerControlsPaddingOfTextItem3027
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
        }

        #endregion

        private readonly string xmlStringWithNoPaddingAttribute =
            @"<text label=""T1"" style=""normal"">" +
            @"<paragraph indent=""0"" align=""left"">" +
            XmlConstants.DefaultTabsXml +
            XmlConstants.DefaultBeginFont +
            @"[Replace this with text of your own.]" +
            XmlConstants.EndFont +
            @"</paragraph>" +
            @"</text>" +
            Environment.NewLine;

        private readonly string xmlStringWithFalsePaddingAttribute =
            @"<text label=""T1"" style=""normal"" paddingBottom=""false"">" +
            @"<paragraph indent=""0"" align=""left"">" +
            XmlConstants.DefaultTabsXml +
            XmlConstants.DefaultBeginFont +
            @"[Replace this with text of your own.]" +
            XmlConstants.EndFont +
            @"</paragraph>" +
            @"</text>" +
            Environment.NewLine;

        private ITextItem textItem1;
        private ITextItem textItem2;
        private ITextItem textItem3;

        private void setupForms()
        {
            IForm form1 = Project.Current.AddForm();
            IForm form2 = Project.Current.AddForm();

            textItem1 = new TextItem();
            form1.ItemList.Add(textItem1);

            textItem2 = new TextItem();
            form2.ItemList.Add(textItem2);

            textItem3 = new TextItem();
            form2.ItemList.Add(textItem3);
        }

        [Test]
        public void ProjectSetAllTextItemStylesModifiesPaddingInAllTextItems()
        {
            setupForms();

            Project.Current.SetAllTextItemStyles("instructional", false);

            Assert.IsFalse(textItem1.PaddingBottom);
            Assert.IsFalse(textItem2.PaddingBottom);
            Assert.IsFalse(textItem3.PaddingBottom);
        }

        [Test]
        public void SettingPaddingPropertyToFalseProducesXmlWithFalsePaddingAttribute()
        {
            ITextItem textItem = new TextItem();

            textItem.PaddingBottom = false;

            Assert.AreEqual(xmlStringWithFalsePaddingAttribute, textItem.ToXml("T1"));
        }

        [Test]
        public void SettingPaddingPropertyToTrueProducesXmlWithNoPaddingAttribute()
        {
            ITextItem textItem = new TextItem();

            textItem.PaddingBottom = true;

            Assert.AreEqual(xmlStringWithNoPaddingAttribute, textItem.ToXml("T1"));
        }

        [Test]
        public void XmlWithFalsePaddingProducesXmlWithFalsePaddingAttribute()
        {
            var textItem = new TextItem(new XmlElement(xmlStringWithFalsePaddingAttribute));

            Assert.AreEqual(xmlStringWithFalsePaddingAttribute, textItem.ToXml("T1"));
        }

        [Test]
        public void XmlWithNoPaddingAttributeProducesXmlWithNoPaddingAttribute()
        {
            var textItem = new TextItem(new XmlElement(xmlStringWithNoPaddingAttribute));

            Assert.AreEqual(xmlStringWithNoPaddingAttribute, textItem.ToXml("T1"));
        }
    }
}