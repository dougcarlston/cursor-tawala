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
    public class GlobalFibStylesTest2272
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
        private FibItem fibItem1;
        private FibItem fibItem2;

        private void setupForms()
        {
            form1 = Project.Current.AddForm();
            form2 = Project.Current.AddForm();

            fibItem1 = new FibItem();
            fibItem1.Text = "Fib Item 1 __________";
            form1.ItemList.Add(fibItem1);

            fibItem2 = new FibItem();
            fibItem2.Text = "Fib Item 2 __________";
            form2.ItemList.Add(fibItem2);
        }

        [Test]
        public void FibWithStyleProducesXmlWithStyle()
        {
            fibItem1.Style = "freeform";

            string expectedXml =
                @"<fib label=""Q1"" style=""freeform"">" +
                @"<paragraph indent=""0"" align=""left"">" +
                @"<font face=""Arial"" size=""180"" color=""000000"">" +
                @"Fib Item 1 " +
                XmlConstants.EndFont +
                @"<blank label=""a"" length=""10"" required=""false""></blank>" +
                @"</paragraph>" +
                @"</fib>" +
                Environment.NewLine;

            Assert.AreEqual(expectedXml, fibItem1.ToXml("Q1"));
        }

        [Test]
        public void ProjectSetAllFibStylesModifiesAllFibs()
        {
            Project.Current.SetAllFibStyles("leftAlignLabels");

            Assert.AreEqual(fibItem1.Style, "leftAlignLabels");
            Assert.AreEqual(fibItem2.Style, "leftAlignLabels");
        }

        [Test]
        public void XmlWithoutStyleProducesXmlWithoutStyle()
        {
            string xmlString =
                @"<fib label=""Q1"">" +
                @"<paragraph indent=""0"" align=""left"">" +
                XmlConstants.FaceColorBeginFont +
                @"Fib Item 1 " +
                XmlConstants.EndFont +
                @"<blank label=""a"" length=""10"" required=""false""></blank>" +
                @"</paragraph>" +
                @"</fib>" +
                Environment.NewLine;

            var fibItem = new FibItem(new XmlElement(xmlString));

            Assert.AreEqual(xmlString, fibItem.ToXml("Q1"));
        }

        [Test]
        public void XmlWithStyleProducesXmlWithStyle()
        {
            string xmlString =
                @"<fib label=""Q1"" style=""freeform"">" +
                @"<paragraph indent=""0"" align=""left"">" +
                XmlConstants.FaceColorBeginFont +
                @"Fib Item 1 " +
                XmlConstants.EndFont +
                @"<blank label=""a"" length=""10"" required=""false""></blank>" +
                @"</paragraph>" +
                @"</fib>" +
                Environment.NewLine;

            var fibItem = new FibItem(new XmlElement(xmlString));

            Assert.AreEqual(xmlString, fibItem.ToXml("Q1"));
        }
    }
}