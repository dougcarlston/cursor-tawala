// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.Projects.Links;
using Tawala.Projects.Properties;
using Tawala.ProjectUI;
using Tawala.RtfSupport;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.Invitations
{
    [TestFixture]
    public class PrivateInvitationInDocument1602
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
            form = Project.Current.AddForm();

            var fibItem = new FibItem();
            form.ItemList.Add(fibItem);

            fieldsPalette = new FieldsPalette();
            fieldsPalette.Show();
        }

        [TearDown]
        public void TearDown()
        {
            fieldsPalette.Dispose();
        }

        #endregion

        private IForm form;

        private FieldsPalette fieldsPalette;

        private const string NEWLINE = "\r\n";

        private const string rtfPrefixString =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + NEWLINE +
            @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + NEWLINE +
            @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + NEWLINE +
            @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + NEWLINE +
            @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + NEWLINE +
            @"{\*\generator TX_RTF32 12.0.500.502;}" + NEWLINE;

        private const string defaultTabPositionsString =
            "<tabPositions>" +
            "<tabStop position=\"1134\"/>" +
            "<tabStop position=\"2268\"/>" +
            "<tabStop position=\"3402\"/>" +
            "<tabStop position=\"4536\"/>" +
            "<tabStop position=\"5670\"/>" +
            "<tabStop position=\"6804\"/>" +
            "<tabStop position=\"7938\"/>" +
            "<tabStop position=\"9072\"/>" +
            "<tabStop position=\"10206\"/>" +
            "<tabStop position=\"11340\"/>" +
            "<tabStop position=\"12474\"/>" +
            "<tabStop position=\"13608\"/>" +
            "<tabStop position=\"14742\"/>" +
            "<tabStop position=\"15876\"/>" +
            "</tabPositions>";

#pragma warning disable 169
        private string oldPrivateInvitationPersistenceXml =
