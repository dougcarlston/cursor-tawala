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
    public class DefaultGlobalStyleSelectionTest2478
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
        private McqItem mcItem1;
        private TextItem textItem1;

        private void setupForms()
        {
            form1 = Project.Current.AddForm();

            fibItem1 = new FibItem();
            fibItem1.Text = "Fib Item 1 __________";
            form1.ItemList.Add(fibItem1);

            mcItem1 = new McqItem();
            mcItem1.Text = "MCQ1";
            mcItem1.Choices.Clear();
            mcItem1.Choices.Add(new Choice("Choice A"));
            mcItem1.Choices.Add(new Choice("Choice B"));
            form1.ItemList.Add(mcItem1);

            textItem1 = new TextItem();
            textItem1.Text = "Text Item 1";
            form1.ItemList.Add(textItem1);
        }

        [Test]
        public void FibItemsHaveStyleByDefault()
        {
            Assert.AreEqual(fibItem1.Style, "topLabels");
        }

        [Test]
        public void GlobalStylesAreContainedInProjectXml()
        {
            Project.Current.SetAllFibStyles("leftAlignLabels");
            Project.Current.SetAllMCQStyles("horizontal", true);
            Project.Current.SetAllTextItemStyles("heading", true);

            string expectedXml =
                @"<pageHeader></pageHeader>" +
                @"<styles fibItemStyle=""leftAlignLabels"" mcItemStyle=""horizontal"" textItemStyle=""heading"" />" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" blockBackButton=""false"">" + Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1"" style=""leftAlignLabels""><paragraph indent=""0"" align=""left"">" +
                @"<font face=""Arial"" size=""180"" color=""000000"">" +
                @"Fib Item 1 " +
                XmlConstants.EndFont +
                @"<blank label=""a"" length=""10"" " +
                @"required=""false""></blank></paragraph></fib>" + Environment.NewLine +
                @"<mc label=""Q2"" onlyone=""true"" required=""false"" style=""horizontal""><question><paragraph indent=""0"" " +
                @"align=""left"">" +
                XmlConstants.FullBeginFont +
                @"MCQ1" +
                XmlConstants.EndFont +
                @"</paragraph></question><choice label=""a""><paragraph indent=""0"" " +
                @"align=""left"">" +
                XmlConstants.FullBeginFont +
                @"Choice A" +
                XmlConstants.EndFont +
                @"</paragraph></choice><choice label=""b""><paragraph indent=""0"" align=""left"">" +
                XmlConstants.FullBeginFont +
                @"Choice B" +
                XmlConstants.EndFont +
                @"</paragraph></choice></mc>" + Environment.NewLine +
                @"<text label=""T1"" style=""heading""><paragraph indent=""0"" align=""left"">Text Item 1</paragraph></text>" +
                Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Assert.AreEqual(expectedXml, Util.StripProjectHeader(Project.Current.ToXml()));
        }

        [Test]
        public void GlobalStylesAreRestoredFromProjectXml()
        {
            string xmlString =
                @"<project name=""Untitled"" themePath=""default"" format=""1.6"">" + Environment.NewLine +
                @"<styles fibItemStyle=""leftAlignLabels"" mcItemStyle=""horizontal"" textItemStyle=""heading"" />" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"">" + Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1"" style=""leftAlignLabels""><paragraph indent=""0"" align=""left""><font face=""Arial"" size=""180"" " +
                @"color=""000000"">Fib Item 1 </font><blank label=""a"" length=""10"" " +
                @"required=""false""></blank></paragraph></fib>" + Environment.NewLine +
                @"<mc label=""Q2"" onlyone=""true"" required=""false"" style=""horizontal""><question><paragraph indent=""0"" " +
                @"align=""left""><font face=""Arial"" size=""200"" " +
                @"color=""000000"">MCQ1</font></paragraph></question><choice label=""a""><paragraph indent=""0"" " +
                @"align=""left""><font face=""Arial"" size=""200"" color=""000000"">Choice " +
                @"A</font></paragraph></choice><choice label=""b""><paragraph indent=""0"" align=""left""><font " +
                @"face=""Arial"" size=""200"" color=""000000"">Choice B</font></paragraph></choice></mc>" + Environment.NewLine +
                @"<text label=""T1"" style=""heading""><paragraph indent=""0"" align=""left"">Text Item 1</paragraph></text>" +
                Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            IXmlElement element = new XmlElement(xmlString);
            Project.New(element);

            Assert.AreEqual("leftAlignLabels", Project.Current.GlobalFibItemStyle);
            Assert.AreEqual("horizontal", Project.Current.GlobalMCItemStyle);
            Assert.AreEqual("heading", Project.Current.GlobalTextItemStyle);
        }

        [Test]
        public void McqItemsHaveVerticalStyleByDefault()
        {
            Assert.AreEqual("vertical", mcItem1.Style);
        }

        [Test]
        public void NewFibItemHasDefaultGlobalStyle()
        {
            Project.Current.SetAllFibStyles("leftAlignLabels");

            var newFibItem = new FibItem();

            Assert.AreEqual("leftAlignLabels", newFibItem.Style);
        }

        [Test]
        public void NewMCItemHasDefaultGlobalStyle()
        {
            Project.Current.SetAllMCQStyles("horizontal", true);

            var newMCItem = new McqItem();

            Assert.AreEqual("horizontal", newMCItem.Style);
        }

        [Test]
        public void NewTextItemHasDefaultGlobalStyle()
        {
            Project.Current.SetAllTextItemStyles("heading", true);

            var newTextItem = new TextItem();

            Assert.AreEqual("heading", newTextItem.Style);
        }

        [Test]
        public void TextItemsHaveNormalStyleByDefault()
        {
            Assert.AreEqual("normal", textItem1.Style);
        }
    }
}