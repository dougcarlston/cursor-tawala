// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Xml;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;
using XmlElement=Tawala.XmlSupport.XmlElement;

namespace TawalaTest.BugTest
{
    [TestFixture]
    public class InvitationFieldXml394
    {
        private static IForm setupProjectWithForm()
        {
            Util.NewTestProject();
            IForm form = Project.Current.AddForm();

            return form;
        }

        [Test]
        public void EscapedXmlCharactersProduceEscapedXmlCharacters()
        {
            Util.NewTestProject();

            string xmlString =
                "<invitation form=\"Form 1\" project=\"\"><displayText><string value=\"Invitation &amp; &lt;Text&gt;\"/>\r\n</displayText></invitation>";

            IXmlElement element = new XmlElement(xmlString);
            var invitation = new DocumentNamedInvitationField(element);

            Assert.AreEqual(xmlString, invitation.ToXml());
        }

        [Test]
        public void TrailingBackslashCharacterInRtfDoesNotGenerateAnException()
        {
            string docRtf =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" +
                @"{\f0\fswiss\fcharset0\fprq2 Arial;}" +
                @"{\f1\froman\fcharset2\fprq2 Symbol;}}" +
                @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;\red0\green0\blue128;}" +
                @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" +
                @"{\*\generator TX_RTF32 12.0.500.502;}" +
                @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080\plain\f0\fs20\ul\cf3{\*\txfieldstart\txfieldtype0\txfieldflags219\txfielddataval15" +
                @"\txfielddata 4900460024005c003a0046006f0072006d00200031003a0032003a00260020005c000000}& \\{\*\txfieldend}\par }";
            try
            {
                var rtfDoc = new RtfDocument("Document");
                rtfDoc.Rtf = docRtf;
            }
            catch (XmlException)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void XmlReservedCharactersInRtfDoNotGenerateAnException()
        {
            string docRtf =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" +
                @"{\f0\fswiss\fcharset0\fprq2 Arial;}" +
                @"{\f1\froman\fcharset2\fprq2 Symbol;}}" +
                @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;\red0\green102\blue204;}" +
                @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" +
                @"{\*\generator TX_RTF32 12.0.500.502;}" +
                @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080\plain\f0\fs20\ul\cf3{\*\txfieldstart\txfieldtype0\txfieldflags219\txfielddataval13\txfielddata 4900460024005300690067006e00750070003a005300690067006e005500700046006f0072006d003a0030003a0061003c000000}a<{\*\txfieldend}\par }";

            try
            {
                var rtfDoc = new RtfDocument("Document");
                rtfDoc.Rtf = docRtf;
            }
            catch (XmlException)
            {
                Assert.Fail();
            }
        }

        // NOTE: The change of Invitation Fields to use an ID for passing data via the RTF
        //		 obviates the need for the following test. However the more general tests, above,
        //		 to check for backslashes in RTF, are still valid and useful.
        //															jdf - 5/07

        //private static int getInvitationFieldId(DocumentNamedInvitationField invitationField)
        //{
        //    BindingFlags flags =
        //        BindingFlags.NonPublic |
        //        BindingFlags.Public |
        //        BindingFlags.Static |
        //        BindingFlags.Instance;

        //    FieldInfo field = typeof(DocumentNamedInvitationField).GetField("id", flags);
        //    return (int)field.GetValue(invitationField);
        //}

        //[Test]
        //public void BackslashCharacterInXmlProducesBackslashCharacterInRtf()
        //{
        //    Form form = setupProjectWithForm();

        //    string xmlString =
        //        "<invitation form=\"Form 1\" project=\"\">Invitation Text with backslash \\</invitation>";

        //    IXmlElement element = new XmlElement(xmlString);
        //    DocumentNamedInvitationField documentInvitationField = new DocumentNamedInvitationField(element);

        //    Assert.AreEqual("Invitation Text with backslash \\", documentInvitationField.Invitation.DisplayText);

        //    string expectedRtf =
        //        @"{\*\txfieldstart\txfieldtype0\txfieldflags219" +
        //        @"\txfielddataval" + getInvitationFieldId(documentInvitationField) +
        //        @"\txfielddata " + RtfUtility.EncodeHexString(@"IF$\:Form 1:" + form.Id.ToString() + ":Invitation Text with backslash \\") + "}" +
        //        @"Invitation Text with backslash \\{" +
        //        @"\*\txfieldend}";

        //    Assert.AreEqual(expectedRtf, documentInvitationField.ToRtf());
        //}
    }
}