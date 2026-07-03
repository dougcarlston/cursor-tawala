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
    public class MCQColumnCountTest2544
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
        private McqItem mcItem1;
        private McqItem mcItem2;
        private McqItem mcItem3;

        private void setupForms()
        {
            form1 = Project.Current.AddForm();
            form2 = Project.Current.AddForm();

            mcItem1 = new McqItem();
            mcItem1.Text = "MCQ1";
            mcItem1.Choices.Clear();
            mcItem1.Choices.Add(new Choice("Choice A"));
            mcItem1.Choices.Add(new Choice("Choice B"));
            form1.ItemList.Add(mcItem1);

            mcItem2 = new McqItem();
            mcItem2.Text = "MCQ2";
            mcItem2.Choices.Clear();
            mcItem2.Choices.Add(new Choice("Choice A"));
            mcItem2.Choices.Add(new Choice("Choice B"));
            form2.ItemList.Add(mcItem2);

            mcItem3 = new McqItem();
            mcItem3.Text = "MCQ3";
            mcItem3.Choices.Clear();
            mcItem3.Choices.Add(new Choice("Choice A"));
            mcItem3.Choices.Add(new Choice("Choice B"));
            form2.ItemList.Add(mcItem3);
        }

        [Test]
        public void MCQWithColumnCountProducesXmlWithColumnCount()
        {
            mcItem1.Style = "multicolumn";
            mcItem1.ColumnCount = 2;

            string expectedXml =
                @"<mc label=""Q1"" onlyone=""true"" required=""false"" style=""multicolumn"" columnCount=""2"">" +
                @"<question>" +
                @"<paragraph indent=""0"" align=""left"">" +
                XmlConstants.FullBeginFont +
                @"MCQ1" +
                XmlConstants.EndFont +
                @"</paragraph>" +
                @"</question>" +
                @"<choice label=""a"">" +
                @"<paragraph indent=""0"" align=""left"">" +
                XmlConstants.FullBeginFont +
                @"Choice A" +
                XmlConstants.EndFont +
                @"</paragraph>" +
                @"</choice>" +
                @"<choice label=""b"">" +
                @"<paragraph indent=""0"" align=""left"">" +
                XmlConstants.FullBeginFont +
                @"Choice B" +
                XmlConstants.EndFont +
                @"</paragraph>" +
                @"</choice>" +
                @"</mc>" +
                Environment.NewLine;

            Console.WriteLine(mcItem1.ToXml("Q1"));
            Assert.AreEqual(expectedXml, mcItem1.ToXml("Q1"));
        }

        [Test]
        public void MCQWithoutColumnCountProducesXmlWithoutColumnCount()
        {
            mcItem1.Style = "multicolumn";
            mcItem1.ColumnCount = 0;

            string expectedXml =
                @"<mc label=""Q1"" onlyone=""true"" required=""false"" style=""multicolumn"">" +
                @"<question>" +
                @"<paragraph indent=""0"" align=""left"">" +
                XmlConstants.FullBeginFont +
                @"MCQ1" +
                XmlConstants.EndFont +
                @"</paragraph>" +
                @"</question>" +
                @"<choice label=""a"">" +
                @"<paragraph indent=""0"" align=""left"">" +
                XmlConstants.FullBeginFont +
                @"Choice A" +
                XmlConstants.EndFont +
                @"</paragraph>" +
                @"</choice>" +
                @"<choice label=""b"">" +
                @"<paragraph indent=""0"" align=""left"">" +
                XmlConstants.FullBeginFont +
                @"Choice B" +
                XmlConstants.EndFont +
                @"</paragraph>" +
                @"</choice>" +
                @"</mc>" +
                Environment.NewLine;

            Console.WriteLine(mcItem1.ToXml("Q1"));
            Assert.AreEqual(expectedXml, mcItem1.ToXml("Q1"));
        }

        [Test]
        public void ProjectSetAllMCQStylesModifiesAllMCQs()
        {
            Project.Current.SetAllMCQStyles("multicolumn", 2, true);

            Assert.AreEqual(mcItem1.Style, "multicolumn");
            Assert.AreEqual(mcItem1.ColumnCount, 2);

            Assert.AreEqual(mcItem2.Style, "multicolumn");
            Assert.AreEqual(mcItem2.ColumnCount, 2);

            Assert.AreEqual(mcItem3.Style, "multicolumn");
            Assert.AreEqual(mcItem3.ColumnCount, 2);
        }

        [Test]
        public void XmlWithColumnCountProducesXmlWithColumnCount()
        {
            string xmlString =
                @"<mc label=""Q1"" onlyone=""true"" required=""false"" style=""multicolumn"" columnCount=""2"">" +
                @"<question>" +
                @"<paragraph indent=""0"" align=""left"">" +
                XmlConstants.BeginFont +
                @"MCQ1" +
                XmlConstants.EndFont +
                @"</paragraph>" +
                @"</question>" +
                @"<choice label=""a"">" +
                @"<paragraph indent=""0"" align=""left"">" +
                XmlConstants.BeginFont +
                @"Choice A" +
                XmlConstants.EndFont +
                @"</paragraph>" +
                @"</choice>" +
                @"<choice label=""b"">" +
                @"<paragraph indent=""0"" align=""left"">" +
                XmlConstants.BeginFont +
                @"Choice B" +
                XmlConstants.EndFont +
                @"</paragraph>" +
                @"</choice>" +
                @"</mc>" +
                Environment.NewLine;

            var mcItem = new McqItem(new XmlElement(xmlString));

            Assert.AreEqual(xmlString, mcItem.ToXml("Q1"));
        }

        [Test]
        public void XmlWithoutColumnCountProducesXmlWithoutColumnCount()
        {
            string xmlString =
                @"<mc label=""Q1"" onlyone=""true"" required=""false"" style=""multicolumn"">" +
                @"<question>" +
                @"<paragraph indent=""0"" align=""left"">" +
                XmlConstants.BeginFont +
                @"MCQ1" +
                XmlConstants.EndFont +
                @"</paragraph>" +
                @"</question>" +
                @"<choice label=""a"">" +
                @"<paragraph indent=""0"" align=""left"">" +
                XmlConstants.BeginFont +
                @"Choice A" +
                XmlConstants.EndFont +
                @"</paragraph>" +
                @"</choice>" +
                @"<choice label=""b"">" +
                @"<paragraph indent=""0"" align=""left"">" +
                XmlConstants.BeginFont +
                @"Choice B" +
                XmlConstants.EndFont +
                @"</paragraph>" +
                @"</choice>" +
                @"</mc>" +
                Environment.NewLine;

            var mcItem = new McqItem(new XmlElement(xmlString));

            Assert.AreEqual(xmlString, mcItem.ToXml("Q1"));
        }
    }
}