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
    public class GlobalMCQStylesTest2274
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
        }

        [Test]
        public void MCQWithStyleProducesXmlWithStyle()
        {
            mcItem1.Style = "vertical";

            string expectedXml =
                @"<mc label=""Q1"" onlyone=""true"" required=""false"" style=""vertical"">" +
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
            Project.Current.SetAllMCQStyles("vertical", true);

            Assert.AreEqual(mcItem1.Style, "vertical");
            Assert.AreEqual(mcItem2.Style, "vertical");
        }

        [Test]
        public void XmlWithoutStyleProducesXmlWithoutStyle()
        {
            string xmlString =
                @"<mc label=""Q1"" onlyone=""true"" required=""false""" + XmlConstants.DefaultMcqItemStyleAttribute + ">" +
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
        public void XmlWithStyleProducesXmlWithStyle()
        {
            string xmlString =
                @"<mc label=""Q1"" onlyone=""true"" required=""false"" style=""vertical"">" +
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