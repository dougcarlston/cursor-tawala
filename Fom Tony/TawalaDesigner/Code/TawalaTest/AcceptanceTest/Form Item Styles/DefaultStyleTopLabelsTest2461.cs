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
    public class DefaultStyleTopLabelsTest2461
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
        public void DefaultFibStyleIsTopLabels()
        {
            Assert.AreEqual("topLabels", fibItem1.Style);
        }

        [Test]
        public void LegacyFibItemsDoNotGetTopLabelsStyle()
        {
            string xmlString =
                @"<fib label=""Q1"">" +
                @"<paragraph indent=""0"" align=""left"">" +
                @"<tabPositions><tabStop position=""2880""/></tabPositions>" +
                XmlConstants.FullBeginFont +
                @"Fib Item 1 " +
                XmlConstants.EndFont +
                @"<blank label=""a"" length=""10"" required=""false""></blank>" +
                @"</paragraph>" +
                @"</fib>" + Environment.NewLine;

            var fibItem = new FibItem(new XmlElement(xmlString));

            Assert.AreEqual(xmlString, fibItem.ToXml("Q1"));
        }
    }
}