#pragma warning restore 169
            "<invitation form=\"Form 1\" project=\"\" private=\"true\">" +
            "<authenticationTokenValue>" +
            "<string value=\"Text \"/>" + NEWLINE +
            "<string field=\"Form 1:Q1:a\"/>" + NEWLINE +
            "</authenticationTokenValue>" +
            "<displayText>" +
            "<string value=\"click here\"/>" + NEWLINE +
            "</displayText>" +
            "</invitation>";

        private const string newPrivateInvitationPersistenceXml =
            "<invitation form=\"Form 1\" project=\"\" private=\"true\">" +
            "<authenticationTokenValue>" +
            "<string value=\"Text \"/>" + NEWLINE +
            "<field name=\"Form 1:Q1:a\"/>" + NEWLINE +
            "</authenticationTokenValue>" +
            "<displayText>" +
            "<string value=\"click here\"/>" + NEWLINE +
            "</displayText>" +
            "</invitation>";

        [Test]
        public void InviteeIDVariableExisitInFieldsPalette()
        {
            fieldsPalette.RefreshFieldList();
            Assert.AreEqual(2, fieldsPalette.FieldsTreeView.Nodes.Count);
            Assert.AreEqual("Form 1", fieldsPalette.FieldsTreeView.Nodes[0].Text);
            Assert.AreEqual("Variables", fieldsPalette.FieldsTreeView.Nodes[1].Text);

            Assert.AreEqual(1, fieldsPalette.FieldsTreeView.Nodes[1].Nodes.Count);
            Assert.AreEqual(Resources.PrivateInvitationVariableLabel, fieldsPalette.FieldsTreeView.Nodes[1].Nodes[0].Text);
        }

        [Test]
        public void ParserXmlWithIdGeneratesProperDocumentInvitationField()
        {
            var invitationField = new InvitationField();

            string xmlString = "<invitation id=\"" + invitationField.Id + "\"/>";

            IXmlElement element = new XmlElement(xmlString);

            var documentInvitationField = new DocumentIdedInvitationField(element);
            Assert.AreSame(invitationField, documentInvitationField.Invitation);
        }

        [Test]
        public void PersistenceXmlGeneratesRtfWithId()
        {
            string persistenceXml =
                "<invitation form=\"Form 1\" project=\"\"><displayText><string value=\"click here\"/></displayText></invitation>";

            var documentInvitationField = new DocumentNamedInvitationField(new XmlElement(persistenceXml));

            string encryptedInvitationData = RtfUtility.EncodeHexString(@"IF$" + documentInvitationField.Invitation.Id);

            string expectedRtf =
                @"{\*\txfieldstart\txfieldtype0\txfieldflags219" +
                @"\txfielddataval" + documentInvitationField.Invitation.Id +
                @"\txfielddata " + encryptedInvitationData + "}" +
                @"click here{" +
                @"\*\txfieldend}";

            Assert.AreEqual(expectedRtf, documentInvitationField.ToRtf());
        }

        [Test]
        public void PrivateInvitationFieldGeneratesPrivateInvitationPersistenceXml()
        {
            var invitationField = new InvitationField();
            invitationField.Form = form;
            invitationField.ProjectName = "";
            invitationField.DisplayText = "click here";
            invitationField.IsPrivate = true;
            invitationField.AuthenticationTokenExpression = new FieldsAndLiteralsExpression("Text <<Form 1:Q1:a>>");

            Assert.AreEqual(newPrivateInvitationPersistenceXml, invitationField.ToXml());
        }

        [Test]
        public void PrivateInvitationPersistenceXmlGeneratesPrivateInvitationField()
        {
            var documentInvitationField = new DocumentNamedInvitationField(new XmlElement(newPrivateInvitationPersistenceXml));

            InvitationField invitation = documentInvitationField.Invitation;
            Assert.AreEqual(true, invitation.IsPrivate);
            Assert.IsTrue(invitation.AuthenticationTokenExpression is FieldsAndLiteralsExpression);
            Assert.AreEqual(2, invitation.AuthenticationTokenExpression.Elements.Count);
            Assert.AreEqual("Text <<Form 1:Q1:a>>", invitation.AuthenticationTokenExpression.ToString());
        }

        [Test]
        public void OldPrivateInvitationPersistenceXmlGeneratesPrivateInvitationField()
        {
            var documentInvitationField = new DocumentNamedInvitationField(new XmlElement(oldPrivateInvitationPersistenceXml));

            InvitationField invitation = documentInvitationField.Invitation;
            Assert.AreEqual(true, invitation.IsPrivate);
            Assert.IsTrue(invitation.AuthenticationTokenExpression is FieldsAndLiteralsExpression);
            Assert.AreEqual(2, invitation.AuthenticationTokenExpression.Elements.Count);
            Assert.AreEqual("Text <<Form 1:Q1:a>>", invitation.AuthenticationTokenExpression.ToString());
        }

        [Test]
        public void RtfWithIdGeneratesValidInvitationField()
        {
            var invitationField = new InvitationField();
            invitationField.Form = form;
            invitationField.ProjectName = "";
            invitationField.DisplayText = "Click here";

            string encryptedInvitationData = RtfUtility.EncodeHexString(@"IF$" + invitationField.Id);

            string rtfString =
                rtfPrefixString +
                @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
                @"\itap0\plain\f0\fs24" +
                @"{\*\txfieldstart\txfieldtype0\txfieldflags219" +
                @"\txfielddataval" + invitationField.Id +
                @"\txfielddata " + encryptedInvitationData + "}" +
                @"click here{" +
                @"\*\txfieldend}\par }";

            var parser = new RtfParser(rtfString);
            parser.Parse();

            string expectedXml =
                "<paragraph indent=\"0\" align=\"left\">" +
                defaultTabPositionsString +
                "<invitation id=\"" + invitationField.Id + "\"/>" +
                "</paragraph>";

            var paragraph = new Paragraph(new XmlElement(parser.ToXml()));

            Assert.AreEqual(1, paragraph.Contents.Count);

            var documentInvitationField = paragraph.Contents[0] as DocumentInvitationField;
            Assert.IsNotNull(documentInvitationField);

            Assert.AreSame(invitationField, documentInvitationField.Invitation);
        }
    }
}