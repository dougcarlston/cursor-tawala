// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.ProjectUI;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.HiddenFields
{
    [TestFixture]
    public class TemporaryVariablesTest2151
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            setupForm();
            setupProcess();
            form.ConnectedPostProcess = process;

            setupFieldsPalette();
        }

        [TearDown]
        public void Teardown()
        {
            process.ActiveGetStatement = null;
            if (testPalette != null)
            {
                testPalette.Dispose();
                testPalette = null;
            }
        }

        #endregion

        private IForm form;
        private Blank blank;
        private FieldsPalette testPalette;
        private Process process;
        private GetStatement getStatement;
        private FormList forms;
        private RecordSet recordSet;

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

            addSetStatement(makeSetStatement());

            getStatement = makeGetStatement();
            addGetStatement(getStatement);
            addForEachStatement(makeForEachStatement());
        }

        private void addGetStatement(GetStatement getStatement)
        {
            process.Lines.Add(getGetLine(getStatement));
        }

        private GetStatement makeGetStatement()
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

        private void addSetStatement(SetStatement setStatement)
        {
            process.Lines.Add(getSetLine(setStatement));
        }

        private SetStatement makeSetStatement()
        {
            var setStatement = new SetStatement();
            setStatement.Variable = new Variable("Variable 1");
            setStatement.Expression = new Expression("100");

            return (setStatement);
        }

        private static ProcessLineList getSetLine(SetStatement setStatement)
        {
            return (new ProcessLineList(setStatement));
        }

        private void addForEachStatement(ForEachStatement forEachStatement)
        {
            process.Lines.Add(getForEachLine(forEachStatement));
        }

        private ForEachStatement makeForEachStatement()
        {
            var record = new Record("Record");
            ForEachStatement forEachStatement = new ForEachRecordStatement(record, recordSet);

            return (forEachStatement);
        }

        private static ProcessLineList getForEachLine(ForEachStatement forEachStatement)
        {
            return (new ProcessLineList(forEachStatement));
        }

        private void setupFieldsPalette()
        {
            testPalette = new FieldsPalette();
            testPalette.Show();
        }

        private void selectGetStatement()
        {
            process.ActiveGetStatement = getStatement;
            Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, 1, getStatement));
            testPalette.RefreshFieldList();
        }

        private void setInsertionPointWithinForEachStatement()
        {
            Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(process, 4));
            testPalette.RefreshFieldList();
        }

        [Test]
        public void CompoundSetExpressionConvertsToSimpleSetFieldExpression()
        {
            string inputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Compound Set Expression 01"" themePath=""default"" format=""1.5"">" + Environment.NewLine +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" process=""Process 1"" blockBackButton=""false"">" +
                Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.FullBeginFont + @"First: " +
                XmlConstants.EndFont + @"<blank label=""a"" length=""10"" required=""false"" " +
                @"alternateLabel=""First""/></paragraph></fib>" + Environment.NewLine +
                @"<fib label=""Q2""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.FullBeginFont + @"Last: " +
                XmlConstants.EndFont +
                @"<blank label=""a"" length=""10"" required=""false"" alternateLabel=""Last""></blank>" +
                @"</paragraph></fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"<set field=""Full"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string field=""Form 1:First""/>" + Environment.NewLine +
                @"<string value="" ""/>" + Environment.NewLine +
                @"<string field=""Form 1:Last""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            string outputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Compound Set Expression 01"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" process=""Process 1"" blockBackButton=""false"">" +
                Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.FullBeginFont + @"First: " +
                XmlConstants.EndFont +
                @"<blank label=""a"" length=""10"" required=""false"" alternateLabel=""First""></blank>" +
                @"</paragraph></fib>" + Environment.NewLine +
                @"<fib label=""Q2""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.FullBeginFont + @"Last: " +
                XmlConstants.EndFont +
                @"<blank label=""a"" length=""10"" required=""false"" alternateLabel=""Last""></blank></paragraph></fib>" +
                Environment.NewLine +
                @"<field name=""Full""/>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"<set field=""Full"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string field=""Form 1:First""/>" + Environment.NewLine +
                @"<string value="" ""/>" + Environment.NewLine +
                @"<string field=""Form 1:Last""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"<set field=""Form 1:Full"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string field=""Full""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Util.OpenProjectXml(inputXmlString);

            // Console.WriteLine(Project.Current.ToXmlForSaving());
            Assert.AreEqual(outputXmlString, Project.Current.ToXmlForSaving());
        }

        [Test]
        [Ignore]
        public void ConvertingFieldTableLeavesXmlIntact()
        {
            string inputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Field Table Show"" themePath=""default"" format=""1.5"">" + Environment.NewLine +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" process=""Process 1"">" + Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.BeginFont +
                @"[Replace this with your question. Underscores create blanks.] " + XmlConstants.EndFont + @"<blank " +
                @"label=""a"" length=""10"" required=""false""/></paragraph></fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"<show document=""Document 1"" reset=""false""/>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"<documents>" + Environment.NewLine +
                @"<document name=""Document 1"">" + Environment.NewLine +
                @"<xmlData>" + Environment.NewLine +
                @"<paragraph indent=""0"" align=""left""><tabPositions><tabStop position=""720""/><tabStop " +
                @"position=""1440""/><tabStop position=""2160""/><tabStop position=""2880""/><tabStop " +
                @"position=""3600""/><tabStop position=""4320""/><tabStop position=""5040""/><tabStop " +
                @"position=""5760""/><tabStop position=""6480""/><tabStop position=""7200""/><tabStop " +
                @"position=""7920""/><tabStop position=""8640""/><tabStop position=""9360""/><tabStop " +
                @"position=""10080""/></tabPositions>" + XmlConstants.BeginFont + @"<itemization-table " +
                @"version=""1""><number-of-columns>1</number-of-columns><column><header>Test</header><contents><field " +
                @"name=""Record:Form 1:Q1:a"" /></contents></column><conditions><form name=""Form 1""/><conditions>" + Environment.NewLine +
                @"<equals field=""Record:Form 1:Q1:a"">" + Environment.NewLine +
                @"</equals>" + Environment.NewLine +
                @"</conditions></conditions></itemization-table>" + XmlConstants.EndFont + @"</paragraph>" + Environment.NewLine +
                @"</xmlData>" + Environment.NewLine +
                @"</document>" + Environment.NewLine +
                @"</documents>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            string outputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Field Table Show"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" process=""Process 1"">" + Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.BeginFont +
                @"[Replace this with your question. Underscores create blanks.] " + XmlConstants.EndFont + @"<blank " +
                @"label=""a"" length=""10"" required=""false""/></paragraph></fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"<show document=""Document 1"" reset=""false""/>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"<documents>" + Environment.NewLine +
                @"<document name=""Document 1"">" + Environment.NewLine +
                @"<xmlData>" + Environment.NewLine +
                @"<paragraph indent=""0"" align=""left""><tabPositions><tabStop position=""720""/><tabStop " +
                @"position=""1440""/><tabStop position=""2160""/><tabStop position=""2880""/><tabStop " +
                @"position=""3600""/><tabStop position=""4320""/><tabStop position=""5040""/><tabStop " +
                @"position=""5760""/><tabStop position=""6480""/><tabStop position=""7200""/><tabStop " +
                @"position=""7920""/><tabStop position=""8640""/><tabStop position=""9360""/><tabStop " +
                @"position=""10080""/></tabPositions>" + XmlConstants.BeginFont + @"<itemization-table " +
                @"version=""1""><number-of-columns>1</number-of-columns><column><header>Test</header><contents><field " +
                @"name=""Record:Form 1:Q1:a"" /></contents></column><conditions><form name=""Form 1""/><conditions>" + Environment.NewLine +
                @"<equals field=""Record:Form 1:Q1:a"">" + Environment.NewLine +
                @"</equals>" + Environment.NewLine +
                @"</conditions></conditions></itemization-table>" + XmlConstants.EndFont + @"</paragraph>" + Environment.NewLine +
                @"</xmlData>" + Environment.NewLine +
                @"</document>" + Environment.NewLine +
                @"</documents>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Util.OpenProjectXml(inputXmlString);

            Assert.AreEqual(outputXmlString, Project.Current.ToXmlForSaving());
        }

        [Test]
        [Ignore]
        public void ConvertingFieldTableWithWhereLeavesXmlIntact()
        {
            string inputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Story 2151 Test Field Table Where"" themePath=""default"" format=""1.5"">" + Environment.NewLine +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" process=""Process 1"">" + Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.BeginFont +
                @"[Replace this with your question. Underscores create blanks.] " + XmlConstants.EndFont + @"<blank " +
                @"label=""a"" length=""10"" required=""false""/></paragraph></fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"<show document=""Document 1"" reset=""false""/>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"<documents>" + Environment.NewLine +
                @"<document name=""Document 1"">" + Environment.NewLine +
                @"<xmlData>" + Environment.NewLine +
                @"<paragraph indent=""0"" align=""left""><tabPositions><tabStop position=""720""/><tabStop " +
                @"position=""1440""/><tabStop position=""2160""/><tabStop position=""2880""/><tabStop " +
                @"position=""3600""/><tabStop position=""4320""/><tabStop position=""5040""/><tabStop " +
                @"position=""5760""/><tabStop position=""6480""/><tabStop position=""7200""/><tabStop " +
                @"position=""7920""/><tabStop position=""8640""/><tabStop position=""9360""/><tabStop " +
                @"position=""10080""/></tabPositions>" + XmlConstants.BeginFont + @"<itemization-table " +
                @"version=""1""><number-of-columns>1</number-of-columns><column><header>Test</header><contents><field " +
                @"name=""Record:Form 1:Q1:a"" /></contents></column><conditions><form name=""Form 1""/><conditions>" + Environment.NewLine +
                @"<equals field=""Record:Form 1:Q1:a"">" + Environment.NewLine +
                @"<string value=""100""/>" + Environment.NewLine +
                @"</equals>" + Environment.NewLine +
                @"</conditions></conditions></itemization-table>" + XmlConstants.EndFont + @"</paragraph>" + Environment.NewLine +
                @"</xmlData>" + Environment.NewLine +
                @"</document>" + Environment.NewLine +
                @"</documents>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            string outputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Story 2151 Test Field Table Where"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" process=""Process 1"">" + Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.BeginFont +
                @"[Replace this with your question. Underscores create blanks.] " + XmlConstants.EndFont + @"<blank " +
                @"label=""a"" length=""10"" required=""false""/></paragraph></fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"<show document=""Document 1"" reset=""false""/>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"<documents>" + Environment.NewLine +
                @"<document name=""Document 1"">" + Environment.NewLine +
                @"<xmlData>" + Environment.NewLine +
                @"<paragraph indent=""0"" align=""left""><tabPositions><tabStop position=""720""/><tabStop " +
                @"position=""1440""/><tabStop position=""2160""/><tabStop position=""2880""/><tabStop " +
                @"position=""3600""/><tabStop position=""4320""/><tabStop position=""5040""/><tabStop " +
                @"position=""5760""/><tabStop position=""6480""/><tabStop position=""7200""/><tabStop " +
                @"position=""7920""/><tabStop position=""8640""/><tabStop position=""9360""/><tabStop " +
                @"position=""10080""/></tabPositions>" + XmlConstants.BeginFont + @"<itemization-table " +
                @"version=""1""><number-of-columns>1</number-of-columns><column><header>Test</header><contents><field " +
                @"name=""Record:Form 1:Q1:a"" /></contents></column><conditions><form name=""Form 1""/><conditions>" + Environment.NewLine +
                @"<equals field=""Record:Form 1:Q1:a"">" + Environment.NewLine +
                @"<string value=""100""/>" + Environment.NewLine +
                @"</equals>" + Environment.NewLine +
                @"</conditions></conditions></itemization-table>" + XmlConstants.EndFont + @"</paragraph>" + Environment.NewLine +
                @"</xmlData>" + Environment.NewLine +
                @"</document>" + Environment.NewLine +
                @"</documents>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Util.OpenProjectXml(inputXmlString);

            Assert.AreEqual(outputXmlString, Project.Current.ToXmlForSaving());
        }

        [Test]
        public void MultipleSetsAddsHiddenFieldAndSetStatements()
        {
            string inputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Story 2151 Test Multiple Sets"" themePath=""default"" format=""1.5"">" + Environment.NewLine +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" process=""Process 1"" blockBackButton=""false"">" +
                Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.FullBeginFont +
                @"[Replace this with your question. Underscores create blanks.] " + XmlConstants.EndFont +
                @"<blank label=""a"" length=""10"" required=""false""></blank>" +
                @"</paragraph></fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"<set field=""Score"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string value=""1""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"<set field=""Score"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string value=""2""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            string outputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Story 2151 Test Multiple Sets"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" process=""Process 1"" blockBackButton=""false"">" +
                Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.FullBeginFont +
                @"[Replace this with your question. Underscores create blanks.] " + XmlConstants.EndFont +
                @"<blank label=""a"" length=""10"" required=""false""></blank>" +
                @"</paragraph></fib>" + Environment.NewLine +
                "<field name=\"Score\"/>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"<set field=""Score"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string value=""1""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"<set field=""Form 1:Score"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string field=""Score""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"<set field=""Score"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string value=""2""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"<set field=""Form 1:Score"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string field=""Score""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Util.OpenProjectXml(inputXmlString);

            // Console.WriteLine(Project.Current.ToXmlForSaving());
            Assert.AreEqual(outputXmlString, Project.Current.ToXmlForSaving());
        }

        [Test]
        public void MultipleSetsAddsMultipleHiddenFields()
        {
            string inputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Var to Field test 38"" themePath=""default"" format=""1.4"">" + Environment.NewLine +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" process=""Process 1"" blockBackButton=""false"">" +
                Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<mc label=""Q1"" onlyone=""true"" required=""false""" + XmlConstants.DefaultMcqItemStyleAttribute +
                @"><question><paragraph indent=""0"" " +
                @"align=""left""><tabPositions><tabStop position=""720""/><tabStop position=""1440""/><tabStop " +
                @"position=""2160""/><tabStop position=""2880""/><tabStop position=""3600""/><tabStop " +
                @"position=""4320""/><tabStop position=""5040""/><tabStop position=""5760""/><tabStop " +
                @"position=""6480""/><tabStop position=""7200""/><tabStop position=""7920""/><tabStop " +
                @"position=""8640""/><tabStop position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.BeginFont +
                @"MCQ 1" + XmlConstants.EndFont + @"</paragraph></question><choice " +
                @"label=""a""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.BeginFont + @"Choice 1" +
                XmlConstants.EndFont + @"</paragraph></choice><choice label=""b""><paragraph indent=""0"" " +
                @"align=""left""><tabPositions><tabStop position=""720""/><tabStop position=""1440""/><tabStop " +
                @"position=""2160""/><tabStop position=""2880""/><tabStop position=""3600""/><tabStop " +
                @"position=""4320""/><tabStop position=""5040""/><tabStop position=""5760""/><tabStop " +
                @"position=""6480""/><tabStop position=""7200""/><tabStop position=""7920""/><tabStop " +
                @"position=""8640""/><tabStop position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.BeginFont +
                @"Choice 2" + XmlConstants.EndFont + @"</paragraph></choice></mc>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"<set field=""Var1"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string value=""Choice 1""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"<set field=""Var2"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string value=""Choice 2""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"<process name=""Process 2"">" + Environment.NewLine +
                @"<get recordList=""Record List 1"">" + Environment.NewLine +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1""/>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"</get>" + Environment.NewLine +
                @"<foreach record=""Record"" recordList=""Record List 1"">" + Environment.NewLine +
                @"<if>" + Environment.NewLine +
                @"<conditions>" + Environment.NewLine +
                @"<equals field=""Record:Form 1:Var2"">" + Environment.NewLine +
                @"<string value=""Choice 2""/>" + Environment.NewLine +
                @"</equals>" + Environment.NewLine +
                @"</conditions>" + Environment.NewLine +
                @"<trueSet>" + Environment.NewLine +
                @"</trueSet>" + Environment.NewLine +
                @"</if>" + Environment.NewLine +
                @"</foreach>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            string outputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Var to Field test 38"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" process=""Process 1"" blockBackButton=""false"">" +
                Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<mc label=""Q1"" onlyone=""true"" required=""false""" + XmlConstants.DefaultMcqItemStyleAttribute +
                @"><question><paragraph indent=""0"" " +
                @"align=""left""><tabPositions><tabStop position=""720""/><tabStop position=""1440""/><tabStop " +
                @"position=""2160""/><tabStop position=""2880""/><tabStop position=""3600""/><tabStop " +
                @"position=""4320""/><tabStop position=""5040""/><tabStop position=""5760""/><tabStop " +
                @"position=""6480""/><tabStop position=""7200""/><tabStop position=""7920""/><tabStop " +
                @"position=""8640""/><tabStop position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.BeginFont +
                @"MCQ 1" + XmlConstants.EndFont + @"</paragraph></question><choice " +
                @"label=""a""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.BeginFont + @"Choice 1" +
                XmlConstants.EndFont + @"</paragraph></choice><choice label=""b""><paragraph indent=""0"" " +
                @"align=""left""><tabPositions><tabStop position=""720""/><tabStop position=""1440""/><tabStop " +
                @"position=""2160""/><tabStop position=""2880""/><tabStop position=""3600""/><tabStop " +
                @"position=""4320""/><tabStop position=""5040""/><tabStop position=""5760""/><tabStop " +
                @"position=""6480""/><tabStop position=""7200""/><tabStop position=""7920""/><tabStop " +
                @"position=""8640""/><tabStop position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.BeginFont +
                @"Choice 2" + XmlConstants.EndFont + @"</paragraph></choice></mc>" + Environment.NewLine +
                @"<field name=""Var1""/>" + Environment.NewLine +
                @"<field name=""Var2""/>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"<set field=""Var1"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string value=""Choice 1""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"<set field=""Form 1:Var1"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string field=""Var1""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"<set field=""Var2"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string value=""Choice 2""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"<set field=""Form 1:Var2"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string field=""Var2""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"<process name=""Process 2"">" + Environment.NewLine +
                @"<get recordList=""Record List 1"">" + Environment.NewLine +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1""/>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"</get>" + Environment.NewLine +
                @"<foreach record=""Record"" recordList=""Record List 1"">" + Environment.NewLine +
                @"<if>" + Environment.NewLine +
                @"<conditions>" + Environment.NewLine +
                @"<equals field=""Record:Form 1:Var2"">" + Environment.NewLine +
                @"<string value=""Choice 2""/>" + Environment.NewLine +
                @"</equals>" + Environment.NewLine +
                @"</conditions>" + Environment.NewLine +
                @"<trueSet>" + Environment.NewLine +
                @"</trueSet>" + Environment.NewLine +
                @"</if>" + Environment.NewLine +
                @"</foreach>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Util.OpenProjectXml(inputXmlString);

            // Console.WriteLine(Project.Current.ToXmlForSaving());
            Assert.AreEqual(outputXmlString, Project.Current.ToXmlForSaving());
        }

        [Test]
        public void OpeningCurrentProjectXmlCausesNoModification()
        {
            string xmlString =
                "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine +
                "<project name=\"Story 2151 Test 01\" themePath=\"default\" format=\"" + Project.XmlFormatVersion +
                "\" designerBuild=\"0\">" + Environment.NewLine +
                "<pageHeader></pageHeader>" +
                "<forms>" + Environment.NewLine +
                "<form name=\"Form 1\" startPoint=\"true\" themePath=\"default\" process=\"Process 1\" blockBackButton=\"false\">" +
                Environment.NewLine +
                "<items>" + Environment.NewLine +
                "<field name=\"Score\"/>" + Environment.NewLine +
                "</items>" + Environment.NewLine +
                "</form>" + Environment.NewLine +
                "</forms>" + Environment.NewLine +
                "<processes>" + Environment.NewLine +
                "<process name=\"Process 1\">" + Environment.NewLine +
                "<set field=\"Score\" arithmeticAsText=\"false\">" + Environment.NewLine +
                "<string value=\"100\"/>" + Environment.NewLine +
                "</set>" + Environment.NewLine +
                "<set field=\"Form 1:Score\" arithmeticAsText=\"false\">" + Environment.NewLine +
                "<string value=\"100\"/>" + Environment.NewLine +
                "</set>" + Environment.NewLine +
                "</process>" + Environment.NewLine +
                "</processes>" + Environment.NewLine +
                "</project>" + Environment.NewLine;

            Util.OpenProjectXml(xmlString);

            Assert.AreEqual(xmlString, Project.Current.ToXmlForSaving());
        }

        [Test]
        public void OpeningOldProjectXmlLeavesFibIntact()
        {
            string inputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Story 2151 Test FIB"" themePath=""default"" format=""1.5"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" blockBlackButton=""false"">" + Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.FullBeginFont +
                @"[Replace this with your question. Underscores create blanks.] " + XmlConstants.EndFont +
                @"<blank label=""a"" length=""10"" required=""false""></blank>" +
                @"</paragraph></fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            string outputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Story 2151 Test FIB"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" blockBackButton=""false"">" + Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.FullBeginFont +
                @"[Replace this with your question. Underscores create blanks.] " + XmlConstants.EndFont +
                @"<blank label=""a"" length=""10"" required=""false""></blank>" +
                @"</paragraph></fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Util.OpenProjectXml(inputXmlString);

            Assert.AreEqual(outputXmlString, Project.Current.ToXmlForSaving());
        }

        [Test]
        public void OpeningOldProjectXmlLeavesTextIntact()
        {
            string inputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Story 2151 Test Text"" themePath=""default"" format=""1.5"">" + Environment.NewLine +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" blockBackButton=""false"">" + Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<text label=""T1""" + XmlConstants.DefaultTextItemStyleAttribute +
                @"><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.BeginFont +
                @"[Replace this with text of your own.]" + XmlConstants.EndFont + @"</paragraph></text>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            string outputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Story 2151 Test Text"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" blockBackButton=""false"">" + Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<text label=""T1""" + XmlConstants.DefaultTextItemStyleAttribute +
                @"><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.BeginFont +
                @"[Replace this with text of your own.]" + XmlConstants.EndFont + @"</paragraph></text>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Util.OpenProjectXml(inputXmlString);

            Assert.AreEqual(outputXmlString, Project.Current.ToXmlForSaving());
        }

        [Test]
        public void OpeningOldProjectXmlWithBoldFieldInFibLeavesFibIntact()
        {
            string inputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Bold Field Test 03"" themePath=""default"" format=""1.5"">" + Environment.NewLine +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" blockBackButton=""false"">" + Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" +
                @"<blank label=""a"" length=""10"" required=""false"" alternateLabel=""FIB 1 Alternate Label"">" +
                @"</blank>" +
                @"</paragraph></fib>" + Environment.NewLine +
                @"<fib label=""Q2""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.FullBeginFont +
                @"<b><field name=""Form 1:FIB 1 Alternate Label""/></b>" + XmlConstants.EndFont + @"</paragraph><paragraph " +
                @"indent=""0"" align=""left""><tabPositions><tabStop position=""720""/><tabStop " +
                @"position=""1440""/><tabStop position=""2160""/><tabStop position=""2880""/><tabStop " +
                @"position=""3600""/><tabStop position=""4320""/><tabStop position=""5040""/><tabStop " +
                @"position=""5760""/><tabStop position=""6480""/><tabStop position=""7200""/><tabStop " +
                @"position=""7920""/><tabStop position=""8640""/><tabStop position=""9360""/><tabStop " +
                @"position=""10080""/></tabPositions><blank label=""a"" length=""10"" " +
                @"required=""false""/></paragraph></fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            string outputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Bold Field Test 03"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" blockBackButton=""false"">" + Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" +
                @"<blank label=""a"" length=""10"" required=""false"" alternateLabel=""FIB 1 Alternate Label"">" +
                @"</blank>" +
                "</paragraph></fib>" + Environment.NewLine +
                @"<fib label=""Q2""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.FullBeginFont +
                @"<b><field name=""Form 1:FIB 1 Alternate Label""/></b>" + XmlConstants.EndFont + @"</paragraph><paragraph " +
                @"indent=""0"" align=""left""><tabPositions><tabStop position=""720""/><tabStop " +
                @"position=""1440""/><tabStop position=""2160""/><tabStop position=""2880""/><tabStop " +
                @"position=""3600""/><tabStop position=""4320""/><tabStop position=""5040""/><tabStop " +
                @"position=""5760""/><tabStop position=""6480""/><tabStop position=""7200""/><tabStop " +
                @"position=""7920""/><tabStop position=""8640""/><tabStop position=""9360""/><tabStop " +
                @"position=""10080""/></tabPositions>" +
                @"<blank label=""a"" length=""10"" required=""false""></blank>" +
                @"</paragraph></fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Util.OpenProjectXml(inputXmlString);

            Assert.AreEqual(outputXmlString, Project.Current.ToXmlForSaving());
        }

        [Test]
        public void OpeningOldProjectXmlWithBoldInFibLeavesFibIntact()
        {
            string inputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""FIB Bold Test 01"" themePath=""default"" format=""1.5"">" + Environment.NewLine +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" blockBackButton=""false"">" + Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.FullBeginFont + @"<b>Bold</b>" +
                XmlConstants.EndFont + @"<sp/><blank label=""a"" length=""10"" " +
                @"required=""false""/></paragraph></fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            string outputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""FIB Bold Test 01"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" blockBackButton=""false"">" + Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.FullBeginFont + @"<b>Bold</b>" +
                XmlConstants.EndFont + @"<sp/>" +
                @"<blank label=""a"" length=""10"" required=""false""></blank></paragraph></fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Util.OpenProjectXml(inputXmlString);

            Assert.AreEqual(outputXmlString, Project.Current.ToXmlForSaving());
        }

        [Test]
        public void OpeningOldProjectXmlWithBoldNonBlackInFibLeavesFibIntact()
        {
            string inputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Bold Field Test 04"" themePath=""default"" format=""1.5"">" + Environment.NewLine +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" blockBackButton=""false"">" + Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions><font face=""Arial"" size=""200"" color=""804000""><b>Bold Non Black</b>" +
                XmlConstants.EndFont + @"<sp/><blank label=""a"" length=""10"" " +
                @"required=""false""/></paragraph></fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            string outputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Bold Field Test 04"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" blockBackButton=""false"">" + Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions><font face=""Arial"" size=""200"" color=""804000""><b>Bold Non Black</b>" +
                XmlConstants.EndFont + @"<sp/>" +
                @"<blank label=""a"" length=""10"" required=""false""></blank></paragraph></fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Util.OpenProjectXml(inputXmlString);

            Assert.AreEqual(outputXmlString, Project.Current.ToXmlForSaving());
        }

        [Test]
        public void ProcessContainsCorrectStatements()
        {
            Assert.AreEqual("Set Variable 1 to 100", process.Lines[0].ToString());
            Assert.AreEqual("Get Records from Form 1", process.Lines[1].ToString());
            Assert.AreEqual("For Each Record in Records", process.Lines[2].ToString());
            Assert.AreEqual("(", process.Lines[3].ToString());
            Assert.AreEqual(")", process.Lines[4].ToString());
        }

        [Test]
        public void ProjectXmlContainsNewFormatNumber()
        {
            string projectXmlString = Project.Current.ToXml();
            IXmlElement element = new XmlElement(projectXmlString);
            string formatNumber = element.GetAttribute("format");

            int dotPosition = formatNumber.IndexOf(".");
            int majorVersion = Convert.ToInt32(formatNumber.Substring(0, dotPosition++));
            int minorVersion = Convert.ToInt32(formatNumber.Substring(dotPosition, formatNumber.Length - dotPosition));

            Assert.IsTrue(majorVersion >= 1 && minorVersion >= 6, "Project XML format number should be greater than or equal to 1.6");
        }

        [Test]
        public void RecordNodeDoesNotContainVariable()
        {
            setInsertionPointWithinForEachStatement();

            TreeNode recordNode = testPalette.FieldsTreeView.Nodes[2];

            Assert.AreEqual(1, recordNode.Nodes.Count);
            Assert.AreEqual("Form 1:Q1:a", recordNode.Nodes[0].Text);
        }

        [Test]
        public void RecordSetNodeDoesNotContainVariable()
        {
            selectGetStatement();

            TreeNode recordSetNode = testPalette.FieldsTreeView.Nodes[2];

            Assert.AreEqual(1, recordSetNode.Nodes.Count);
            Assert.AreEqual("Form 1:Q1:a", recordSetNode.Nodes[0].Text);
        }

        [Test]
        public void SelectingGetStatementDisplaysRecordSetNode()
        {
            selectGetStatement();

            Assert.AreEqual(3, testPalette.FieldsTreeView.Nodes.Count);
            Assert.AreEqual("Records", testPalette.FieldsTreeView.Nodes[2].Text);
        }

        [Test]
        public void SetAddsHiddenFieldAndSetStatement()
        {
            string inputXmlString =
                "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine +
                "<project name=\"Story 2151 Test 01\" themePath=\"default\" format=\"1.5\">" +
                "<forms>" +
                "<form name=\"Form 1\" startPoint=\"true\" themePath=\"default\" process=\"Process 1\" blockBackButton=\"false\">" +
                "<items>" +
                "</items>" +
                "</form>" +
                "</forms>" +
                "<processes>" +
                "<process name=\"Process 1\">" +
                "<set field=\"Score\" arithmeticAsText=\"false\">" +
                "<string value=\"100\"/>" +
                "</set>" +
                "</process>" +
                "</processes>" +
                "</project>";

            string outputXmlString =
                "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine +
                "<project name=\"Story 2151 Test 01\" themePath=\"default\" format=\"" + Project.XmlFormatVersion +
                "\" designerBuild=\"0\">" + Environment.NewLine +
                "<pageHeader></pageHeader>" +
                "<forms>" + Environment.NewLine +
                "<form name=\"Form 1\" startPoint=\"true\" themePath=\"default\" process=\"Process 1\" blockBackButton=\"false\">" +
                Environment.NewLine +
                "<items>" + Environment.NewLine +
                "<field name=\"Score\"/>" + Environment.NewLine +
                "</items>" + Environment.NewLine +
                "</form>" + Environment.NewLine +
                "</forms>" + Environment.NewLine +
                "<processes>" + Environment.NewLine +
                "<process name=\"Process 1\">" + Environment.NewLine +
                "<set field=\"Score\" arithmeticAsText=\"false\">" + Environment.NewLine +
                "<string value=\"100\"/>" + Environment.NewLine +
                "</set>" + Environment.NewLine +
                "<set field=\"Form 1:Score\" arithmeticAsText=\"false\">" + Environment.NewLine +
                "<string field=\"Score\"/>" + Environment.NewLine +
                "</set>" + Environment.NewLine +
                "</process>" + Environment.NewLine +
                "</processes>" + Environment.NewLine +
                "</project>" + Environment.NewLine;

            Util.OpenProjectXml(inputXmlString);

            // Console.WriteLine(Project.Current.ToXmlForSaving());
            Assert.AreEqual(outputXmlString, Project.Current.ToXmlForSaving());
        }

        [Test]
        public void SetInPreProcessAddsHiddenFieldAndSetStatement()
        {
            string inputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Story 2151 Test Preprocess"" themePath=""default"" format=""1.5"">" + Environment.NewLine +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" preProcess=""Process 1"" blockBackButton=""false"">" +
                Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.FullBeginFont +
                @"[Replace this with your question. Underscores create blanks.] " + XmlConstants.EndFont + @"<blank " +
                @"label=""a"" length=""10"" required=""false""/></paragraph></fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"<set field=""Score"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string value=""100""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            string outputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Story 2151 Test Preprocess"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" preProcess=""Process 1"" blockBackButton=""false"">" +
                Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.FullBeginFont +
                @"[Replace this with your question. Underscores create blanks.] " + XmlConstants.EndFont +
                @"<blank label=""a"" length=""10"" required=""false""></blank></paragraph></fib>" + Environment.NewLine +
                "<field name=\"Score\"/>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"<set field=""Score"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string value=""100""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"<set field=""Form 1:Score"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string field=""Score""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Util.OpenProjectXml(inputXmlString);

            // Console.WriteLine(Project.Current.ToXmlForSaving());
            Assert.AreEqual(outputXmlString, Project.Current.ToXmlForSaving());
        }

        [Test]
        public void SetInPreProcessWithPostProcessAddsHiddenFieldAndSetStatement()
        {
            string inputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Pre and Post Process"" themePath=""default"" format=""1.5"">" + Environment.NewLine +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" process=""Process 2"" preProcess=""Process 1"" blockBackButton=""false"">" +
                Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.FullBeginFont +
                @"[Replace this with your question. Underscores create blanks.] " + XmlConstants.EndFont + @"<blank " +
                @"label=""a"" length=""10"" required=""false""/></paragraph></fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"<set field=""Score"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string value=""100""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"<process name=""Process 2"">" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            string outputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Pre and Post Process"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" process=""Process 2"" preProcess=""Process 1"" blockBackButton=""false"">" +
                Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.FullBeginFont +
                @"[Replace this with your question. Underscores create blanks.] " + XmlConstants.EndFont +
                @"<blank label=""a"" length=""10"" required=""false""></blank></paragraph></fib>" + Environment.NewLine +
                "<field name=\"Score\"/>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"<set field=""Score"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string value=""100""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"<set field=""Form 1:Score"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string field=""Score""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"<process name=""Process 2"">" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Util.OpenProjectXml(inputXmlString);

            // Console.WriteLine(Project.Current.ToXmlForSaving());
            Assert.AreEqual(outputXmlString, Project.Current.ToXmlForSaving());
        }

        [Test]
        public void SetInSharedProcessAddsHiddenFieldsAndSetStatements()
        {
            string inputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Story 2151 Test Multiple Forms"" themePath=""default"" format=""1.5"">" + Environment.NewLine +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" preProcess=""Process 1"" blockBackButton=""false"">" +
                Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.FullBeginFont +
                @"[Replace this with your question. Underscores create blanks.] " + XmlConstants.EndFont + @"<blank " +
                @"label=""a"" length=""10"" required=""false""/></paragraph></fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"<form name=""Form 2"" startPoint=""false"" themePath=""default"" process=""Process 1"" blockBackButton=""false"">" +
                Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.FullBeginFont +
                @"[Replace this with your question. Underscores create blanks.] " + XmlConstants.EndFont + @"<blank " +
                @"label=""a"" length=""10"" required=""false""/></paragraph></fib>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"<set field=""Score"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string value=""100""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            string outputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Story 2151 Test Multiple Forms"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" preProcess=""Process 1"" blockBackButton=""false"">" +
                Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.FullBeginFont +
                @"[Replace this with your question. Underscores create blanks.] " + XmlConstants.EndFont +
                @"<blank label=""a"" length=""10"" required=""false""></blank>" +
                @"</paragraph></fib>" + Environment.NewLine +
                "<field name=\"Score\"/>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"<form name=""Form 2"" startPoint=""false"" themePath=""default"" process=""Process 1"" blockBackButton=""false"">" +
                Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.FullBeginFont +
                @"[Replace this with your question. Underscores create blanks.] " + XmlConstants.EndFont +
                @"<blank label=""a"" length=""10"" required=""false""></blank>" +
                @"</paragraph></fib>" + Environment.NewLine +
                "<field name=\"Score\"/>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"<set field=""Score"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string value=""100""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"<set field=""Form 2:Score"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string field=""Score""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"<set field=""Form 1:Score"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string field=""Score""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Util.OpenProjectXml(inputXmlString);

            // Console.WriteLine(Project.Current.ToXmlForSaving());
            Assert.AreEqual(outputXmlString, Project.Current.ToXmlForSaving());
        }

        [Test]
        public void SetInsideIfAddsHiddenFieldAndSetStatement()
        {
            string inputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Story 2151 Test If Set"" themePath=""default"" format=""1.5"">" + Environment.NewLine +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" process=""Process 1"" blockBackButton=""false"">" +
                Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<mc label=""Q1"" onlyone=""true"" required=""false""" + XmlConstants.DefaultMcqItemStyleAttribute +
                @"><question><paragraph indent=""0"" " +
                @"align=""left""><tabPositions><tabStop position=""720""/><tabStop position=""1440""/><tabStop " +
                @"position=""2160""/><tabStop position=""2880""/><tabStop position=""3600""/><tabStop " +
                @"position=""4320""/><tabStop position=""5040""/><tabStop position=""5760""/><tabStop " +
                @"position=""6480""/><tabStop position=""7200""/><tabStop position=""7920""/><tabStop " +
                @"position=""8640""/><tabStop position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.BeginFont +
                @"[Replace this with your question. Use Enter key to add " +
                @"choices below.]" + XmlConstants.EndFont + @"</paragraph></question><choice label=""a""><paragraph indent=""0"" " +
                @"align=""left""><tabPositions><tabStop position=""720""/><tabStop position=""1440""/><tabStop " +
                @"position=""2160""/><tabStop position=""2880""/><tabStop position=""3600""/><tabStop " +
                @"position=""4320""/><tabStop position=""5040""/><tabStop position=""5760""/><tabStop " +
                @"position=""6480""/><tabStop position=""7200""/><tabStop position=""7920""/><tabStop " +
                @"position=""8640""/><tabStop position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.BeginFont +
                @"A" + XmlConstants.EndFont + @"</paragraph></choice><choice " +
                @"label=""b""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.BeginFont + @"B" + XmlConstants.EndFont +
                @"</paragraph></choice></mc>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"<if>" + Environment.NewLine +
                @"<conditions>" + Environment.NewLine +
                @"<mcEquals field=""Form 1:Q1"" value=""a""/>" + Environment.NewLine +
                @"</conditions>" + Environment.NewLine +
                @"<trueSet>" + Environment.NewLine +
                @"<set field=""Score"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string value=""100""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"</trueSet>" + Environment.NewLine +
                @"</if>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            string outputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Story 2151 Test If Set"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" process=""Process 1"" blockBackButton=""false"">" +
                Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<mc label=""Q1"" onlyone=""true"" required=""false""" + XmlConstants.DefaultMcqItemStyleAttribute +
                @"><question><paragraph indent=""0"" " +
                @"align=""left""><tabPositions><tabStop position=""720""/><tabStop position=""1440""/><tabStop " +
                @"position=""2160""/><tabStop position=""2880""/><tabStop position=""3600""/><tabStop " +
                @"position=""4320""/><tabStop position=""5040""/><tabStop position=""5760""/><tabStop " +
                @"position=""6480""/><tabStop position=""7200""/><tabStop position=""7920""/><tabStop " +
                @"position=""8640""/><tabStop position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.BeginFont +
                @"[Replace this with your question. Use Enter key to add " +
                @"choices below.]" + XmlConstants.EndFont + @"</paragraph></question><choice label=""a""><paragraph indent=""0"" " +
                @"align=""left""><tabPositions><tabStop position=""720""/><tabStop position=""1440""/><tabStop " +
                @"position=""2160""/><tabStop position=""2880""/><tabStop position=""3600""/><tabStop " +
                @"position=""4320""/><tabStop position=""5040""/><tabStop position=""5760""/><tabStop " +
                @"position=""6480""/><tabStop position=""7200""/><tabStop position=""7920""/><tabStop " +
                @"position=""8640""/><tabStop position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.BeginFont +
                @"A" + XmlConstants.EndFont + @"</paragraph></choice><choice " +
                @"label=""b""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions>" + XmlConstants.BeginFont + @"B" + XmlConstants.EndFont +
                @"</paragraph></choice></mc>" + Environment.NewLine +
                "<field name=\"Score\"/>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"<if>" + Environment.NewLine +
                @"<conditions>" + Environment.NewLine +
                @"<mcEquals field=""Form 1:Q1"" value=""a""/>" + Environment.NewLine +
                @"</conditions>" + Environment.NewLine +
                @"<trueSet>" + Environment.NewLine +
                @"<set field=""Score"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string value=""100""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"<set field=""Form 1:Score"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string field=""Score""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"</trueSet>" + Environment.NewLine +
                @"</if>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Util.OpenProjectXml(inputXmlString);

            // Console.WriteLine(Project.Current.ToXmlForSaving());
            Assert.AreEqual(outputXmlString, Project.Current.ToXmlForSaving());
        }

        [Test]
        public void SettingInsertionPointWithinforEachStatementDisplaysRecordNode()
        {
            setInsertionPointWithinForEachStatement();

            Assert.AreEqual(3, testPalette.FieldsTreeView.Nodes.Count);
            Assert.AreEqual("Record", testPalette.FieldsTreeView.Nodes[2].Text);
        }

        [Test]
        public void SetWithEmptyFormAddsHiddenFieldAndSetStatement()
        {
            string inputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Empty Form Test 01"" themePath=""default"" format=""1.5"">" + Environment.NewLine +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" process=""Process 1"" blockBackButton=""false"">" +
                Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"<set field=""Score"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string value=""100""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            string outputXmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Empty Form Test 01"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                @""" designerBuild=""0"">" + Environment.NewLine +
                @"<pageHeader></pageHeader>" +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" process=""Process 1"" blockBackButton=""false"">" +
                Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<field name=""Score""/>" + Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"<processes>" + Environment.NewLine +
                @"<process name=""Process 1"">" + Environment.NewLine +
                @"<set field=""Score"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string value=""100""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"<set field=""Form 1:Score"" arithmeticAsText=""false"">" + Environment.NewLine +
                @"<string field=""Score""/>" + Environment.NewLine +
                @"</set>" + Environment.NewLine +
                @"</process>" + Environment.NewLine +
                @"</processes>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Util.OpenProjectXml(inputXmlString);

            // Console.WriteLine(Project.Current.ToXmlForSaving());
            Assert.AreEqual(outputXmlString, Project.Current.ToXmlForSaving());
        }
    }
}