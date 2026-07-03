// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.ProjectUI;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.GetAndForEach
{
    /// <summary>
    /// Acceptance tests for story 1902 (Designer compares record fields to Form fields in GET).
    /// </summary>
    [TestFixture]
    public class CompareRecordFieldsTest1902
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            Util.NewTestProject();

            setupForm();
            setupProcess();
            setupFieldsPalette();
        }

        [TearDown]
        public void TearDown()
        {
            process.ActiveGetStatement = null;
            testPalette.Dispose();
        }

        #endregion

        private FieldsPalette testPalette;

        private IForm form;
        private Blank blank;
        private Process process;
        private FormList forms;
        private RecordSet recordSet;
        private GetStatement getStatement;

        private void setupForm()
        {
            form = Project.Current.AddForm();

            var fibItem = new FibItem();
            form.ItemList.Add(fibItem);
            blank = fibItem.BlankList[0];
        }

        private void setupProcess()
        {
            process = Project.Current.AddProcess();

            getStatement = getGetStatement();
            addGetStatement(getStatement);

            addCommentStatement();
        }

        private void addGetStatement(GetStatement getStatement)
        {
            process.Lines.Add(getGetLine(getStatement));
        }

        private GetStatement getGetStatement()
        {
            forms = new FormList();
            forms.Add(form);
            recordSet = new RecordSet("Records", forms);

            return (new GetStatement(recordSet));
        }

        private static ProcessLineList getGetLine(GetStatement getStatement)
        {
            return (new ProcessLineList(getStatement));
        }

        private void addCommentStatement()
        {
            process.Lines.Add(new ProcessLineList(new CommentStatement("This is a comment")));
        }

        private void setupFieldsPalette()
        {
            testPalette = new FieldsPalette();
            testPalette.Show();
        }

        [Test]
        public void FieldsPaletteInitiallyHasOnlyFormNode()
        {
            testPalette.RefreshFieldList();

            Assert.AreEqual(2, testPalette.FieldsTreeView.Nodes.Count);
            Assert.AreEqual("Form 1", testPalette.FieldsTreeView.Nodes[0].Text);
        }

        [Test]
        public void ProcessContainsCommentAndGetStatements()
        {
            Assert.AreEqual("Get Records from Form 1", process.Lines[0].ToString());
            Assert.AreEqual("This is a comment", process.Lines[1].ToString());
        }

        [Test]
        public void ProjectConversionProducesProperXml()
        {
            string xmlString =
                "<project name=\"Story1902Test\" themePath=\"default\" format=\"1.4\">" +
                "<forms>\r\n" +
                "<form name=\"Form 1\" startPoint=\"true\" themePath=\"default\">\r\n" +
                "<items>\r\n" +
                "<fib label=\"Q1\">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "<font face=\"Arial\" size=\"200\" color=\"000000\">FIB 1: " +
                "</font>" +
                "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
                "</paragraph>" +
                "</fib>" +
                "</items>" +
                "</form>" +
                "</forms>\r\n" +
                "<processes>" +
                "<process name=\"Process 2\">\r\n" +
                "<get recordList=\"Record List 1\">\r\n" +
                "<forms>\r\n" +
                "<form name=\"Form 1\"/>\r\n" +
                "</forms>\r\n" +
                "<conditions>\r\n" +
                "<equals field=\"Form 1:Q1:a\">\r\n" +
                "<string field=\"Record List 1:Form 1:Q1:a\"/>\r\n" +
                "</equals>\r\n" +
                "</conditions>\r\n" +
                "</get>\r\n" +
                "</process>\r\n" +
                "</processes>\r\n" +
                "</project>";

            using (var xmlStream = new MemoryStream())
            {
                byte[] xmlByteArray = Encoding.UTF8.GetBytes(xmlString);
                xmlStream.Write(xmlByteArray, 0, xmlByteArray.Length);

                var converter = new TawalaProjectConverter(xmlStream);
                converter.ConvertXmlToProject();
            }

            process = Project.Current.ProcessList[0];
            var statement = (GetStatement)process.Lines[0].Statement;

            string expectedXml =
                "<get recordList=\"Record List 1\">\r\n" +
                "<forms>\r\n" +
                "<form name=\"Form 1\"/>\r\n" +
                "</forms>\r\n" +
                "<conditions>\r\n" +
                "<equals field=\"Form 1:Q1:a\">\r\n" +
                "<string field=\"Record List 1:Form 1:Q1:a\"/>\r\n" +
                "</equals>\r\n" +
                "</conditions>" +
                "</get>";

            Assert.AreEqual(expectedXml, statement.ToXml());
        }

        [Test]
        public void RecordListNodeHasSingleSubNode()
        {
            process.ActiveGetStatement = getStatement;
            Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, 0));
            testPalette.RefreshFieldList();

            TreeNode recordListNode = testPalette.FieldsTreeView.Nodes[2];

            Assert.AreEqual(1, recordListNode.Nodes.Count);
            Assert.AreEqual("Form 1:Q1:a", recordListNode.Nodes[0].Text);
        }

        [Test]
        public void RecordSetXmlProducesRecordSetXml()
        {
            string xmlString =
                "<process name=\"Process 2\">\r\n" +
                "<get recordList=\"Record List 1\">\r\n" +
                "<forms>\r\n" +
                "<form name=\"Form 1\"/>\r\n" +
                "</forms>\r\n" +
                "<conditions>\r\n" +
                "<equals field=\"Form 1:Q1:a\">\r\n" +
                "<string field=\"Record List 1:Form 1:Q1:a\"/>\r\n" +
                "</equals>\r\n" +
                "</conditions>" +
                "</get>\r\n" +
                "</process>\r\n";

            var process = new Process(new XmlElement(xmlString));

            Assert.AreEqual(xmlString, process.ToXml());
        }

        [Test]
        public void SelectingGetStatementDisplaysRecordListNode()
        {
            process.ActiveGetStatement = getStatement;
            Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, 0));
            testPalette.RefreshFieldList();

            Assert.AreEqual(3, testPalette.FieldsTreeView.Nodes.Count);
            Assert.AreEqual("Records", testPalette.FieldsTreeView.Nodes[2].Text);
        }
    }
}