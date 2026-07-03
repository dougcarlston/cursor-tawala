// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.Projects.Links;
using Tawala.RtfSupport;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;
using NUnit.Framework;

namespace TawalaTest.BugTest
{
	[TestFixture]
	public class PastingDocumentWithInvitationCausesException642 : ClipboardTester<RtfDocument>
	{
		private static string rtfFieldStringPrefix =
			RtfConstants.DefaultRtfPrologue +
			@"{\*\txfieldstart\txfieldtype0\txfieldflags219";

		private static string privateInvitationXmlString =
			"<invitation form=\"Form 1\" project=\"\" private=\"true\">" +
			"<authenticationTokenValue>" +
			"<string value=\"Text \"/>" + Environment.NewLine +
			"<string field=\"Form 1:Q1:a\"/>" + Environment.NewLine +
			"</authenticationTokenValue>" +
			"click here" +
			"</invitation>";

		[Test]
		public void InvitationInCopiedDocumentIsValidCopy()
		{
			SetUpTest();

			IForm form = project.AddForm();
			FibItem fib = new FibItem();
			form.ItemList.Add(fib);
			RtfDocument document = project.AddDocument() as RtfDocument;

			InvitationField invitationField = new InvitationField(new XmlElement(privateInvitationXmlString));

			string encryptedInvitationData = RtfUtility.EncodeHexString(@"IF$" + invitationField.Id);

			document.Rtf =
				rtfFieldStringPrefix +
				@"\txfielddataval" + invitationField.Id +
				@"\txfielddata " + encryptedInvitationData + "}" +
				@"click here{" +
				@"\*\txfieldend}\par }";

			RtfDocument copiedDocument = CopyPaste();
			Assert.IsNotNull(copiedDocument, ErrorMessage);

			Project.Current.PasteDocument(copiedDocument);

			DocumentInvitationField copiedField = findDocumentInvitationField(copiedDocument);
			Assert.IsInstanceOfType(typeof(InvitationField), copiedField.Invitation);

			Assert.AreSame(form, copiedField.Invitation.Form);
			Assert.AreEqual("click here", copiedField.Invitation.DisplayText);
			Assert.IsTrue(copiedField.Invitation.IsPrivate);
			Assert.AreEqual("Text <<Form 1:Q1:a>>", copiedField.Invitation.AuthenticationTokenExpression.ToString());
		}


		private DocumentInvitationField findDocumentInvitationField(RtfDocument doc)
		{
			Assert.AreEqual(1, doc.Contents.Count, "Expect doc.Contents to contain 1 element");
			Assert.IsAssignableFrom(typeof(Paragraph), doc.Contents[0], "Expect doc.Contents[0] to be Paragraph");
			Paragraph paragraph = doc.Contents[0] as Paragraph;
			Assert.AreEqual(1, paragraph.Contents.Count, "Expect paragraph.Contents to contain 1 element");
			return findDocumentInvitationField(paragraph.Contents[0]);
		}

		private DocumentInvitationField findDocumentInvitationField(IParagraphComponent component)
		{
			DocumentInvitationField field = component as DocumentInvitationField;
			ParagraphInlineComponent inline = component as ParagraphInlineComponent;

			if (field != null)
				return field;


			ParagraphComponentList list = component as ParagraphComponentList;

			if (list != null)
			{
				field = list[0] as DocumentInvitationField;

				if (field != null)
				{
					return field;
				}
			}

			Assert.IsNotNull(inline, "Expect ParagraphInlineComponent if not DocumentField");
			Assert.AreNotEqual(ParagraphComponent.NULL, inline.Contents, "Expect inline.Contents is not ParagraphComponent.Null");

			return findDocumentInvitationField(inline.Contents);
		}

		protected override RtfDocument GetComponent()
		{
			return Project.Current.DocumentList[0] as RtfDocument;
		}
    }
}
