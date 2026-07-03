// $Workfile: ClipboardCopyPasteDocument296.cs $
// $Revision: 6 $	$Date: 1/22/07 9:36a $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using TawalaTest.TestSupport;
using NUnit.Framework;

namespace TawalaTest.BugTest
{
    [TestFixture]
    public class ClipboardCopyPasteDocument296 : ClipboardTester<RtfDocument>
    {
        private RtfDocument document1;
        private IForm form1;

        [SetUp]
        public void SetUp()
        {
            SetUpTest();
            document1 = project.AddDocument() as RtfDocument;
            form1 = null;
        }

        [Test]
        public void Empty()
        {
            Util.OpenProjectXml(Properties.Resources.ProjectXmlDocumentEmpty296);
            project = Project.Current;
            document1 = project.DocumentList[0] as RtfDocument;

            RtfDocument clipboardCopy = CopyPaste();
            Assert.IsNotNull(clipboardCopy, ErrorMessage);
            Assert.AreNotSame(document1, clipboardCopy);
            Assert.AreEqual(1, project.DocumentList.Count);

            project.PasteDocument(clipboardCopy);
            Assert.AreEqual(2, project.DocumentList.Count);
        }

        [Test]
        public void TableInDocument()
        {
            document1.Rtf = Properties.Resources.RtfWithTable296;
            RtfDocument clipboardCopy = CopyPaste();
            Assert.IsNotNull(clipboardCopy, ErrorMessage);
        }

        [Test]
        public void DocumentFieldInDocumentFromFibField()
        {
			Util.OpenProjectXml(Properties.Resources.ProjectXmlDocumentWithFibField296);
            project = Project.Current;

            form1 = project.FormList[0];
            document1 = project.DocumentList[0] as RtfDocument;
            FieldList form1Fields = form1.GetAllFields();
            DocumentField df1 = findDocumentField(document1);

            Assert.IsInstanceOfType(typeof(FibItem), form1.ItemList[0]);
            Assert.AreEqual("<<Form 1:Q1:a>>", form1Fields[0].FieldString);

            RtfDocument clipboardCopy = CopyPaste();
            Assert.IsNotNull(clipboardCopy, ErrorMessage);

            Project.Current.PasteDocument(clipboardCopy);

            DocumentField df2 = findDocumentField(clipboardCopy);
            Assert.AreEqual("Form 1:Q1:a", df1.Field.QualifiedFieldName);
            Assert.AreEqual(df1.Field.QualifiedFieldName, df2.Field.QualifiedFieldName);
            Assert.AreSame(df1.Field, df2.Field);
            Assert.AreNotSame(df1, df2);
        }

        [Test]
        public void DocumentFieldInDocumentFromVariable()
        {
            Util.OpenProjectXml(Properties.Resources.ProjectXmlDocumentWithVariableField296);
            project = Project.Current;

            document1 = project.DocumentList[0] as RtfDocument;
            DocumentField df1 = findDocumentField(document1);

            Assert.IsInstanceOfType(typeof(Variable), df1.Field);

            RtfDocument clipboardCopy = CopyPaste();
            Assert.IsNotNull(clipboardCopy, ErrorMessage);

            Project.Current.PasteDocument(clipboardCopy);

            DocumentField df2 = findDocumentField(clipboardCopy);
            Assert.AreEqual("MyVariable", df1.Field.QualifiedFieldName);
            Assert.AreEqual(df1.Field.QualifiedFieldName, df2.Field.QualifiedFieldName);
            Assert.AreSame(df1.Field, df2.Field);
            Assert.AreNotSame(df1, df2);
        }

        private DocumentField findDocumentField(RtfDocument doc)
        {
            Assert.AreEqual(1, doc.Contents.Count, "Expect doc.Contents to contain 1 element");
            Assert.IsAssignableFrom(typeof(Paragraph), doc.Contents[0], "Expect doc.Contents[0] to be Paragraph");
            Paragraph paragraph = doc.Contents[0] as Paragraph;
            Assert.AreEqual(1, paragraph.Contents.Count, "Expect paragraph.Contents to contain 1 element");

			Console.WriteLine("component is {0}", paragraph.Contents[0].GetType().FullName);

			return findDocumentField(paragraph.Contents[0]);
        }

        private DocumentField findDocumentField(IParagraphComponent component)
        {
            DocumentField field = component as DocumentField;
            ParagraphInlineComponent inline = component as ParagraphInlineComponent;

			if (field != null)
			{
				return field;
			}

			ParagraphComponentList list = component as ParagraphComponentList;

			if (list != null)
			{
				field = list[0] as DocumentField;

				if (field != null)
				{
					return field;
				}
			}

			Assert.IsNotNull(inline, "Expect ParagraphInlineComponent if not DocumentField");
			Assert.AreNotEqual(ParagraphComponent.NULL, inline.Contents, "Expect inline.Contents is not ParagraphComponent.Null");

			return findDocumentField(inline.Contents);
        }

        protected override RtfDocument GetComponent()
        {
            return Project.Current.DocumentList[0] as RtfDocument;
        }
    }
}
