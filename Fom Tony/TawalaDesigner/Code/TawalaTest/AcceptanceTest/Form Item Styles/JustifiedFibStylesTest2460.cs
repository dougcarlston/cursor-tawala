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
    public class JustifiedFibStylesTest2460
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
        private FibItem fibItem1;

        private void setupForms()
        {
            form1 = Project.Current.AddForm();

            fibItem1 = new FibItem();
            fibItem1.Text = "Fib Item 1 __________";
            form1.ItemList.Add(fibItem1);
        }

        [Test]
        public void FibWithLeftJustifiedStyleProducesXmlWithLeftJustifiedStyle()
        {
            fibItem1.Style = "leftAlignLabelsJustified";

            string expectedXml =
                @"<fib label=""Q1"" style=""leftAlignLabelsJustified"">" +
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
        public void FibWithRightJustifiedStyleProducesXmlWithRightJustifiedStyle()
        {
            fibItem1.Style = "rightAlignLabelsJustified";

            string expectedXml =
                @"<fib label=""Q1"" style=""rightAlignLabelsJustified"">" +
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
        public void XmlWithLeftJustifiedStyleProducesXmlWithLeftJustifiedStyle()
        {
            string xmlString =
                @"<fib label=""Q1"" style=""leftAlignLabelsJustified"">" +
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
        public void XmlWithRightJustifiedStyleProducesXmlWithRightJustifiedStyle()
        {
            string xmlString =
                @"<fib label=""Q1"" style=""rightAlignLabelsJustified"">" +
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