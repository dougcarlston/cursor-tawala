// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Components;
using Tawala.Projects.Documents;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.Projects.Properties;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;
using GlobalSettings=Tawala.Common.GlobalSettings;
using XmlElement=Tawala.XmlSupport.XmlElement;

namespace TawalaTest.ProjectTest
{
    [TestFixture]
    public class ProjectTest
    {
        // used by some test methods to make sure an event is raised only once
        // set to bogus value to make sure its initialized before use

        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            raiseCount = 999999;
            xml = null;

            Util.NewTestProject();

            string tmpProjName = Path.GetRandomFileName() + ".test.tawala";
            tmpProjPath = Path.Combine(Path.GetTempPath(), tmpProjName);
        }

        [TearDown]
        public void TearDown()
        {
            xml = null;

            try
            {
                File.Delete(tmpProjPath);
            }
            catch (Exception)
            {
            }
        }

        #endregion

        private int raiseCount = 65535;

        private string credentialsXml;
        private XmlDocument xml;
        private string tmpProjPath;

        // execute this once at beginning of tests
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            credentialsXml = GlobalSettings.CredentialsElement("TestUserName", "TestUserPW");
        }

        private void handleNewProjectEvent(object sender, ProjectEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.ProjectName);
            Assert.AreEqual(Resources.ProjectDefaultName, args.ProjectName);
        }

        private void handleFormAddedEvent(object sender, ComponentEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.Component);
            Assert.AreEqual(Resources.FormDefaultBaseName + " 1", args.Component.Name);
        }

        private void handleFormRemovedEvent(object sender, ComponentEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.Component);
            Assert.AreEqual(Resources.FormDefaultBaseName + " 1", args.Component.Name);
        }

        public void HandleFormRenamedEvent(object sender, ComponentRenamedEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.Component);
            Assert.AreEqual("Form 1 Renamed", args.Component.Name);
        }

        public void HandleCurrentFormSetEvent(object sender, ComponentEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.Component);
            Assert.AreEqual(Resources.FormDefaultBaseName + " 1", args.Component.Name);
        }

        // Note: Do not use [Test] attribute for event handlers
        public void HandleNewTextItemEvent(object sender, FormItemEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.Item);

            var item = (TextItem)(args.Item);
            Assert.AreEqual(RtfConstants.TextItemDefaultRTF, item.Rtf);
        }

        // Note: Do not use [Test] attribute for event handlers
        public void HandleNewFibItemEvent(object sender, FormItemEventArgs args)
        {
            Assert.IsNotNull(args.Item);

            var item = (FibItem)(args.Item);
//			Assert.AreEqual(Project.Resources.GetString("FibItemDefaultText"), item.Text);
            Assert.AreEqual(1, item.BlankList.Count);
        }

        // Note: Do not use [Test] attribute for event handlers
        public void HandleNewMCItemEvent(object sender, FormItemEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.Item);

            var item = (McqItem)(args.Item);
            Assert.AreEqual(args.Form.Name, "Test Form");
            Assert.AreEqual(Resources.MCItemDefaultText, item.Text);
        }

        // handler for "document added" event
        // Note: Do not use [Test] attribute for event handlers
        private void handleDocumentAddedEvent(object sender, ComponentEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.Component);
            Assert.AreEqual(Resources.DocumentDefaultBaseName + " 1", args.Component.Name);
        }

        // handler for "document removed" event
        // Note: Do not use [Test] attribute for event handlers
        public void HandleDocumentRemovedEvent(object sender, ComponentEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.Component);
            Assert.AreEqual(Resources.DocumentDefaultBaseName + " 1", args.Component.Name);
        }

        // handler for "document renamed" event
        // Note: Do not use [Test] attribute for event handlers
        public void HandleDocumentRenamedEvent(object sender, ComponentRenamedEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.Component);
            Assert.AreEqual("Document 1 Renamed", args.Component.Name);
        }

        // handler for "current document set" event
        // Note: Do not use [Test] attribute for event handlers
        public void HandleCurrentDocumentSetEvent(object sender, ComponentEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.Component);
            Assert.AreEqual(Resources.DocumentDefaultBaseName + " 1", args.Component.Name);
        }

        // handler for "process added" event
        // Note: Do not use [Test] attribute for event handlers
        public void HandleProcessAddedEvent(object sender, ComponentEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.Component);
            Assert.AreEqual(Resources.ProcessDefaultBaseName + " 1", args.Component.Name);
        }

        // handler for "process removed" event
        // Note: Do not use [Test] attribute for event handlers
        private void handleProcessRemovedEvent(object sender, ComponentEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.Component);
            Assert.AreEqual(Resources.ProcessDefaultBaseName + " 1", args.Component.Name);
        }

        // handler for "process renamed" event
        // Note: Do not use [Test] attribute for event handlers
        public void HandleProcessRenamedEvent(object sender, ComponentRenamedEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.Component);
            Assert.AreEqual("Process 1 Renamed", args.Component.Name);
        }

        // handler for "current process set" event
        // Note: Do not use [Test] attribute for event handlers
        public void HandleCurrentProcessSetEvent(object sender, ComponentEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.Component);
            Assert.AreEqual(Resources.ProcessDefaultBaseName + " 1", args.Component.Name);
        }

        // handler for "statement added" event
        // Note: Do not use [Test] attribute for event handlers
        public void HandleStatementAddedEvent(object sender, StatementEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.Statement);
            Assert.AreEqual("Show Document " + Resources.DocumentDefaultBaseName + " 1", args.Statement.ToString());
            Assert.AreEqual(1, ((Process)Project.Current.CurrentComponent).Lines.Count);
        }

        // handler for "statement inserted" event
        // Note: Do not use [Test] attribute for event handlers
        public void HandleStatementInsertedEvent(object sender, StatementEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.Statement);
            Assert.AreEqual("Show Document " + Resources.DocumentDefaultBaseName + " 2", args.Statement.ToString());
        }

        // handler for "if statement added" event
        // Note: Do not use [Test] attribute for event handlers
        public void HandleIfStatementAddedEvent(object sender, StatementEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.Statement);

            string expString1 = "If First Name equals \"Steve\"";

            Assert.AreEqual(expString1, args.Statement.ToString());
        }

        // handler for "if statement inserted" event
        // Note: Do not use [Test] attribute for event handlers
        public void HandleIfStatementInsertedEvent(object sender, StatementEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.Statement);

            string expString1 = "If First Name equals \"Steve\"";

            Assert.AreEqual(expString1, args.Statement.ToString());
        }

        // handler for "statement removed" event
        // Note: Do not use [Test] attribute for event handlers
        public void HandleStatementRemovedEvent(object sender, StatementEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.Statement);
            Assert.AreEqual("Show Document " + Resources.DocumentDefaultBaseName + " 1", args.Statement.ToString());
        }

        // handler for "Send statement added" event
        // Note: Do not use [Test] attribute for event handlers
        public void HandleSendStatementAddedEvent(object sender, StatementEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.Statement);
            Assert.AreEqual("Send Email to \"doug@carlston.net\"", args.Statement.ToString());
            Assert.AreEqual("Here's some email body text.", ((SendEmailBody)((SendStatement)(args.Statement)).SendBody).Text);
        }

        // handler for "Send statement inserted" event
        // Note: Do not use [Test] attribute for event handlers
        public void HandleSendStatementInsertedEvent(object sender, StatementEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.Statement);
            Assert.AreEqual("Send Email to \"jdf@jdftech.com\"", args.Statement.ToString());
            Assert.AreEqual("Here's some different body text.", ((SendEmailBody)((SendStatement)(args.Statement)).SendBody).Text);
        }

        // handler for "Send statement removed" event
        // Note: Do not use [Test] attribute for event handlers
        public void HandleSendStatementRemovedEvent(object sender, StatementEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.Statement);
            Assert.AreEqual("Send Email to \"doug@carlston.net\"", args.Statement.ToString());
            Assert.AreEqual("Here's some email body text.", ((SendEmailBody)((SendStatement)(args.Statement)).SendBody).Text);
        }

        // handler for "Set statement added" event
        // Note: Do not use [Test] attribute for event handlers
        public void HandleSetStatementAddedEvent(object sender, StatementEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.Statement);
            Assert.AreEqual("Set a variable to \"a value\"", args.Statement.ToString());
        }

        //
        //	NOTE:	Writing Insert, Remove and Modify tests for the Set statement seems
        //			unnecessary. Once we've seen that a SetStatement can be added to
        //			the Lines list, we know that all other list functions should work
        //

        // handler for "Add statement added" event
        // Note: Do not use [Test] attribute for event handlers
        public void HandleAddStatementAddedEvent(object sender, StatementEventArgs args)
        {
            raiseCount++;
            Assert.IsNotNull(args.Statement);
            Assert.AreEqual("Add 2 to a variable", args.Statement.ToString());
        }

        private int themeCountChange;

        private void events_ThemeChanged(object sender, EventArgs e)
        {
            themeCountChange++;
        }

        [Test]
        public void AddAddStatement()
        {
            // create new process and set it to current
            Process process = Project.Current.AddProcess();
            Project.Current.SetCurrentComponent(process);

            // attach event handler to "statement added" event
            raiseCount = 0;
            Project.Events.StatementAdded += HandleAddStatementAddedEvent;

            // add Set statement
            var statement = new AddStatement();
            statement.Value.Text = "2";
            statement.Value.Type = FieldOrLiteral.StringType.literal;
            statement.Variable = "a variable";
            var line = new ArithmeticLine(statement);
            ((Process)Project.Current.CurrentComponent).Lines.Add(line);

            Assert.AreEqual(1, raiseCount);

            // detach event handler from "statement added" event
            Project.Events.StatementAdded -= HandleAddStatementAddedEvent;
        }

        [Test]
        public void AddExistingDocument()
        {
            string rtfString =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" +
                @"{\f0\fswiss\fcharset0\fprq2 Arial;}" +
                @"{\f1\froman\fcharset2\fprq2 Symbol;}}" +
                @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" +
                @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" +
                @"{\*\generator TX_RTF32 12.0.500.502;}" +
                @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind" +
                @"{\pard\itap0\sb2\sa2\plain\f0\fs20 Plain Text\par }";

            var rtfDocument = new RtfDocument("RTF Doc");
            rtfDocument.Rtf = rtfString;

            Project.Current.AddDocument(rtfDocument);

            Assert.AreEqual(1, Project.Current.DocumentList.Count);
            Assert.AreEqual("RTF Doc", ((RtfDocument)Project.Current.DocumentList[0]).Name);

            Assert.AreEqual(1, rtfDocument.Contents.Count);
            Assert.IsInstanceOfType(typeof(Paragraph), rtfDocument.Contents[0]);
            Assert.IsInstanceOfType(typeof(FontAttributes), ((Paragraph)rtfDocument.Contents[0]).Contents[0]);

            var paragraph = (Paragraph)rtfDocument.Contents[0];
            var fontAttributes = (FontAttributes)paragraph.Contents[0];

            foreach (IParagraphComponent component in fontAttributes.Contents)
            {
                Assert.AreEqual("Plain Text", component.Text);
            }
        }

        //
        //	NOTE:	Writing Insert, Remove and Modify tests for the Set statement seems
        //			unnecessary. Once we've seen that a SetStatement can be added to
        //			the Lines list, we know that all other list functions should work
        //
        //			Similarly, since Add, Subtract, Multiply and Divide statements are all
        //			derived from ArithmeticStatement, there is no need to further test
        //			the other derived classes.
        //

        [Test]
        public void AddFibFields()
        {
            IForm form = Project.Current.AddForm();
            Project.Current.SetCurrentComponent(form);

            form.ItemList.Add(new FibItem());
            var item1 = ((FibItem)form.ItemList[0]);
            item1.Text = "First Name: ____________________ Last Name: ____________________";

            Assert.AreEqual(2, item1.BlankList.Count);
            Assert.AreEqual("a", item1.BlankList.GetLabel(0));
            Assert.AreEqual("b", item1.BlankList.GetLabel(1));
        }

        [Test]
        public void AddForm()
        {
            // attach event handler to "form added" event
            Project.Events.ComponentAdded += handleFormAddedEvent;

            raiseCount = 0;

            IForm form = Project.Current.AddForm();

            Assert.AreEqual(1, raiseCount, "event raiseCount != 1");

            // detach event handler form "form added" event
            Project.Events.ComponentAdded -= handleFormAddedEvent;

            //Assertions 
            Assert.IsNotNull(form);
        }

        [Test]
        public void AddIfStatement()
        {
            // create new process and set it to current
            Process process = Project.Current.AddProcess();
            Project.Current.SetCurrentComponent(process);

            // attach event handler to "statement added" event
            raiseCount = 0;
            Project.Events.StatementAdded += HandleIfStatementAddedEvent;

            // create if list from if statement
            var firstName = new Field("First Name");
            ComparisonOperator equals = StringOperator.List[StringOperator.Ops.equals];
            var conditions = new Conditions(firstName, equals, new Expression("Steve"));
            var ifStatement = new IfStatement(conditions);
            var ifList = new ProcessLineList(ifStatement);

            // add if statement to project
            ((Process)Project.Current.CurrentComponent).Lines.Add(ifList);

            Assert.AreEqual(1, raiseCount);

            // detach event handler from "statement added" event
            Project.Events.StatementAdded -= HandleIfStatementAddedEvent;
        }

        [Test]
        public void AddItemToCurrentForm()
        {
            IForm form = Project.Current.AddForm();
            form.Name = "Test Form";
            Project.Current.SetCurrentComponent(form);

            var currentForm = Project.Current.CurrentComponent as Form;

            var item1 = new FormItem();
            item1.Text = "Content for text item #1.";
            currentForm.ItemList.Add(item1);

            var item2 = new FormItem();
            item2.Text = "Different content for text item #2.";
            currentForm.ItemList.Add(item2);

            //Assertions 
            Assert.AreEqual("Content for text item #1.", ((FormItem)currentForm.ItemList[0]).Text);
            Assert.AreEqual("Different content for text item #2.", ((FormItem)currentForm.ItemList[1]).Text);
        }

        [Test]
        public void AddOneDocument()
        {
            // attach event handler 1 to "document added" event
            raiseCount = 0;
            Project.Events.ComponentAdded += handleDocumentAddedEvent;

            // create new document and add to list
            Project.Current.AddDocument();

            Assert.AreEqual(1, raiseCount);
        }

        [Test]
        public void AddOneProcess()
        {
            // attach event handler 1 to "process added" event
            raiseCount = 0;
            Project.Events.ComponentAdded += HandleProcessAddedEvent;

            // create new process and add to list
            Project.Current.AddProcess();

            Assert.AreEqual(1, raiseCount);
        }

        [Test]
        public void AddSendEmailStatement()
        {
            // create new process and set it to current
            Process process = Project.Current.AddProcess();
            Project.Current.SetCurrentComponent(process);

            // attach event handler to "statement added" event
            raiseCount = 0;
            Project.Events.StatementAdded += HandleSendStatementAddedEvent;

            // add Send statement
            var statement = new SendStatement();
            statement.AddressTo.Text = "doug@carlston.net";
            statement.SendBody = new SendEmailBody("Here's some email body text.");
            var line = new SendLine(statement);
            ((Process)Project.Current.CurrentComponent).Lines.Add(line);

            Assert.AreEqual(1, raiseCount);

            // detach event handler from "statement added" event
            Project.Events.StatementAdded -= HandleSendStatementAddedEvent;
        }

        [Test]
        public void AddSetStatement()
        {
            // create new process and set it to current
            Process process = Project.Current.AddProcess();
            Project.Current.SetCurrentComponent(process);

            // attach event handler to "statement added" event
            raiseCount = 0;
            Project.Events.StatementAdded += HandleSetStatementAddedEvent;

            // add Set statement
            var statement = new SetStatement();
            statement.Variable = new Variable("a variable");
            statement.Expression = new Expression("a value");
            var line = new SetLine(statement);
            ((Process)Project.Current.CurrentComponent).Lines.Add(line);

            Assert.AreEqual(1, raiseCount);

            // detach event handler from "statement added" event
            Project.Events.StatementAdded -= HandleSetStatementAddedEvent;
        }

        [Test]
        public void AddShowLine()
        {
            // create new process and set it to current
            Process process = Project.Current.AddProcess();
            Project.Current.SetCurrentComponent(process);

            // attach event handler to "statement added" event
            raiseCount = 0;
            Project.Events.StatementAdded += HandleStatementAddedEvent;

            // add show line
            IDocument document1 = Project.Current.AddDocument();
            ShowStatement statement = new ShowDocumentStatement(document1);
            var list = new ProcessLineList(statement);
            ((Process)Project.Current.CurrentComponent).Lines.Add(list);

            Assert.AreEqual(1, raiseCount);

            // detach event handler from "statement added" event
            Project.Events.StatementAdded -= HandleStatementAddedEvent;
        }

        [Test]
        public void AddShowStatement()
        {
            // create new process and set it to current
            Process process = Project.Current.AddProcess();
            Project.Current.SetCurrentComponent(process);

            // attach event handler to "statement added" event
            raiseCount = 0;
            Project.Events.StatementAdded += HandleStatementAddedEvent;

            // add show statement
            IDocument document1 = Project.Current.AddDocument();
            var statement = new ShowDocumentStatement(document1);
            ShowLine line = new ShowDocumentLine(statement);
            ((Process)Project.Current.CurrentComponent).Lines.Add(line);

            Assert.AreEqual(1, raiseCount);

            // detach event handler from "statement added" event
            Project.Events.StatementAdded -= HandleStatementAddedEvent;
        }

        [Test]
        public void AllVariablesListTest()
        {
            Assert.AreEqual(1, Project.Current.AllVariables.Count); // "_InviteeID" is always present

            Process process = Project.Current.AddProcess();
            process.Variables.AddUnique("Variable 1");
            process.Variables.AddUnique("Variable 2");
            process.Variables.AddUnique("Variable 3");

            Assert.AreEqual(4, Project.Current.AllVariables.Count);

            Process process2 = Project.Current.AddProcess();
            process2.Variables.AddUnique("Variable 2");
            process2.Variables.AddUnique("Variable 4");

            Assert.AreEqual(5, Project.Current.AllVariables.Count);
            Assert.AreEqual("Variable 1", Project.Current.AllVariables[0].FieldName);
            Assert.AreEqual("Variable 2", Project.Current.AllVariables[1].FieldName);
            Assert.AreEqual("Variable 3", Project.Current.AllVariables[2].FieldName);
            Assert.AreEqual("Variable 4", Project.Current.AllVariables[3].FieldName);

            // Note: the AllVariables list is compiled from the Processes' variables lists;
            //		 you should not be able to add to it directly
            Project.Current.AllVariables.Add(new Variable("foo"));
            Assert.AreEqual(5, Project.Current.AllVariables.Count);
        }

        [Test]
        public void ChangingThemeRaisesEvent()
        {
            Project.Events.ThemeChanged += events_ThemeChanged;

            Assert.AreEqual(0, themeCountChange);

            Project.Current.ThemePath = "Test3141";

            Assert.AreEqual(1, themeCountChange);
        }

        [Test]
        public void ConnectProcessToForm()
        {
            IForm form1 = Project.Current.AddForm();
            IForm form2 = Project.Current.AddForm();

            var process = new Process("Connected Process");

            // connect the Process to Form 2
            Project.Current.ConnectProcessToForm(process, form2.Name);

            //Assertions 
            Assert.AreEqual(null, form1.ConnectedPostProcess);
            Assert.AreEqual("Connected Process", form2.ConnectedPostProcess.Name);

            // now disconnect the Process from Form 2
            Project.Current.DisconnectProcessFromForm(form2.Name);

            //Assertions 
            Assert.AreEqual(null, form1.ConnectedPostProcess);
            Assert.AreEqual(null, form2.ConnectedPostProcess);
        }

        [Test]
        public void ConstructFromXml()
        {
            string xmlString =
                "<project name=\"Hello World\" themePath=\"style2\" format=\"" + Project.XmlFormatVersion + "\">\r\n" +
                "</project>\r\n";

            IXmlElement element = new XmlElement(xmlString);
            Project.New(element);

            Assert.AreEqual("Hello World", Project.Current.Name);
            Assert.AreEqual("style2", Project.Current.ThemePath);
        }

        [Test]
        public void ConstructFromXmlNoTheme()
        {
            string xmlString =
                "<project name=\"Hello World\" format=\"" + Project.XmlFormatVersion + "\">\r\n" +
                "</project>\r\n";

            IXmlElement element = new XmlElement(xmlString);
            Project.New(element);

            Assert.AreEqual("Hello World", Project.Current.Name);
            Assert.AreEqual("default", Project.Current.ThemePath);
        }

        [Test]
        public void CreateFibItems()
        {
            IForm form = Project.Current.AddForm();
            form.Name = "Test Form";
            Project.Current.SetCurrentComponent(form);

            // Assertion
            Assert.AreEqual(form, Project.Current.CurrentComponent);

            // add our event handler to the Project delegate
            raiseCount = 0;
            Project.Events.FormItemAdded += HandleNewFibItemEvent;

            form.ItemList.Add(new FibItem());

            var item1 = ((FibItem)form.ItemList[0]);

//			Assert.AreEqual(Project.Resources.GetString("FibItemDefaultRtf"), item1.Rtf);

            form.ItemList.Insert(1, new FibItem());
            var item2 = ((FibItem)form.ItemList[1]);
            item2.Text = "Blank 2: ____";

            form.ItemList.Insert(1, new FibItem());
            var item3 = ((FibItem)form.ItemList[1]);
            item3.Text = "Blank 3: ____";

            var relocatedItem = ((FibItem)form.ItemList[2]);

            //Assertion
            Assert.AreEqual("Blank 2: ____", relocatedItem.Text);
        }

        [Test]
        public void CreateMCItems()
        {
            IForm form = Project.Current.AddForm();
            form.Name = "Test Form";
            Project.Current.SetCurrentComponent(form);

            // add our event handler to the Project delegate
            raiseCount = 0;
            Project.Events.FormItemAdded += HandleNewMCItemEvent;

            form.ItemList.Add(new McqItem());

            var item1 = ((McqItem)form.ItemList[0]);

            //Assertion
            Assert.AreEqual(Project.Current.CurrentComponent.Name, "Test Form");
            Assert.AreEqual(Resources.MCItemDefaultText, item1.Text);
            Assert.AreEqual(1, item1.Choices.Count);
            Assert.AreEqual(1, raiseCount);
        }

        [Test]
        public void CreateTextFormItems()
        {
            IForm form = Project.Current.AddForm();
            form.Name = "Test Form";
            Project.Current.SetCurrentComponent(form);

            // add our event handler to the Project delegate
            raiseCount = 0;
            Project.Events.FormItemAdded += HandleNewTextItemEvent;

            raiseCount = 0;
            form.ItemList.Add(new TextItem());
            Assert.AreEqual(1, raiseCount);
            var item1 = ((TextItem)form.ItemList[0]);

            Assert.AreEqual(RtfConstants.TextItemDefaultRTF, item1.Rtf);

            form.ItemList.Add(new TextItem());
            Assert.AreEqual(2, raiseCount);
            var item2 = ((TextItem)form.ItemList[1]);
            item2.Text = "Text item 2.";

            form.ItemList.Insert(1, new TextItem());
            Assert.AreEqual(3, raiseCount);
            var item3 = ((TextItem)form.ItemList[1]);
            item3.Text = "Text item 3.";

            var relocatedItem = ((TextItem)form.ItemList[2]);

            //Assertion
            Assert.AreEqual("Text item 2.", relocatedItem.Text);
        }

        [Test]
        public void DocumentList()
        {
            Project.Current.AddDocument();
            Project.Current.AddDocument();

            IList<IDocument> list = Project.Current.DocumentList;

            // Assertions
            Assert.IsTrue(list.IsReadOnly);
            Assert.IsTrue(list.Count == 2);
            Assert.AreEqual(Resources.DocumentDefaultBaseName + " 1", list[0].ToString());
            Assert.AreEqual(Resources.DocumentDefaultBaseName + " 2", list[1].ToString());

            // Verify that read-only wrapper truely is a wrapper and thus should reflect changes

            Project.Current.AddDocument();

            // Assertions
            Assert.IsTrue(list.Count == 3);
            Assert.AreEqual(Resources.DocumentDefaultBaseName + " 1", list[0].ToString());
            Assert.AreEqual(Resources.DocumentDefaultBaseName + " 2", list[1].ToString());
            Assert.AreEqual(Resources.DocumentDefaultBaseName + " 3", list[2].ToString());
        }

        [Test]
        public void FindAlternateLabeledBlankByName()
        {
            IForm form1 = Project.Current.AddForm();
            var fibItem1 = new FibItem();
            form1.ItemList.Add(fibItem1);

            fibItem1.BlankList[0].AlternateLabel = "Name";

            IField searchField = Project.FieldMapById.FindField("Form 1:Name");
            Assert.AreSame(fibItem1.BlankList[0], searchField);
        }

        [Test]
        public void FindAlternateLabeledMCQByName()
        {
            IForm form1 = Project.Current.AddForm();
            var mcItem1 = new McqItem();
            form1.ItemList.Add(mcItem1);

            mcItem1.AlternateLabel = "Pick one";

            IField searchField = Project.FieldMapById.FindField("Pick one");
            Assert.AreSame(mcItem1, searchField);
        }

        [Test]
        public void FindBlanksByName()
        {
            IForm form1 = Project.Current.AddForm();
            var fibItem1 = new FibItem();
            form1.ItemList.Add(fibItem1);

            IPaletteField field = fibItem1.BlankList[0];
            Assert.AreEqual("Form 1:Q1:a", field.QualifiedFieldName);

            IForm form2 = Project.Current.AddForm();
            var fibItem2 = new FibItem();
            form2.ItemList.Add(fibItem2);

            field = fibItem2.BlankList[0];
            Assert.AreEqual("Form 2:Q1:a", field.QualifiedFieldName);

            IField searchField = Project.FieldMapById.FindField("Form 1:Q1:a");
            Assert.AreSame(fibItem1.BlankList[0], searchField);

            searchField = Project.FieldMapById.FindField("Form 2:Q1:a");
            Assert.AreSame(fibItem2.BlankList[0], searchField);
        }

        [Test]
        public void FindMCQsByName()
        {
            IForm form1 = Project.Current.AddForm();
            var mcItem1 = new McqItem();
            form1.ItemList.Add(mcItem1);

            IPaletteField field = mcItem1;
            Assert.AreEqual("Form 1:Q1", field.QualifiedFieldName);

            IForm form2 = Project.Current.AddForm();
            var mcItem2 = new McqItem();
            form2.ItemList.Add(mcItem2);

            field = mcItem2;
            Assert.AreEqual("Form 2:Q1", field.QualifiedFieldName);

            IField searchField = Project.FieldMapById.FindField("Form 1:Q1");
            Assert.AreSame(mcItem1, searchField);

            searchField = Project.FieldMapById.FindField("Form 2:Q1");
            Assert.AreSame(mcItem2, searchField);
        }

        [Test]
        public void FindVariableByName()
        {
            var var1 = new Variable("variable");

            IField searchField = Project.FieldMapById.FindField("variable");
            Assert.AreSame(var1, searchField);
        }

        [Test]
        public void FormList()
        {
            Project.Current.AddForm();
            Project.Current.AddForm();

            IList<IForm> list = Project.Current.FormList;

            // Assertions
            Assert.IsTrue(list.IsReadOnly);
            Assert.IsTrue(list.Count == 2);
            Assert.AreEqual(Resources.FormDefaultBaseName + " 1", (list[0]).Name);
            Assert.AreEqual(Resources.FormDefaultBaseName + " 2", (list[1]).Name);

            // Verify that read-only wrapper truely is a wrapper and thus should reflect changes

            Project.Current.AddForm();

            // Assertions
            Assert.IsTrue(list.Count == 3);
            Assert.AreEqual(Resources.FormDefaultBaseName + " 1", (list[0]).Name);
            Assert.AreEqual(Resources.FormDefaultBaseName + " 2", (list[1]).Name);
            Assert.AreEqual(Resources.FormDefaultBaseName + " 3", (list[2]).Name);
        }

        [Test]
        public void GetDefaultLabel()
        {
            // add 2 forms to project
            IForm form1 = Project.Current.AddForm();
            IForm form2 = Project.Current.AddForm();

            var fibItem1 = new FibItem();
            var mcItem1 = new McqItem();
            var textItem1 = new TextItem();

            var fibItem2 = new FibItem();
            var mcItem2 = new McqItem();
            var textItem2 = new TextItem();

            // add items to both forms
            form1.ItemList.Add(fibItem1);
            form1.ItemList.Add(mcItem1);
            form1.ItemList.Add(textItem1);

            form2.ItemList.Add(fibItem2);
            form2.ItemList.Add(mcItem2);
            form2.ItemList.Add(textItem2);

            Assert.AreEqual("Q1", Project.Current.GetDefaultLabel(fibItem1));
            Assert.AreEqual("Q2", Project.Current.GetDefaultLabel(mcItem1));
            Assert.AreEqual("T1", Project.Current.GetDefaultLabel(textItem1));
            Assert.AreEqual("Q1", Project.Current.GetDefaultLabel(fibItem2));
            Assert.AreEqual("Q2", Project.Current.GetDefaultLabel(mcItem2));
            Assert.AreEqual("T1", Project.Current.GetDefaultLabel(textItem2));
        }

        [Test]
        public void GetDocument()
        {
            Project.Current.AddDocument();

            // Get the process
            IDocument doc1 = Project.Current.GetDocument("Nonexistent Document");
            IDocument doc2 = Project.Current.GetDocument(Resources.DocumentDefaultBaseName + " 1");

            // Assertions
            Assert.AreEqual(Document.NULL, doc1);
            Assert.AreEqual(Resources.DocumentDefaultBaseName + " 1", doc2.Name);
        }

        [Test]
        public void GetEmptyImagesXml()
        {
            Project.Images.AddUnique(new GraphicImageDefinition());

            string expString =
                "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" +
                "<request type=\"uploadProject\" protocol=\"1.0\">\r\n" +
                credentialsXml +
                "<project name=\"" + Project.Current.Name + "\" themePath=\"default\" format=\"" + Project.XmlFormatVersion +
                "\" designerBuild=\"0\">\r\n" +
                "<pageHeader></pageHeader>" +
                "</project>\r\n" +
                "</request>\r\n";

            Assert.AreEqual(expString, Project.Current.ToXmlForUpload(credentialsXml));
        }

        [Test]
        public void GetForm()
        {
            // create new processes and add to list
            Project.Current.AddForm();

            // Get the process
            IForm form1 = Project.Current.GetForm("Nonexistent Form");
            IForm form2 = Project.Current.GetForm(Resources.FormDefaultBaseName + " 1");

            // Assertions
            Assert.AreEqual(null, form1);
            Assert.AreEqual(Resources.FormDefaultBaseName + " 1", form2.Name);
        }

        [Test]
        public void GetFormListFromProcess()
        {
            // add 2 forms to project
            IForm form1 = Project.Current.AddForm();
            IForm form2 = Project.Current.AddForm();

            // create process and connect to both forms
            var process = new Process("Connected Process");

            Project.Current.ConnectProcessToForm(process, form1.Name);
            Project.Current.ConnectProcessToForm(process, form2.Name);

            Assert.AreEqual("Connected Process", form1.ConnectedPostProcess.Name);
            Assert.AreEqual("Connected Process", form2.ConnectedPostProcess.Name);

            // generate list of forms to which process is connected
            FormList list = Project.Current.GetFormList(process);

            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public void GetFormName()
        {
            IForm form1 = Project.Current.AddForm();
            IForm form2 = Project.Current.AddForm();
            string name = form2.Name;

            //Assertions 
            Assert.AreEqual(Resources.FormDefaultBaseName + " 2", name);
        }

        [Test]
        public void GetProcess()
        {
            Project.Current.AddProcess();

            // Get the process
            Process proc1 = Project.Current.GetProcess("Nonexistent Process");
            Process proc2 = Project.Current.GetProcess(Resources.ProcessDefaultBaseName + " 1");

            // Assertions
            Assert.AreEqual(Process.NULL, proc1);
            Assert.AreEqual(Resources.ProcessDefaultBaseName + " 1", proc2.Name);
        }

        [Test]
        public void GetProcessFromStatement()
        {
            Process process1 = Project.Current.AddProcess();
            Process process2 = Project.Current.AddProcess();

            // add show statements referencing documents to process
            IDocument document1 = Project.Current.AddDocument();
            var statement1 = new SendStatement();
            var line1 = new SendLine(statement1);

            IDocument document2 = Project.Current.AddDocument();
            var statement2 = new SendStatement();
            var line2 = new SendLine(statement2);

            Project.Current.SetCurrentComponent(process1);
            ((Process)Project.Current.CurrentComponent).Lines.Add(line1);

            Project.Current.SetCurrentComponent(process2);
            ((Process)Project.Current.CurrentComponent).Lines.Add(line2);

            //Assertions 
            Assert.AreEqual(process1, Project.Current.GetProcessOrSkipInstructions(statement1));
            Assert.AreEqual(process2, Project.Current.GetProcessOrSkipInstructions(statement2));
        }

        [Test]
        public void GetRealOrVirtualDocument()
        {
            IDocument realDoc = Project.Current.AddDocument();
            realDoc.Name = "Real Doc";

            // retrieve it using GetRealOrVirtualDocument()
            IDocument fetchedDoc = Project.Current.GetRealOrVirtualDocument("Real Doc", false);
            Assert.AreEqual(realDoc, fetchedDoc, "fetchedDoc should equal realDoc");

            // try to fetch a non-existent document by name
            fetchedDoc = Project.Current.GetRealOrVirtualDocument("Test Doc", false);
            Assert.IsNull(fetchedDoc, "attempt to fetch Test Doc should be null");

            // setting the second arg to true causes the virtual document to be created
            fetchedDoc = Project.Current.GetRealOrVirtualDocument("Test Doc", true);
            Assert.IsNotNull(fetchedDoc, "Fetched virtual document should not be null");
        }

        [Test]
        public void GetSkipInstructionsFromStatement()
        {
            // add two Forms
            IForm form1 = Project.Current.AddForm();
            IForm form2 = Project.Current.AddForm();

            // add Skip Instruction items to the Forms
            var skip1 = new SkipInstructionsItem();
            var skip2 = new SkipInstructionsItem();
            var skip3 = new SkipInstructionsItem();

            form1.ItemList.Add(skip1);
            form1.ItemList.Add(skip2);
            form2.ItemList.Add(skip3);

            // create some statements and put them in the Skip Instructions
            var if1 = new IfStatement();
            var if2 = new IfStatement();
            var set1 = new SetStatement();
            var set2 = new SetStatement();

            skip1.Instructions.Lines.Add(new ProcessLineList(if1));
            skip1.Instructions.Lines.Add(new ProcessLineList(set1));
            skip2.Instructions.Lines.Add(new ProcessLineList(if2));
            skip3.Instructions.Lines.Add(new ProcessLineList(set2));

            Assert.AreEqual(skip1.Instructions, Project.Current.GetProcessOrSkipInstructions(if1));
            Assert.AreEqual(skip1.Instructions, Project.Current.GetProcessOrSkipInstructions(set1));
            Assert.AreEqual(skip2.Instructions, Project.Current.GetProcessOrSkipInstructions(if2));
            Assert.AreEqual(skip3.Instructions, Project.Current.GetProcessOrSkipInstructions(set2));
        }

        [Test]
        public void GetStringFromResourceManager()
        {
            Assert.AreEqual(Resources.ProjectDefaultName, Project.Current.Name);
        }

        [Test]
        public void GetXml()
        {
            Project.Current.Name = "Hello World";

            // add and name form
            IForm form1 = Project.Current.AddForm();
            string oldFormname = form1.Name;
            Project.Current.RenameForm(oldFormname, "Hello");

            // add text item to form
            var item = new TextItem();
            item.Text = "Hello, World!";
            form1.ItemList.Add(item);

            // add process and make current
            Process proc1 = Project.Current.AddProcess();
            Project.Current.SetCurrentComponent(proc1);

            // add document with text
            IDocument doc1 = Project.Current.AddDocument();
            string htmlContent1 =
                Document.RawHtmlPrefix +
                "<p><span style=\"font-size:10pt;\">This is the text of Document 1.</span></p>" +
                Document.RawHtmlPostfix;
            doc1.Text = htmlContent1;

            // add show line to process
            //Document document1 = new Document("Document 1");
            var st1 = new ShowDocumentStatement(doc1);
            ProcessLine line1 = new ShowDocumentLine(st1);
            proc1.Lines.Add(line1);

            // create if statement
            var firstName = new Field("First Name");
            ComparisonOperator equals = StringOperator.List[StringOperator.Ops.equals];
            var conditions = new Conditions(firstName, equals, new Expression("Steve"));

            var st2 = new IfStatement(conditions);

            // add if line list to process
            var ifLines = new ProcessLineList(st2);
            proc1.Lines.Add(ifLines);

            // add a Send Email line to the Process
            var st3 = new SendStatement();
            st3.AddressTo.Text = "doug@carlston.net";
            st3.AddressCc.Text = "jdf@jdftech.com";
            st3.Subject = "Testing the Send command";
            st3.SendBody = new SendEmailBody("Hi Doug, can you read this?");

            ProcessLine sendLine = new SendLine(st3);
            proc1.Lines.Add(sendLine);

            // add a Set line to the Process
            var st4 = new SetStatement();
            st4.Variable = new Variable("a variable");
            st4.Expression = new Expression("a value");

            proc1.Lines.Add(new SetLine(st4));

            // add a second Form
            IForm form2 = Project.Current.AddForm();
            form2.Name = "Second Form";

            string expCredentials = GlobalSettings.CredentialsElement("TestUserName", "TestUserPW");

            string expString =
                "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" +
                "<request type=\"uploadProject\" protocol=\"1.0\">\r\n" +
                expCredentials +
                "<project name=\"" + Project.Current.Name + "\" themePath=\"default\" format=\"" + Project.XmlFormatVersion +
                "\" designerBuild=\"0\">\r\n" +
                "<pageHeader></pageHeader>" +
                "<forms>\r\n" +
                "<form name=\"Hello\" startPoint=\"true\" themePath=\"default\" blockBackButton=\"false\">\r\n" +
                "<items>\r\n" +
                "<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "Hello, World!" +
                "</paragraph>" +
                "</text>\r\n" +
                "</items>\r\n" +
                "</form>\r\n" +
                "<form name=\"Second Form\" startPoint=\"false\" themePath=\"default\" blockBackButton=\"false\">\r\n" +
                "</form>\r\n" +
                "</forms>\r\n" +
                "<processes>\r\n" +
                "<process name=\"" + Resources.ProcessDefaultBaseName + " 1\">\r\n" +
                "<show document=\"Document 1\" reset=\"false\"/>\r\n" +
                "<if>\r\n" +
                "<conditions>\r\n" +
                "<equals field=\"First Name\">\r\n" +
                "<string value=\"Steve\"/>\r\n" +
                "</equals>\r\n" +
                "</conditions>\r\n" +
                "<trueSet>\r\n" +
                "</trueSet>\r\n" +
                "</if>\r\n" +
                "<send>\r\n" +
                "<to addressLiteral=\"doug@carlston.net\"/>\r\n" +
                "<cc addressLiteral=\"jdf@jdftech.com\"/>\r\n" +
                "<subject>Testing the Send command</subject>\r\n" +
                "<body>Hi Doug, can you read this?</body>\r\n" +
                "</send>\r\n" +
                "<set field=\"a variable\" arithmeticAsText=\"false\">\r\n" +
                "<string value=\"a value\"/>\r\n" +
                "</set>\r\n" +
                "</process>\r\n" +
                "</processes>\r\n" +
                "<documents>\r\n" +
                "<document name=\"" + Resources.DocumentDefaultBaseName + " 1\">\r\n" +
                "<xmlData>\r\n" +
                "\r\n" +
                "</xmlData>\r\n" +
                "</document>\r\n" +
                "</documents>\r\n" +
                "</project>\r\n" +
                "</request>\r\n";

            string xmlString = Project.Current.ToXmlForUpload(expCredentials);

            //Assertion
            Assert.AreEqual(expString, xmlString);

            // connect the Process to Form 2
            Project.Current.ConnectProcessToForm(proc1, form2.Name);

            string expStringWithFormProcessConnection =
                "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" +
                "<request type=\"uploadProject\" protocol=\"1.0\">\r\n" +
                expCredentials +
                "<project name=\"" + Project.Current.Name + "\" themePath=\"default\" format=\"" + Project.XmlFormatVersion +
                "\" designerBuild=\"0\">\r\n" +
                "<pageHeader></pageHeader>" +
                "<forms>\r\n" +
                "<form name=\"Hello\" startPoint=\"true\" themePath=\"default\" blockBackButton=\"false\">\r\n" +
                "<items>\r\n" +
                "<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
                "<paragraph indent=\"0\" align=\"left\">" +
                "Hello, World!" +
                "</paragraph>" +
                "</text>\r\n" +
                "</items>\r\n" +
                "</form>\r\n" +
                "<form name=\"Second Form\" startPoint=\"false\" themePath=\"default\" process=\"" + Resources.ProcessDefaultBaseName +
                " 1\" blockBackButton=\"false\">\r\n" +
                "</form>\r\n" +
                "</forms>\r\n" +
                "<processes>\r\n" +
                "<process name=\"" + Resources.ProcessDefaultBaseName + " 1\">\r\n" +
                "<show document=\"Document 1\" reset=\"false\"/>\r\n" +
                "<if>\r\n" +
                "<conditions>\r\n" +
                "<equals field=\"First Name\">\r\n" +
                "<string value=\"Steve\"/>\r\n" +
                "</equals>\r\n" +
                "</conditions>\r\n" +
                "<trueSet>\r\n" +
                "</trueSet>\r\n" +
                "</if>\r\n" +
                "<send>\r\n" +
                "<to addressLiteral=\"doug@carlston.net\"/>\r\n" +
                "<cc addressLiteral=\"jdf@jdftech.com\"/>\r\n" +
                "<subject>Testing the Send command</subject>\r\n" +
                "<body>Hi Doug, can you read this?</body>\r\n" +
                "</send>\r\n" +
                "<set field=\"a variable\" arithmeticAsText=\"false\">\r\n" +
                "<string value=\"a value\"/>\r\n" +
                "</set>\r\n" +
                "</process>\r\n" +
                "</processes>\r\n" +
                "<documents>\r\n" +
                "<document name=\"" + Resources.DocumentDefaultBaseName + " 1\">\r\n" +
                "<xmlData>\r\n" +
                "\r\n" +
                "</xmlData>\r\n" +
                "</document>\r\n" +
                "</documents>\r\n" +
                "</project>\r\n" +
                "</request>\r\n";

            //Assertion
            Assert.AreEqual(expStringWithFormProcessConnection, Project.Current.ToXmlForUpload(expCredentials));

            // now disconnect the Process from Form 2
            Project.Current.DisconnectProcessFromForm(form2.Name);

            //Assertion
            Assert.AreEqual(expString, Project.Current.ToXmlForUpload(expCredentials));
        }

        //[Test]
        //public void GetImageXml()
        //{
        //    string xmlString =
        //        "<image>" +
        //        "<metafileHeader fileType=\"1\" headerSize=\"9\" version=\"768\" fileSize=\"827\" numOfObjects=\"2\" maxRecordSize=\"517\" numOfParams=\"0\" />\r\n" +
        //        "<metafileRecord size=\"5\" function=\"263\">03000000</metafileRecord>\r\n" +
        //        "<metafileRecord size=\"517\" function=\"247\">00030001000000000d0d0d001a1a1a00282828003535350043434300505050005d5d5d006b6b6b00787878008686860093939300a1a1a100aeaeae00bbbbbb00c9c9c900d6d6d600e4e4e400f1f1f100ffffff00000000000000330000006600000099000000cc000000ff00003300000033330000336600003399000033cc000033ff00006600000066330000666600006699000066cc000066ff00009900000099330000996600009999000099cc000099ff0000cc000000cc330000cc660000cc990000cccc0000ccff0000ff000000ff330000ff660000ff990000ffcc0000ffff00330000003300330033006600330099003300cc003300ff00333300003333330033336600333399003333cc003333ff00336600003366330033666600336699003366cc003366ff00339900003399330033996600339999003399cc003399ff0033cc000033cc330033cc660033cc990033cccc0033ccff0033ff000033ff330033ff660033ff990033ffcc0033ffff00660000006600330066006600660099006600cc006600ff00663300006633330066336600663399006633cc006633ff00666600006666330066666600666699006666cc006666ff00669900006699330066996600669999006699cc006699ff0066cc000066cc330066cc660066cc990066cccc0066ccff0066ff000066ff330066ff660066ff990066ffcc0066ffff00990000009900330099006600990099009900cc009900ff00993300009933330099336600993399009933cc009933ff00996600009966330099666600996699009966cc009966ff00999900009999330099996600999999009999cc009999ff0099cc000099cc330099cc660099cc990099cccc0099ccff0099ff000099ff330099ff660099ff990099ffcc0099ffff00cc000000cc003300cc006600cc009900cc00cc00cc00ff00cc330000cc333300cc336600cc339900cc33cc00cc33ff00cc660000cc663300cc666600cc669900cc66cc00cc66ff00cc990000cc993300cc996600cc999900cc99cc00cc99ff00cccc0000cccc3300cccc6600cccc9900cccccc00ccccff00ccff0000ccff3300ccff6600ccff9900ccffcc00ccffff00ff000000ff003300ff006600ff009900ff00cc00ff00ff00ff330000ff333300ff336600ff339900ff33cc00ff33ff00ff660000ff663300ff666600ff669900ff66cc00ff66ff00ff990000ff993300ff996600ff999900ff99cc00ff99ff00ffcc0000ffcc3300ffcc6600ffcc9900ffcccc00ffccff00ffff0000ffff3300ffff6600ffff9900ffffcc00ffffff000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000</metafileRecord>\r\n" +
        //        "<metafileRecord size=\"4\" function=\"564\">0000</metafileRecord>\r\n" +
        //        "<metafileRecord size=\"3\" function=\"53\"></metafileRecord>\r\n" +
        //        "<metafileRecord size=\"5\" function=\"523\">00000000</metafileRecord>\r\n" +
        //        "<metafileRecord size=\"5\" function=\"524\">0c000a00</metafileRecord>\r\n" +
        //        "<metafileRecord size=\"223\" function=\"2368\">2000cc00000000000c000a0000000000280000000a0000000c000000010018000000000080010000000000000000000000000000000000003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff00003d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff3d7eff0000</metafileRecord>\r\n" +
        //        "<metafileRecord size=\"45\" function=\"247\">0003140000000000800000000080000080800000000080008000800000808000c0c0c000c0dcc000a6caf000fffbf000a0a0a40080808000ff00000000ff0000ffff00000000ff00ff00ff0000ffff00ffffff00</metafileRecord>\r\n" +
        //        "<metafileRecord size=\"4\" function=\"564\">0100</metafileRecord>\r\n" +
        //        "<metafileRecord size=\"4\" function=\"496\">0000</metafileRecord>\r\n" +
        //        "<metafileRecord size=\"3\" function=\"0\"></metafileRecord>\r\n" +
        //        "</image>";

        //    IXmlElement element = new XmlElement(xmlString);
        //    GraphicImage image = new GraphicImage(element);

        //    string expString =
        //        "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" +
        //        "<request type=\"uploadProject\" protocol=\"1.0\">\r\n" +
        //        credentialsXml +
        //        "<project name=\"" + Project.Current.Name + "\" themePath=\"default\" format=\"" + Project.XmlFormatVersion + "\">\r\n" +
        //        "<images>" +
        //        "<imagedef id=\"image1\">" +
        //        "<imagedata imageFormat=\"PNG\">" +
        //        "iVBORw0KGgoAAAANSUhEUgAAAAoAAAAMCAIAAADUCbv3AAAAAXNSR0IArs4c6QAAAARnQU1BAACx\r\n" +
        //        "jwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAABpJREFU\r\n" +
        //        "KFNj/F9ny4AHAKXxIIZRaWzhQyBYAFsXv6i+LNJ8AAAAAElFTkSuQmCC" +
        //        "</imagedata>" +
        //        "</imagedef>" +
        //        "</images>" +
        //        "</project>\r\n" +
        //        "</request>\r\n";

        //    Assert.AreEqual(expString, Project.Current.ToXmlForUpload(credentialsXml));
        //}

        [Test]
        public void GetXmlForProjectName()
        {
            // check for illegal XML characters in the Project name
            Project.Current.Name = "Them's <bad> &\"characters\"";

            string expString = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" +
                               "<request type=\"uploadProject\" protocol=\"1.0\">\r\n" +
                               credentialsXml +
                               "<project name=\"Them&apos;s &lt;bad&gt; &amp;&quot;characters&quot;\" themePath=\"default\" format=\"" +
                               Project.XmlFormatVersion + "\" designerBuild=\"0\">\r\n" +
                               "<pageHeader></pageHeader>" +
                               "</project>\r\n" +
                               "</request>\r\n";

            //Assertions 
            Assert.AreEqual(expString, Project.Current.ToXmlForUpload(credentialsXml));
        }

        [Test]
        public void GetXmlProjectTheme()
        {
            Project.Current.AddForm();

            Project.Current.ThemePath = "new theme";

            string expString = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" +
                               "<request type=\"uploadProject\" protocol=\"1.0\">\r\n" +
                               credentialsXml +
                               "<project name=\"" + Project.Current.Name + "\" themePath=\"new theme\" format=\"" + Project.XmlFormatVersion +
                               "\" designerBuild=\"0\">\r\n" +
                               "<pageHeader></pageHeader>" +
                               "<forms>\r\n" +
                               "<form name=\"Form 1\" startPoint=\"true\" themePath=\"new theme\" blockBackButton=\"false\">\r\n" +
                               "</form>\r\n" +
                               "</forms>\r\n" +
                               "</project>\r\n" +
                               "</request>\r\n";

            Assert.AreEqual(expString, Project.Current.ToXmlForUpload(credentialsXml));
        }

        [Test]
        public void GetXmlWhenEmpty()
        {
            string expString = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" +
                               "<request type=\"uploadProject\" protocol=\"1.0\">\r\n" +
                               credentialsXml +
                               "<project name=\"" + Project.Current.Name + "\" themePath=\"default\" format=\"" + Project.XmlFormatVersion +
                               "\" designerBuild=\"0\">\r\n" +
                               "<pageHeader></pageHeader>" +
                               "</project>\r\n" +
                               "</request>\r\n";

            //Assertions 
            Assert.AreEqual(expString, Project.Current.ToXmlForUpload(credentialsXml));
        }

        [Test]
        public void Images()
        {
            Project.Images.AddUnique(new GraphicImageDefinition());

            Assert.AreEqual("image1", Project.Images[0].Id);
        }

        [Test]
        public void InsertIfStatement()
        {
            // create new process and set it to current
            Process process = Project.Current.AddProcess();
            Project.Current.SetCurrentComponent(process);

            // create show statement and add to current process
            IDocument doc2 = ComponentMaker.MakeDocumentObject("Document 2");
            var showStatement2 = new ShowDocumentStatement(doc2);
            ShowLine line2 = new ShowDocumentLine(showStatement2);
            ((Process)Project.Current.CurrentComponent).Lines.Add(line2);

            // create if statement
            var firstName = new Field("First Name");
            ComparisonOperator equals = StringOperator.List[StringOperator.Ops.equals];
            var conditions = new Conditions(firstName, equals, new Expression("Steve"));
            var ifStatement = new IfStatement(conditions);
            var ifList = new ProcessLineList(ifStatement);

            // attach event handler to "statement inserted" event
            raiseCount = 0;
            Project.Events.StatementAdded += HandleIfStatementInsertedEvent;

            // insert if statement into current process
            ((Process)Project.Current.CurrentComponent).Lines.Insert(0, ifList);

            Assert.AreEqual(1, raiseCount);

            // detach event handler from "statement inserted" event
            Project.Events.StatementAdded -= HandleIfStatementInsertedEvent;
        }

        [Test]
        public void InsertSendStatement()
        {
            // create new process and set it to current
            Process process = Project.Current.AddProcess();
            Project.Current.SetCurrentComponent(process);

            // add first Send statement
            var statement1 = new SendStatement();
            statement1.AddressTo.Text = "doug@carlston.net";
            statement1.SendBody = new SendEmailBody("Here's some email body text.");
            var line1 = new SendLine(statement1);
            ((Process)Project.Current.CurrentComponent).Lines.Add(line1);

            // attach event handler to "statement added" event
            raiseCount = 0;
            Project.Events.StatementAdded += HandleSendStatementInsertedEvent;

            // insert second Send statement
            var statement2 = new SendStatement();
            statement2.AddressTo.Text = "jdf@jdftech.com";
            statement2.SendBody = new SendEmailBody("Here's some different body text.");
            var line2 = new SendLine(statement2);
            ((Process)Project.Current.CurrentComponent).Lines.Insert(0, line2);

            Assert.AreEqual(1, raiseCount);

            // detach event handler from "statement added" event
            Project.Events.StatementAdded -= HandleSendStatementInsertedEvent;
        }

        [Test]
        public void InsertShowStatement()
        {
            // create new process and set it to current
            Process process = Project.Current.AddProcess();
            Project.Current.SetCurrentComponent(process);

            // add first show statement
            IDocument document1 = Project.Current.AddDocument();
            var statement1 = new ShowDocumentStatement(document1);
            ShowLine line1 = new ShowDocumentLine(statement1);
            ((Process)Project.Current.CurrentComponent).Lines.Add(line1);

            // attach event handler to "statement added" event
            raiseCount = 0;
            Project.Events.StatementAdded += HandleStatementInsertedEvent;

            // insert second show statement
            IDocument document2 = Project.Current.AddDocument();
            var statement2 = new ShowDocumentStatement(document2);
            ShowLine line2 = new ShowDocumentLine(statement2);
            ((Process)Project.Current.CurrentComponent).Lines.Insert(0, line2);

            Assert.AreEqual(1, raiseCount);

            // detach event handler from "statement added" event
            Project.Events.StatementAdded -= HandleStatementInsertedEvent;
        }

        [Test]
        public void MapFieldIDs()
        {
            var fibItem1 = new FibItem();
            var fibItem2 =
                new FibItem(
                    new XmlElement(
                        "<fib label=\"Q1\" alternateLabel=\"My FIB\">Fib Item 1: <blank label=\"a\" length=\"10\" required=\"true\"></blank>/></fib>",
                        true));
            Assert.AreSame(fibItem1, Project.FieldMapById[fibItem1.Id]);
            Assert.AreNotSame(fibItem2, Project.FieldMapById[fibItem1.Id]);
            Assert.AreSame(fibItem2, Project.FieldMapById[fibItem2.Id]);

            var blank2 = new Blank(fibItem1, 10);
            var blank4 =
                new Blank(new XmlElement("<blank label=\"a\" length=\"10\" required=\"true\" alternateLabel=\"Alternate\"></blank>"),
                          fibItem1);
            Assert.AreSame(blank2, Project.FieldMapById[blank2.Id]);
            Assert.AreSame(blank4, Project.FieldMapById[blank4.Id]);

            var mcItem1 = new McqItem();
            var mcItem2 =
                new McqItem(
                    new XmlElement(
                        "<mc label=\"Q1\" onlyone=\"true\" required=\"false\"><question>Make a choice:</question><choice label=\"a\">Choice One</choice><choice label=\"b\">Choice Two</choice></mc>"));
            Assert.AreSame(mcItem1, Project.FieldMapById[mcItem1.Id]);
            Assert.AreSame(mcItem2, Project.FieldMapById[mcItem2.Id]);

            var choice1 = new Choice();
            var choice2 = new Choice("Choice text");
            var choice3 = new Choice(new XmlElement("<choice label=\"a\">Choice text</choice>"));
            Assert.AreSame(choice1, Project.FieldMapById[choice1.Id]);
            Assert.AreSame(choice2, Project.FieldMapById[choice2.Id]);
            Assert.AreSame(choice3, Project.FieldMapById[choice3.Id]);

            var var1 = new Variable();
            var var2 = new Variable("Name");
            Assert.AreSame(var1, Project.FieldMapById[var1.Id]);
            Assert.AreSame(var2, Project.FieldMapById[var2.Id]);
        }

        [Test]
        public void Modified()
        {
            string tempName = Path.GetRandomFileName() + ".test.tawala";
            string tempProjFile = Path.Combine(Path.GetTempPath(), tempName);

            try
            {
                // a new empty project is unmodified until the user does something to it
                // the assumption is that there is no need to save band new empty project
                Assert.IsFalse(Project.Current.Modified);

                IForm form = Project.Current.AddForm();
                Assert.IsTrue(Project.Current.Modified);

                Project.Save(tempProjFile);
                Assert.IsFalse(Project.Current.Modified);

                Project.Current.AddProcess();
                Assert.IsTrue(Project.Current.Modified);

                Project.Save(tempProjFile);
                Assert.IsFalse(Project.Current.Modified);

                form.ItemList.Add(new TextItem());

                Assert.IsTrue(Project.Current.Modified);

                Project.New();
                Assert.IsFalse(Project.Current.Modified);

                Project.Current.AddProcess();
                Assert.IsTrue(Project.Current.Modified);

                Project.Open(tempProjFile);
                Assert.IsFalse(Project.Current.Modified);
            }
            finally
            {
                if (File.Exists(tempProjFile))
                {
                    File.Delete(tempProjFile);
                }
            }
        }

        [Test]
        public void MoveComponentDocumentTest()
        {
            IDocument document1 = Project.Current.AddDocument();
            IDocument document2 = Project.Current.AddDocument();
            IDocument document3 = Project.Current.AddDocument();
            IDocument document4 = Project.Current.AddDocument();

            Project.Current.MoveComponent(document3, 1);

            Assert.AreSame(Project.Current.DocumentList[1], document3);
            Assert.AreSame(Project.Current.DocumentList[2], document2);

            Project.Current.MoveComponent(document1, 3);

            Assert.AreSame(Project.Current.DocumentList[0], document3);
            Assert.AreSame(Project.Current.DocumentList[3], document1);
            Assert.AreSame(Project.Current.DocumentList[2], document4);
        }

        [Test]
        public void MoveComponentFormTest()
        {
            IForm form1 = Project.Current.AddForm();
            IForm form2 = Project.Current.AddForm();
            IForm form3 = Project.Current.AddForm();
            IForm form4 = Project.Current.AddForm();

            Project.Current.MoveComponent(form3, 1);

            Assert.AreSame(Project.Current.FormList[1], form3);
            Assert.AreSame(Project.Current.FormList[2], form2);

            Project.Current.MoveComponent(form1, 3);

            Assert.AreSame(Project.Current.FormList[0], form3);
            Assert.AreSame(Project.Current.FormList[3], form1);
            Assert.AreSame(Project.Current.FormList[2], form4);
        }

        [Test]
        public void MoveComponentProcessTest()
        {
            Process process1 = Project.Current.AddProcess();
            Process process2 = Project.Current.AddProcess();
            Process process3 = Project.Current.AddProcess();
            Process process4 = Project.Current.AddProcess();

            Project.Current.MoveComponent(process3, 1);

            Assert.AreSame(Project.Current.ProcessList[1], process3);
            Assert.AreSame(Project.Current.ProcessList[2], process2);

            Project.Current.MoveComponent(process1, 3);

            Assert.AreSame(Project.Current.ProcessList[0], process3);
            Assert.AreSame(Project.Current.ProcessList[3], process1);
            Assert.AreSame(Project.Current.ProcessList[2], process4);
        }

        [Test]
        public void NewProject()
        {
            Util.NewTestProject();

            //Assertions 
            Assert.AreEqual(Resources.ProjectDefaultName, Project.Current.Name);
        }

        [Test]
        public void NewProjectEvent()
        {
            raiseCount = 0;
            Project.Events.NewProject += handleNewProjectEvent;

            Project.New();

            Assert.AreEqual(1, raiseCount);

            Project.Events.NewProject -= handleNewProjectEvent;
        }

        [Test]
        public void NewTestProject()
        {
            Util.NewTestProject();

            // attach event handler to "form added" event
            raiseCount = 0;
            Project.Events.ComponentAdded += handleFormAddedEvent;

            IForm form1 = Project.Current.AddForm();

            // resets form counter and clears Events lists
            Util.NewTestProject();

            IForm form2 = Project.Current.AddForm();
            IForm form3 = Project.Current.AddForm();
        }

        [Test]
        public void PasteDocument()
        {
            IDocument doc1 = Project.Current.AddDocument();
            doc1.Name = "Document 1";

            IDocument doc2 = ComponentMaker.MakeDocumentObject("Document 2");
            Project.Current.PasteDocument(doc2);

            IDocument doc3 = ComponentMaker.MakeDocumentObject("Document 1");
            Project.Current.PasteDocument(doc3);

            IDocument doc4 = ComponentMaker.MakeDocumentObject("Document 1");
            Project.Current.PasteDocument(doc4);

            IDocument doc5 = ComponentMaker.MakeDocumentObject("Document 1");
            Project.Current.PasteDocument(doc5);

            // paste string template: "Copy ($NUM) of $NAME"
            var testString = new StringBuilder(Resources.PasteName2);
            testString.Replace("$NAME", "Document 1");
            testString.Replace("$NUM", "2");
            IDocument doc6 = ComponentMaker.MakeDocumentObject(testString.ToString()); // "Copy (2) of Document 1"
            Project.Current.PasteDocument(doc6);

            IDocument retrievedDoc1 = Project.Current.GetDocument("Document 1");
            IDocument retrievedDoc2 = Project.Current.GetDocument("Document 2");

            // paste string template: "Copy of $NAME"
            testString = new StringBuilder(Resources.PasteName1);
            testString.Replace("$NAME", "Document 1");
            IDocument retrievedDoc3 = Project.Current.GetDocument(testString.ToString());

            // Assertions
            Assert.AreEqual("Document 2", doc2.Name);

            testString = new StringBuilder(Resources.PasteName1);
            testString.Replace("$NAME", "Document 1");
            Assert.AreEqual(testString.ToString(), doc3.Name);

            // paste string template: "Copy ($NUM) of $NAME"
            testString = new StringBuilder(Resources.PasteName2);
            testString.Replace("$NAME", "Document 1");
            testString.Replace("$NUM", "2");
            Assert.AreEqual(testString.ToString(), doc4.Name);

            testString = new StringBuilder(Resources.PasteName2);
            testString.Replace("$NAME", "Document 1");
            testString.Replace("$NUM", "3");
            Assert.AreEqual(testString.ToString(), doc5.Name);

            // "Copy of Copy (2) of Document 1"
            testString = new StringBuilder(Resources.PasteName2);
            testString.Replace("$NAME", "Document 1");
            testString.Replace("$NUM", "2");
            var testString2 = new StringBuilder(Resources.PasteName1);
            testString2.Replace("$NAME", testString.ToString());
            Assert.AreEqual(testString2.ToString(), doc6.Name);

            Assert.AreEqual(retrievedDoc2, doc2);
            Assert.AreEqual(retrievedDoc3, doc3);

            Assert.AreEqual("Document 1", retrievedDoc1.Name);
            Assert.AreEqual("Document 2", retrievedDoc2.Name);
        }

        [Test]
        public void PasteForm()
        {
            // create new processes and add to list
            IForm form1 = Project.Current.AddForm();
            form1.Name = "Form 1";

            IForm form2 = new Form("Form 2");
            Project.Current.PasteForm(form2);

            IForm form3 = new Form("Form 1");
            Project.Current.PasteForm(form3);

            IForm form4 = new Form("Form 1");
            Project.Current.PasteForm(form4);

            IForm form5 = new Form("Form 1");
            Project.Current.PasteForm(form5);

            // paste string template: "Copy ($NUM) of $NAME"
            var testString = new StringBuilder(Resources.PasteName2);
            testString.Replace("$NAME", "Form 1");
            testString.Replace("$NUM", "2");
            IForm form6 = new Form(testString.ToString()); // "Copy (2) of Form 1"
            Project.Current.PasteForm(form6);

            IForm retrievedForm1 = Project.Current.GetForm("Form 1");
            IForm retrievedForm2 = Project.Current.GetForm("Form 2");

            // paste string template: "Copy of $NAME"
            testString = new StringBuilder(Resources.PasteName1);
            testString.Replace("$NAME", "Form 1");
            IForm retrievedForm3 = Project.Current.GetForm(testString.ToString());

            // Assertions
            Assert.AreEqual("Form 2", form2.Name);

            testString = new StringBuilder(Resources.PasteName1);
            testString.Replace("$NAME", "Form 1");
            Assert.AreEqual(testString.ToString(), form3.Name);

            // paste string template: "Copy ($NUM) of $NAME"
            testString = new StringBuilder(Resources.PasteName2);
            testString.Replace("$NAME", "Form 1");
            testString.Replace("$NUM", "2");
            Assert.AreEqual(testString.ToString(), form4.Name);

            testString = new StringBuilder(Resources.PasteName2);
            testString.Replace("$NAME", "Form 1");
            testString.Replace("$NUM", "3");
            Assert.AreEqual(testString.ToString(), form5.Name);

            // "Copy of Copy (2) of Form 1"
            testString = new StringBuilder(Resources.PasteName2);
            testString.Replace("$NAME", "Form 1");
            testString.Replace("$NUM", "2");
            var testString2 = new StringBuilder(Resources.PasteName1);
            testString2.Replace("$NAME", testString.ToString());
            Assert.AreEqual(testString2.ToString(), form6.Name);

            Assert.AreEqual(retrievedForm2, form2);
            Assert.AreEqual(retrievedForm3, form3);

            Assert.AreEqual("Form 1", retrievedForm1.Name);
            Assert.AreEqual("Form 2", retrievedForm2.Name);
        }

        [Test]
        public void PasteProcess()
        {
            Process proc1 = Project.Current.AddProcess();
            proc1.Name = "Process 1";

            var proc2 = new Process("Process 2");
            Project.Current.PasteProcess(proc2);

            var proc3 = new Process("Process 1");
            Project.Current.PasteProcess(proc3);

            var proc4 = new Process("Process 1");
            Project.Current.PasteProcess(proc4);

            var proc5 = new Process("Process 1");
            Project.Current.PasteProcess(proc5);

            // paste string template: "Copy ($NUM) of $NAME"
            var testString = new StringBuilder(Resources.PasteName2);
            testString.Replace("$NAME", "Process 1");
            testString.Replace("$NUM", "2");
            var proc6 = new Process(testString.ToString()); // "Copy (2) of Process 1"
            Project.Current.PasteProcess(proc6);

            Process retrievedProc1 = Project.Current.GetProcess("Process 1");
            Process retrievedProc2 = Project.Current.GetProcess("Process 2");

            // paste string template: "Copy of $NAME"
            testString = new StringBuilder(Resources.PasteName1);
            testString.Replace("$NAME", "Process 1");
            Process retrievedProc3 = Project.Current.GetProcess(testString.ToString());

            // Assertions
            Assert.AreEqual("Process 2", proc2.Name);

            testString = new StringBuilder(Resources.PasteName1);
            testString.Replace("$NAME", "Process 1");
            Assert.AreEqual(testString.ToString(), proc3.Name);

            // paste string template: "Copy ($NUM) of $NAME"
            testString = new StringBuilder(Resources.PasteName2);
            testString.Replace("$NAME", "Process 1");
            testString.Replace("$NUM", "2");
            Assert.AreEqual(testString.ToString(), proc4.Name);

            testString = new StringBuilder(Resources.PasteName2);
            testString.Replace("$NAME", "Process 1");
            testString.Replace("$NUM", "3");
            Assert.AreEqual(testString.ToString(), proc5.Name);

            // "Copy of Copy (2) of Form 1"
            testString = new StringBuilder(Resources.PasteName2);
            testString.Replace("$NAME", "Process 1");
            testString.Replace("$NUM", "2");
            var testString2 = new StringBuilder(Resources.PasteName1);
            testString2.Replace("$NAME", testString.ToString());
            Assert.AreEqual(testString2.ToString(), proc6.Name);

            Assert.AreEqual(retrievedProc2, proc2);
            Assert.AreEqual(retrievedProc3, proc3);

            Assert.AreEqual("Process 1", retrievedProc1.Name);
            Assert.AreEqual("Process 2", retrievedProc2.Name);

            testString = new StringBuilder(Resources.PasteName1);
            testString.Replace("$NAME", "Process 1");
            Assert.AreEqual(testString.ToString(), retrievedProc3.Name);
        }

        [Test]
        public void ProcessList()
        {
            // create new processes and add to list
            Project.Current.AddProcess();
            Project.Current.AddProcess();

            IList<Process> list = Project.Current.ProcessList;

            // Assertions
            Assert.IsTrue(list.IsReadOnly);
            Assert.IsTrue(list.Count == 2);
            Assert.AreEqual(Resources.ProcessDefaultBaseName + " 1", (list[0]).Name);
            Assert.AreEqual(Resources.ProcessDefaultBaseName + " 2", (list[1]).Name);

            // Verify that read-only wrapper truely is a wrapper and thus should reflect changes

            Project.Current.AddProcess();

            // Assertions
            Assert.IsTrue(list.Count == 3);
            Assert.AreEqual(Resources.ProcessDefaultBaseName + " 1", (list[0]).Name);
            Assert.AreEqual(Resources.ProcessDefaultBaseName + " 2", (list[1]).Name);
            Assert.AreEqual(Resources.ProcessDefaultBaseName + " 3", (list[2]).Name);
        }

        [Test]
        public void ProjectSaveFileCheck()
        {
            Project.Current.Name = "No Credentials Project";

            IForm form = Project.Current.AddForm();
            IDocument document = Project.Current.AddDocument();
            Process process = Project.Current.AddProcess();

            string rootName = Path.GetFileNameWithoutExtension(tmpProjPath);
            Assert.IsTrue(rootName.EndsWith(".test"));

            Project.Save(tmpProjPath);
            xml = new XmlDocument();
            xml.Load(tmpProjPath);

            Assert.AreEqual(rootName, Project.Current.Name);
            Assert.IsNotNull(xml.SelectSingleNode("//project"));
            Assert.IsNotNull(xml.SelectSingleNode("//project[@name='" + rootName + "']"));
            Assert.IsNull(xml.SelectSingleNode("//credentials"));
            Assert.IsNull(xml.SelectSingleNode("//request"));
        }

        [Test]
        public void RealOrVirtualDocumentList()
        {
            Assert.AreEqual(0, Project.Current.RealOrVirtualDocumentList.Count);

            // create a new document (which adds it to the "real" document list)
            IDocument realDoc = Project.Current.AddDocument();

            Assert.AreEqual(1, Project.Current.RealOrVirtualDocumentList.Count);
            Assert.AreEqual(realDoc, (Project.Current.RealOrVirtualDocumentList[0]));

            // try to access a non-existent virtual document
            IDocument virtualDoc = Project.Current.GetRealOrVirtualDocument("Test Doc", false);
            Assert.AreEqual(1, Project.Current.RealOrVirtualDocumentList.Count);

            // setting the second arg to true causes the virtual document to be created
            virtualDoc = Project.Current.GetRealOrVirtualDocument("Test Doc", true);
            Assert.AreEqual(2, Project.Current.RealOrVirtualDocumentList.Count);
            Assert.AreEqual(virtualDoc, (Project.Current.RealOrVirtualDocumentList[1]));
        }

        [Test]
        public void RemoveDocument()
        {
            // attach event handler to "document removed" event
            raiseCount = 0;
            Project.Events.ComponentRemoved += HandleDocumentRemovedEvent;

            // create new document and add to list
            Project.Current.AddDocument();

            // try removing a document that doesn't exist
            Project.Current.RemoveDocument("No Document");

            // remove document from list
            Project.Current.RemoveDocument("Document 1");

            Assert.AreEqual(1, raiseCount);
        }

        [Test]
        public void RemoveForm()
        {
            // attach event handlers
            Project.Events.ComponentRemoved += handleFormRemovedEvent;

            // create new form and add to list
            raiseCount = 0;
            Project.Current.AddForm();
            Assert.AreEqual(0, raiseCount, "event raiseCount != 0");

            // try removing a form that doesn't exist
            raiseCount = 0;
            Project.Current.RemoveForm("No Form");
            Assert.AreEqual(0, raiseCount, "event raiseCount != 0");

            // remove form from list
            raiseCount = 0;
            Project.Current.RemoveForm(Resources.FormDefaultBaseName + " 1");
            Assert.AreEqual(1, raiseCount, "event raiseCount != 1");
        }

        [Test]
        public void RemoveProcess()
        {
            raiseCount = 0;
            Project.Events.ComponentRemoved += handleProcessRemovedEvent;

            // create new process and add to list
            Project.Current.AddProcess();

            // first, try to remove a non-existent process
            Project.Current.RemoveProcess("No Process");

            // now remove the real process from list
            Project.Current.RemoveProcess(Resources.ProcessDefaultBaseName + " 1");

            Assert.AreEqual(1, raiseCount);
        }

        [Test]
        public void RemoveProcessWhenConnectedToForm()
        {
            IForm form = Project.Current.AddForm();
            form.Name = "Test Form";
            Process proc = Project.Current.AddProcess();
            proc.Name = "Connected Process";

            // connect the Process to the Form
            Project.Current.ConnectProcessToForm(proc, "Test Form");

            // Assertion
            Assert.AreEqual("Connected Process", form.ConnectedPostProcess.Name);

            // remove the Process and make sure it no longer shows up as connected to the Form
            Project.Current.RemoveProcess("Connected Process");

            // Assertion
            Assert.AreEqual(null, form.ConnectedPostProcess);
        }

        [Test]
        public void RemoveSendStatement()
        {
            // create new process and set it to current
            Process process = Project.Current.AddProcess();
            Project.Current.SetCurrentComponent(process);

            // add 2 Send statements
            var statement1 = new SendStatement();
            statement1.AddressTo.Text = "doug@carlston.net";
            statement1.SendBody = new SendEmailBody("Here's some email body text.");
            var line1 = new SendLine(statement1);
            ((Process)Project.Current.CurrentComponent).Lines.Add(line1);

            var statement2 = new SendStatement();
            statement2.AddressTo.Text = "jdf@jdftech.com";
            statement2.SendBody = new SendEmailBody("Here's some different body text.");
            var line2 = new SendLine(statement2);
            ((Process)Project.Current.CurrentComponent).Lines.Add(line2);

            Assert.AreEqual(2, process.Lines.Count);

            // attach event handler to "statement removed" event
            raiseCount = 0;
            Project.Events.StatementRemoved += HandleSendStatementRemovedEvent;

            // remove first statement
            ((Process)Project.Current.CurrentComponent).Lines.Remove(line1);

            Assert.AreEqual(1, raiseCount);

            // detach event handler from "statement removed" event
            Project.Events.StatementRemoved -= HandleSendStatementRemovedEvent;

            Assert.AreEqual(1, process.Lines.Count);
        }

        [Test]
        public void RemoveShowStatement()
        {
            // create new process and set it to current
            Process process = Project.Current.AddProcess();
            Project.Current.SetCurrentComponent(process);

            // add 2 show statements
            IDocument document1 = Project.Current.AddDocument();
            var statement1 = new ShowDocumentStatement(document1);
            ShowLine line1 = new ShowDocumentLine(statement1);

            IDocument document2 = Project.Current.AddDocument();
            var statement2 = new ShowDocumentStatement(document2);
            ShowLine line2 = new ShowDocumentLine(statement2);

            ((Process)Project.Current.CurrentComponent).Lines.Add(line1);
            ((Process)Project.Current.CurrentComponent).Lines.Add(line2);

            Assert.AreEqual(2, process.Lines.Count);

            // attach event handler to "statement removed" event
            raiseCount = 0;
            Project.Events.StatementRemoved += HandleStatementRemovedEvent;

            // remove first statement
            ((Process)Project.Current.CurrentComponent).Lines.Remove(line1);

            Assert.AreEqual(1, raiseCount);

            // detach event handler from "statement removed" event
            Project.Events.StatementRemoved -= HandleStatementRemovedEvent;

            Assert.AreEqual(1, process.Lines.Count);
        }

        [Test]
        public void RenameDocument()
        {
            // attach event handler to "document renamed" event
            raiseCount = 0;
            Project.Events.ComponentRenamed += HandleDocumentRenamedEvent;

            // create new document and add to list
            Project.Current.AddDocument();

            // rename document in list
            bool success = Project.Current.RenameDocument(Resources.DocumentDefaultBaseName + " 1", "Document 1 Renamed");
            Assert.AreEqual(true, success);
            Assert.AreEqual(1, raiseCount);
        }

        [Test]
        public void RenameForm()
        {
            IForm form = Project.Current.AddForm();
            string oldname = form.Name;

            Project.Events.ComponentRenamed += HandleFormRenamedEvent;

            raiseCount = 0;
            bool success = Project.Current.RenameForm(oldname, "Form 1 Renamed");
            Assert.AreEqual(1, raiseCount, "event raiseCount != 1");
            Assert.AreEqual(true, success);
        }

        [Test]
        public void RenameProcess()
        {
            raiseCount = 0;
            Project.Events.ComponentRenamed += HandleProcessRenamedEvent;

            // create new process and add to list
            Project.Current.AddProcess();

            // rename process in list
            bool success = Project.Current.RenameProcess(Resources.ProcessDefaultBaseName + " 1", "Process 1 Renamed");
            Assert.AreEqual(true, success);
            Assert.AreEqual(1, raiseCount);
        }

        [Test]
        public void RenameProject()
        {
            Project.Current.Name = "Project Renamed";

            Assert.AreEqual("Project Renamed", Project.Current.Name);
        }

        [Test]
        public void SetCurrentComponent()
        {
            IForm form = Project.Current.AddForm();
            Project.Current.SetCurrentComponent(form);
            Assert.AreEqual(form, Project.Current.CurrentComponent);

            IDocument doc = Project.Current.AddDocument();
            Project.Current.SetCurrentComponent(doc);
            Assert.AreEqual(doc, Project.Current.CurrentComponent);

            Process proc = Project.Current.AddProcess();
            Project.Current.SetCurrentComponent(proc);
            Assert.AreEqual(proc, Project.Current.CurrentComponent);
        }

        [Test]
        public void StartingPointList()
        {
            IForm form1 = Project.Current.AddForm();
            form1.StartingPoint = true;

            Project.Current.AddForm();

            Assert.AreEqual(1, Project.Current.StartingPointList.Count);
            Assert.AreEqual("Form 1", Project.Current.StartingPointList[0].Name);
        }

        [Test]
        public void ToXmlForSaving()
        {
            Project.Save(tmpProjPath);
            xml = new XmlDocument();
            xml.LoadXml(Project.Current.ToXmlForSaving());
            Assert.IsNotNull(xml.SelectSingleNode("//project"));
            Assert.IsNull(xml.SelectSingleNode("//credentials"), "Xml for saving shouldn't include credentials tag");
            Assert.IsNull(xml.SelectSingleNode("//request"), "Xml for saving shouldn't include request tag");
        }

        [Test]
        public void ToXmlForUpload()
        {
            Project.Current.Name = "ToXmlForUpload Project";

            string tempName = Path.GetRandomFileName() + ".test.tawala";
            string tempProjFile = Path.Combine(Path.GetTempPath(), tempName);

            Project.Current.Name = Path.GetFileNameWithoutExtension(tempProjFile);
            Assert.IsTrue(Project.Current.Name.EndsWith(".test"));

            try
            {
                Project.Save(tempProjFile);
                xml = new XmlDocument();
                string credentialsXml = GlobalSettings.CredentialsElement("xyzzy", "1649237");
                xml.LoadXml(Project.Current.ToXmlForUpload(credentialsXml));
                Assert.IsNotNull(xml.SelectSingleNode("//project"));
                Assert.IsNotNull(xml.SelectSingleNode("//credentials[@user='xyzzy' and @password='1649237']"));
                Assert.IsNotNull(xml.SelectSingleNode("//request"));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message + "\r\n" + e.StackTrace);
            }
            finally
            {
                xml = null;

                try
                {
                    File.Delete(tempProjFile);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}