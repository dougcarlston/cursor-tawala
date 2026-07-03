// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.RtfSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FormItems
{
    /// <summary>
    /// Acceptance tests for story 1361 (Fields in FIBs).
    /// </summary>
    [TestFixture]
    public class Story1361_FieldsInFIBs
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            form = Project.Current.AddForm();
            referenceItem = new FibItem();
            form.ItemList.Add(referenceItem);
        }

        #endregion

        private const string defaultFibStyleAtttribute = " style=\"topLabels\"";

        private IForm form;
        private FibItem referenceItem;

        private readonly string rtfFieldString1 =
            RtfDocument.RtfStringPrefix +
            @"\deftab720\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
            @"\itap0\plain\f0\fs20" +
            @" FIB with field " +
            @"\plain\f0\fs20{\*\txfieldstart\txfieldtype0\txfieldflags219";

        private string rtfFieldString2 =
            @"\txfielddataval{0}";

        private readonly string rtfFieldString3 =
            @"\txfielddata " + RtfUtility.EncodeHexString("TF$Form 1:Q1:a") + "}" +
            @"<<Form 1:Q1:a>>{" +
            @"\*\txfieldend}\plain\f0\fs20" +
            @"  __________" +
            @"\par }";

        [Test]
        public void RtfToRtf()
        {
            string rtfString =
                rtfFieldString1 +
                string.Format(rtfFieldString2, referenceItem.BlankList[0].Id) +
                rtfFieldString3;

            var fibItem = new FibItem();
            fibItem.Rtf = rtfString;

            string expectedRtf =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
                @"{\fonttbl" + Environment.NewLine +
                @"{\f0\fswiss Arial;}" + Environment.NewLine +
                @"}" + Environment.NewLine +
                @"\fs20" +
                @"{\colortbl;" + Environment.NewLine +
                @"\red0\green0\blue0;" + Environment.NewLine +
                @"\red255\green255\blue255;" + Environment.NewLine +
                @"}" + Environment.NewLine +
                RtfConstants.DefaultTabsRtf +
                @"\pard " +
                @"{\f0\fs20\cf1 FIB with field }" +
                @"{\f0\fs20\cf1 {\*\txfieldstart\txfieldtype0\txfieldflags219" +
                string.Format(rtfFieldString2, referenceItem.BlankList[0].Id) +
                @"\txfielddata " + RtfUtility.EncodeHexString("TF$Form 1:Q1:a") + "}" +
                @"<<Form 1:Q1:a>>{" +
                @"\*\txfieldend}}" +
                @"{\f0\fs20\cf1  }__________\par }";

            Assert.AreEqual(expectedRtf, fibItem.ToRtf());
        }

        [Test]
        public void RtfToXml()
        {
            string rtfString =
                rtfFieldString1 +
                string.Format(rtfFieldString2, referenceItem.BlankList[0].Id) +
                rtfFieldString3;

            var fibItem = new FibItem();
            fibItem.Rtf = rtfString;

            string expectedXml =
                "<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                XmlConstants.FullBeginFont +
                "FIB with field " +
                XmlConstants.EndFont +
                XmlConstants.FullBeginFont +
                "<field name=\"Form 1:Q1:a\"/>" +
                XmlConstants.EndFont +
                XmlConstants.FullBeginFont +
                " " +
                XmlConstants.EndFont +
                "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
                "</paragraph>" +
                "</fib>\r\n";

            Assert.AreEqual(expectedXml, fibItem.ToXml("Q1"));
        }
    }
}