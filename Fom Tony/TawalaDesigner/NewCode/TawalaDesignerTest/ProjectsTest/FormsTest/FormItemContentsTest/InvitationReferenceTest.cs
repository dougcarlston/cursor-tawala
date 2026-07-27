using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Documents;
using Tawala.Projects.Factories;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Images;
using TawalaTest.TestingSupport;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest.Forms.FormItemContents
{
    [TestFixture]
    public class InvitationReferenceTest
    {
        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            ComponentMaker.UseNewComponents(true);
        }

        [TearDown]
        public void TearDown()
        {
            ComponentMaker.UseNewComponents(false);
        }

        [Test]
        public void CanConstructInvitationFieldLinkReferenceFromXml()
        {
            InvitationReference invitationRef = new InvitationReference(new XmlElement(xmlInvitation));

            InvitationField invitation = invitationRef.Invitation;

            Assert.IsNotNull(invitation);
            Assert.AreEqual("Invitation", invitation.DisplayText);
            Assert.AreEqual("Form 1", invitation.InitialFormName);
            Assert.IsTrue(invitation.IsPrivate);
            Assert.AreEqual("<<_InviteeID>>", invitation.AuthenticationTokenExpression.ToString());
        }

        [Test]
        public void CanConstructProjectContainingPrivateInvitationFromXml()
        {
            Project.Create(new XmlElement(projectXmlWithPrivateInvitation));

            IDocument document = Project.Current.DocumentList[0];
            FormItemContentsCollection collection = document.NewContents.GetDescendants(typeof(InvitationReference));

            Assert.AreEqual(1, collection.Count);

            InvitationReference invitationRef = collection[0] as InvitationReference;
            InvitationField invitation = invitationRef.Invitation;

            Assert.IsNotNull(invitation);
            Assert.AreEqual("Invitation", invitation.DisplayText);
            Assert.AreEqual("Form 1", invitation.InitialFormName);
            Assert.IsTrue(invitation.IsPrivate);
            Assert.AreEqual("<<_InviteeID>>", invitation.AuthenticationTokenExpression.ToString());
        }

        [Test]
        public void CanConstructInvitationReferenceFromXhtml()
        {
            IForm form1 = Project.Current.AddForm();
            InvitationField testLink = new InvitationField();
            testLink.DisplayText = "Invitation Text";
            testLink.Form = form1;
            testLink.AuthenticationTokenExpression = new CompoundExpression("<<_InviteeID>>");
            testLink.IsPrivate = true;

            string xhtml = string.Format("<t:link id=\"link_" + testLink.Id + "\">" + testLink.DisplayText + "</t:link>");

            IFormItemContents contents = FormItemContentsFactory.MakeObject(new XhtmlElement(xhtml, true));
            Assert.IsNotNull(contents);

            InvitationReference linkRef = contents as InvitationReference;
            Assert.IsNotNull(linkRef);

            InvitationField invitation = linkRef.Invitation;

            Assert.IsNotNull(invitation);
            Assert.AreEqual("Invitation Text", invitation.DisplayText);
            Assert.AreEqual(form1, invitation.Form);
        }

        private string xmlInvitation =
            @"<invitation form=""Form 1"" project="""" private=""true"">" + 
            @"<authenticationTokenValue>" +
            @"<string field=""_InviteeID""/>" + Environment.NewLine +
            @"</authenticationTokenValue>" + 
            @"Invitation" + 
            @"</invitation>";

        private string projectXmlWithPrivateInvitation = 
            @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
            @"<project name=""InvitationPrivateWithInviteeID"" themePath=""default"" format=""1.10"" " +
            @"designerBuild=""209"">" + Environment.NewLine +
            @"<forms>" + Environment.NewLine +
            @"<form name=""Form 1"" startPoint=""true"" themePath=""default"">" + Environment.NewLine +
            @"<items>" + Environment.NewLine +
            @"<text label=""T1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
            @"position=""2880""/></tabPositions><font>[Replace this with text of your " +
            @"own.]</font></paragraph></text>" + Environment.NewLine +
            @"</items>" + Environment.NewLine +
            @"</form>" + Environment.NewLine +
            @"</forms>" + Environment.NewLine +
            @"<documents>" + Environment.NewLine +
            @"<document name=""Document 1"">" + Environment.NewLine +
            @"<xmlData>" + Environment.NewLine +
            @"<paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
            @"position=""2880""/></tabPositions><font color=""0066CC""><u><invitation form=""Form 1"" " +
            @"project="""" private=""true""><authenticationTokenValue><string field=""_InviteeID""/>" + Environment.NewLine +
            @"</authenticationTokenValue>Invitation</invitation></u></font></paragraph>" + Environment.NewLine +
            @"</xmlData>" + Environment.NewLine +
            @"</document>" + Environment.NewLine +
            @"</documents>" + Environment.NewLine +
            @"</project>" + Environment.NewLine;
    }
}
