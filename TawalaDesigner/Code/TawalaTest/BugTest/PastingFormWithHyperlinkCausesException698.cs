// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

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
	public class PastingFormWithHyperlinkCausesException698 : ClipboardTester<IForm>
	{
		private static string hyperlinkXml =
			@"<link>" + Environment.NewLine +
			@"<description>" + Environment.NewLine +
			@"<string value=""Click here""/>" + Environment.NewLine +
			@"</description>" + Environment.NewLine +
			@"<url>" + Environment.NewLine +
			@"<string value=""http://""/>" + Environment.NewLine +
			@"<field name=""Form 1:Q1:a""/>" + Environment.NewLine +
			@"</url>" + Environment.NewLine +
			@"</link>" + Environment.NewLine;

		private static string projectXmlWithHyperlinkInForm2 =
			@"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
			@"<project name=""HypertextLink in Form"" themePath=""default"" format=""1.9"" designerBuild=""194"">" + Environment.NewLine +
			@"<forms>" + Environment.NewLine +
			@"<form name=""Form 1"" startPoint=""true"" themePath=""default"">" + Environment.NewLine +
			@"<items>" + Environment.NewLine +
			@"<fib label=""Q1"" style=""topLabels""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
			@"position=""2880""/></tabPositions><font face=""Arial"" size=""200"" color=""000000"">[Replace this " +
			@"with your question. Underscores create blanks.] </font><blank label=""a"" length=""20"" " +
			@"required=""false""/></paragraph></fib>" + Environment.NewLine +
			@"</items>" + Environment.NewLine +
			@"</form>" + Environment.NewLine +
			@"<form name=""Form 2"" startPoint=""false"" themePath=""default"">" + Environment.NewLine +
			@"<items>" + Environment.NewLine +
			@"<text label=""T1""" + XmlConstants.DefaultTextItemStyleAttribute + @"><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
			@"position=""2880""/></tabPositions><font color=""0066CC""><u>" +
			hyperlinkXml +
			@"</u></font></paragraph></text>" + Environment.NewLine +
			@"</items>" + Environment.NewLine +
			@"</form>" + Environment.NewLine +
			@"</forms>" + Environment.NewLine +
			@"</project>";

        [Ignore("FOR COMMIT")]
		[Test]
		public void HyperlinkInCopiedFormIsValidCopy()
		{
			Util.OpenProjectXml(projectXmlWithHyperlinkInForm2);

			setFormToBeCopied(Project.Current.FormList[1]);

			IForm copiedForm = CopyPaste();
			Assert.IsNotNull(copiedForm);

			TextItem copiedTextItem = copiedForm.ItemList[0] as TextItem;

			HyperlinkField copiedField = findHyperlinkField(copiedTextItem);
			Assert.IsNotNull(copiedField);
			Assert.AreEqual(hyperlinkXml, copiedField.ToXml());
		}

		private HyperlinkField findHyperlinkField(TextItem textItem)
		{
			Assert.AreEqual(1, textItem.Contents.Count, "Expect textItem.Contents to contain 1 element");
			Assert.IsAssignableFrom(typeof(FormItemParagraph), textItem.Contents[0], "Expect textItem.Contents[0] to be FormItemParagraph");
			FormItemParagraph paragraph = textItem.Contents[0] as FormItemParagraph;
			Assert.AreEqual(1, paragraph.Contents.Count, "Expect paragraph.Contents to contain 1 element");
			return findHyperlinkField(paragraph.Contents[0]);
		}

		private HyperlinkField findHyperlinkField(IParagraphComponent component)
		{
			HyperlinkField field = component as HyperlinkField;
			ParagraphInlineComponent inline = component as ParagraphInlineComponent;

			if (field != null)
				return field;


			ParagraphComponentList list = component as ParagraphComponentList;

			if (list != null)
			{
				field = list[0] as HyperlinkField;

				if (field != null)
				{
					return field;
				}
			}

			Assert.IsNotNull(inline, "Expect ParagraphInlineComponent if not HyperlinkField");
			Assert.AreNotEqual(ParagraphComponent.NULL, inline.Contents, "Expect inline.Contents is not ParagraphComponent.Null");

			return findHyperlinkField(inline.Contents);
		}

		private IForm formToBeCopied = Form.NULL;

		private void setFormToBeCopied(IForm form)
		{
			formToBeCopied = form;
		}

		protected override IForm GetComponent()
		{
			return formToBeCopied;
		}
	}
}
