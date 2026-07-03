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
    public class GlobalTextStylesTest2276
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            setupForms();
        }

        #endregion

        private IForm form1;
        private IForm form2;
        private TextItem textItem1;
        private TextItem textItem2;

        private void setupForms()
        {
            form1 = Project.Current.AddForm();
            form2 = Project.Current.AddForm();

            textItem1 = new TextItem();
            textItem1.Text = "Text Item 1";
            form1.ItemList.Add(textItem1);

            textItem2 = new TextItem();
            textItem2.Text = "Text Item 2";
            form1.ItemList.Add(textItem2);
        }

        [Test]
        public void ProjectSetAllTextItemStylesModifiesAllTextItems()
        {
            Project.Current.SetAllTextItemStyles("normal", true);

            Assert.AreEqual(textItem1.Style, "normal");
            Assert.AreEqual(textItem2.Style, "normal");
        }

        [Test]
        public void TextItemWithStyleProducesXmlWithStyle()
        {
            textItem1.Style = "vertical";

            string expectedXml =
                @"<text label=""T1"" style=""vertical"">" +
                @"<paragraph indent=""0"" align=""left"">" +
                @"Text Item 1" +
                @"</paragraph>" +
                @"</text>" +
                Environment.NewLine;

            Console.WriteLine(textItem1.ToXml("T1"));
            Assert.AreEqual(expectedXml, textItem1.ToXml("T1"));
        }

        [Test]
        public void XmlWithoutStyleProducesXmlWithoutStyle()
        {
            string xmlString =
                @"<text label=""T1""" + XmlConstants.DefaultTextItemStyleAttribute + @">" +
                @"<paragraph indent=""0"" align=""left""><tabPositions>" +
                @"<tabStop position=""2880""/>" +
                @"</tabPositions>" +
                XmlConstants.BeginFont +
                @"Text Item 1" +
                XmlConstants.EndFont +
                @"</paragraph>" +
                @"</text>" +
                Environment.NewLine;

            var mcItem = new TextItem(new XmlElement(xmlString));

            Assert.AreEqual(xmlString, mcItem.ToXml("T1"));
        }

        [Test]
        public void XmlWithStyleProducesXmlWithStyle()
        {
            string xmlString =
                @"<text label=""T1"" style=""vertical"">" +
                @"<paragraph indent=""0"" align=""left"">" +
                @"Text Item 1" +
                @"</paragraph>" +
                @"</text>" +
                Environment.NewLine;

            var textItem = new TextItem(new XmlElement(xmlString));

            Assert.AreEqual(xmlString, textItem.ToXml("T1"));
        }
    }
}