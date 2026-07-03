// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FormItemStyles
{
    [TestFixture]
    public class HeadingsTest2346
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            form = Project.Current.AddForm();

            fibItem = new FibItem();
            hiddenField = new HiddenField();
            mcItem = new McqItem();
            textItem1 = new TextItem();
            textItem2 = new TextItem();
            heading1 = new HeadingItem();
            heading2 = new HeadingItem();
            heading2.HeadingType = HeadingType.Sub;

            form.ItemList.Add(hiddenField);
            form.ItemList.Add(heading1);
            form.ItemList.Add(fibItem);
            form.ItemList.Add(textItem1);
            form.ItemList.Add(textItem2);
            form.ItemList.Add(heading2);
            form.ItemList.Add(mcItem);
        }

        [TearDown]
        public void Teardown()
        {
        }

        #endregion

        private IForm form;

        private HiddenField hiddenField;
        private TextItem textItem1;
        private TextItem textItem2;
        private FibItem fibItem;
        private McqItem mcItem;
        private HeadingItem heading1;
        private HeadingItem heading2;

        private static readonly string rtfPrologue =
            "{\\rtf1\\ansi\\ansicpg1252\\uc1\\deff0{\\fonttbl" + Environment.NewLine +
            "{\\f0\\fswiss\\fcharset0\\fprq2 Arial;}" + Environment.NewLine +
            "{\\f1\\froman\\fcharset2\\fprq2 Symbol;}}" + Environment.NewLine +
            "{\\colortbl;\\red0\\green0\\blue0;\\red255\\green255\\blue255;\\red0\\green0\\blue0;}" + Environment.NewLine +
            "{\\stylesheet{\\s0\\itap0\\f0\\fs24 [Normal];}{\\*\\cs10\\additive Default Paragraph Font;}}" + Environment.NewLine +
            "{\\*\\generator TX_RTF32 12.0.500.502;}" + Environment.NewLine +
            "\\deftab1134\\paperw12240\\paperh15840\\margl720\\margt720\\margr720\\margb720\\notabind\\pard\\itap0" +
            "\\tx2880\\plain\\f0\\fs20\\cf3 ";

        private static readonly string rtfWithFieldString =
            rtfPrologue +
            "[Replace this with text of your own.]" +
            "\\plain\\f0\\fs20\\cf3{\\*\\txfieldstart\\txfieldtype0\\txfieldflags219\\txfielddataval1100\\txfielddata " +
            "54004600240046006f0072006d00200031003a00510031003a0061000000}<<Form 1:Q1:a>>{\\*\\txfieldend}\\par }";

        private static readonly string expectedRtf =
            RtfConstants.BasicRtfThemePrologue +
            "\\pard \\tx2880" +
            "{\\f0\\fs20\\cf1 [Replace this with text of your " +
            "own.]}{\\f0\\fs20\\cf1 " +
            "{\\*\\txfieldstart\\txfieldtype0\\txfieldflags219\\txfielddataval2011\\txfielddata " +
            "54004600240046006f0072006d00200031003a00510031003a0061000000}<<Form 1:Q1:a>>{\\*\\txfieldend}}\\par }";

        [Test]
        public void AlternateLabels()
        {
            heading1.AlternateLabel = "My Heading";
            heading2.AlternateLabel = "Your Heading";

            Assert.AreEqual("My Heading", heading1.AlternateLabel);
            Assert.AreEqual("Your Heading", heading2.AlternateLabel);
        }

        [Test]
        public void DefaultLabels()
        {
            Assert.AreEqual("H1", form.ItemList.GetDefaultLabel(heading1));
            Assert.AreEqual("H2", form.ItemList.GetDefaultLabel(heading2));
        }

        [Test]
        public void Defaults()
        {
            Assert.AreEqual(HeadingType.Main, heading1.HeadingType);
            Assert.AreEqual(HeadingType.Sub, heading2.HeadingType);
            Assert.AreEqual(string.Empty, heading1.AlternateLabel);
            Assert.AreEqual(string.Empty, heading2.AlternateLabel);
        }

        [Test]
        public void HeadingsAreSkipToDestinations()
        {
            Assert.AreEqual(7, form.SkipToDestinations.Count);

            SkipToDestinationItem skipToDestH1 = form.SkipToDestinations[0];
            SkipToDestinationItem skipToDestH2 = form.SkipToDestinations[4];
            Assert.AreEqual("H1", skipToDestH1.ToString());
            Assert.AreEqual("H2", skipToDestH2.ToString());
        }

        [Test]
        public void HeadingTypeInXml()
        {
            Assert.IsTrue(heading1.ToXml("H1").StartsWith("<heading label=\"H1\" type=\"Main\">"));
            Assert.IsTrue(heading2.ToXml("H2").StartsWith("<heading label=\"H2\" type=\"Sub\">"));
        }

        [Test]
        public void OtherDefaultLabelsAreCorrect()
        {
            Assert.AreEqual("Q1", form.ItemList.GetDefaultLabel(fibItem));
            Assert.AreEqual("T1", form.ItemList.GetDefaultLabel(textItem1));
            Assert.AreEqual("T2", form.ItemList.GetDefaultLabel(textItem2));
            Assert.AreEqual("Q2", form.ItemList.GetDefaultLabel(mcItem));
        }

        [Test]
        public void RoundTripThroughProjectXml()
        {
            {
                Util.NewTestProject();
                form = Project.Current.AddForm();
                var heading = new HeadingItem();
                form.ItemList.Add(heading);
                var fib = new FibItem();
                form.ItemList.Add(fib);
                heading.AlternateLabel = "MyHeading";
                heading.HeadingType = HeadingType.Sub;
                heading.Rtf = rtfWithFieldString;
                Util.SaveAndReloadCurrentProject();
            }

            Assert.IsNotNull(Project.Current.FormList[0]);
            Assert.IsNotNull(Project.Current.FormList[0].ItemList[0]);
            Assert.IsTrue(Project.Current.FormList[0].ItemList[0] is IHeadingItem);
            Assert.IsTrue(Project.Current.FormList[0].ItemList[1] is FibItem);

            var loadedHeading = Project.Current.FormList[0].ItemList[0] as HeadingItem;
            Assert.AreEqual("MyHeading", loadedHeading.AlternateLabel);
            Assert.AreEqual(HeadingType.Sub, loadedHeading.HeadingType);
            string rtf = loadedHeading.Rtf;
            Assert.AreEqual(expectedRtf, rtf);
        }

        [Test]
        public void RtfToXml()
        {
            string rtfString =
                rtfPrologue +
                "Hello World!\\par }";

            heading1.Rtf = rtfString;

            string xmlString =
                "<heading label=\"H1\" type=\"Main\">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.DefaultTabsXml +
                XmlConstants.FullBeginFont +
                "Hello World!" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "</heading>\r\n";

            Assert.AreEqual(xmlString, heading1.ToXml("H1"));
        }

        [Test]
        public void RtfWithFieldToXml()
        {
            heading1.Rtf = rtfWithFieldString;

            string xmlString =
                "<heading label=\"H1\" type=\"Main\">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.DefaultTabsXml +
                XmlConstants.FullBeginFont +
                "[Replace this with text of your own.]" +
                XmlConstants.EndFont +
                XmlConstants.FullBeginFont +
                "<field name=\"Form 1:Q1:a\"/>" +
                XmlConstants.EndFont +
                "</paragraph>" +
                "</heading>\r\n";

            Assert.AreEqual(xmlString, heading1.ToXml("H1"));
        }
    }
}