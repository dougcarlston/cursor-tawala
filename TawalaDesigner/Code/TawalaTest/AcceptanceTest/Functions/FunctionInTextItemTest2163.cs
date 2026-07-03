// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.Functions
{
    /// <summary>
    /// Acceptance tests for story 2163 (Designer uses Function in Text Item).
    /// </summary>
    [TestFixture]
    public class FunctionInTextItemTest2163 : FunctionTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            form = Project.Current.AddForm();

            fibItem = new FibItem();
            fibItem.Text = "FIB: __________";
            form.ItemList.Add(fibItem);
            blank = fibItem.BlankList[0];

            textItem = new TextItem();
            form.ItemList.Add(textItem);
        }

        #endregion

        private IForm form;
        private FibItem fibItem;
        private Blank blank;
        private TextItem textItem;
        private const string defaultFibStyleAtttribute = " style=\"topLabels\"";

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            functionSetup();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            functionTearDown();
        }

        private IFunction createItemizationTableFunction()
        {
            IFunction function = functions["itemization-table"].CreateInstance();

            IParameterInfo compositeCollection = function.Info.Parameters["column"];
            var collection = Activator.CreateInstance(compositeCollection.PropertyType) as ICompositeParameterCollection;

            ICompositeParameter composite1 = collection.CreateItem();

            composite1["header"] = new FunctionCompoundExpression(new XmlElement("<container><string value=\"Test\"/></container>"));
            composite1["contents"] = new FunctionContentsField(blank);

            collection.Add(composite1);

            Assert.AreEqual(1, collection.Count);

            compositeCollection.SetValue(function, collection);

            var functionConditions = new FunctionFilterConditions();
            functionConditions.Forms = new FunctionFormCollection(form);
            function["conditions"] = functionConditions;

            Project.FunctionMapById.AddUnique(function);

            return function;
        }

        [Test]
        [Ignore("Runs green by itself, but runs red when entire AcceptanceTest suite is run.")]
        public void ForwardBlankReferenceProducesCorrectXml()
        {
            string xmlString = @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                               @"<project name=""ForwardBlankReference"" themePath=""default""" + Project.XmlFormatVersion + ">" +
                               Environment.NewLine +
                               @"<forms>" + Environment.NewLine +
                               @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" blockBackButton=""false"">" +
                               Environment.NewLine + @"<items>" + Environment.NewLine + @"<text label=""T1""" +
                               XmlConstants.DefaultTextItemStyleAttribute +
                               @"><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                               @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                               @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                               @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                               @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                               @"position=""9360""/><tabStop position=""10080""/></tabPositions><font face=""Arial"" size=""200"" " +
                               @"color=""000000"">" + @"<simple-list version=""1""><simple-list-field>Record:Form " +
                               @"1:Q1:a</simple-list-field>" + @"</simple-list>" + @"</font></paragraph></text>" + Environment.NewLine +
                               @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                               @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                               @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                               @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                               @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                               @"position=""9360""/><tabStop position=""10080""/></tabPositions><font face=""Arial"" size=""200"" " +
                               @"color=""000000"">Name: </font><blank label=""a"" length=""10"" required=""false""></blank></paragraph></fib>" +
                               Environment.NewLine + @"</items>" + Environment.NewLine + @"</form>" + Environment.NewLine + @"</forms>" +
                               Environment.NewLine + @"</project>" + Environment.NewLine;

            Util.OpenProjectXml(xmlString);

            Assert.AreEqual(xmlString, Project.Current.ToXmlForSaving());
        }

        [Test]
        [Ignore("Runs green by itself, but runs red when entire AcceptanceTest suite is run.")]
        public void ForwardMCQReferenceProducesCorrectXml()
        {
            string xmlString = @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                               @"<project name=""MCQ Function in Text Item Test 03"" themePath=""default"" format=""1.7"">" +
                               Environment.NewLine + @"<forms>" + Environment.NewLine +
                               @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" blockBackButton=""false"">" +
                               Environment.NewLine + @"<items>" +
                               Environment.NewLine + @"<text label=""T1""" + XmlConstants.DefaultTextItemStyleAttribute +
                               @"><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                               @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                               @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                               @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                               @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                               @"position=""9360""/><tabStop position=""10080""/></tabPositions><font face=""Arial"" size=""200"" " +
                               @"color=""000000""><popular-choice-correlation-table " +
                               @"version=""1""><rank>1</rank><choice-available-field-name>Record:Form " +
                               @"1:Q1</choice-available-field-name><choice-preferred-field-name>Record:Form " +
                               @"1:Q2</choice-preferred-field-name><popular-choice-display-field-name>Record:Form " +
                               @"1:Q3:a</popular-choice-display-field-name></popular-choice-correlation-table></font></paragraph></text>" +
                               Environment.NewLine +
                               @"<mc label=""Q1"" onlyone=""true"" required=""false""><question><paragraph indent=""0"" " +
                               @"align=""left""><tabPositions><tabStop position=""720""/><tabStop position=""1440""/><tabStop " +
                               @"position=""2160""/><tabStop position=""2880""/><tabStop position=""3600""/><tabStop " +
                               @"position=""4320""/><tabStop position=""5040""/><tabStop position=""5760""/><tabStop " +
                               @"position=""6480""/><tabStop position=""7200""/><tabStop position=""7920""/><tabStop " +
                               @"position=""8640""/><tabStop position=""9360""/><tabStop position=""10080""/></tabPositions><font " +
                               @"face=""Arial"" size=""200"" color=""000000"">MCQ1:</font></paragraph></question><choice " +
                               @"label=""a""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                               @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                               @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                               @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                               @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                               @"position=""9360""/><tabStop position=""10080""/></tabPositions></paragraph></choice></mc>" +
                               Environment.NewLine +
                               @"<mc label=""Q2"" onlyone=""true"" required=""false""><question><paragraph indent=""0"" " +
                               @"align=""left""><tabPositions><tabStop position=""720""/><tabStop position=""1440""/><tabStop " +
                               @"position=""2160""/><tabStop position=""2880""/><tabStop position=""3600""/><tabStop " +
                               @"position=""4320""/><tabStop position=""5040""/><tabStop position=""5760""/><tabStop " +
                               @"position=""6480""/><tabStop position=""7200""/><tabStop position=""7920""/><tabStop " +
                               @"position=""8640""/><tabStop position=""9360""/><tabStop position=""10080""/></tabPositions><font " +
                               @"face=""Arial"" size=""200"" color=""000000"">MCQ2:</font></paragraph></question><choice " +
                               @"label=""a""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                               @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                               @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                               @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                               @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                               @"position=""9360""/><tabStop position=""10080""/></tabPositions></paragraph></choice></mc>" +
                               Environment.NewLine + @"<fib label=""Q3""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                               @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                               @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                               @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                               @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                               @"position=""9360""/><tabStop position=""10080""/></tabPositions><font face=""Arial"" size=""200"" " +
                               @"color=""000000"">FIB: </font><blank label=""a"" length=""10"" required=""false""></blank></paragraph></fib>" +
                               Environment.NewLine + @"</items>" + Environment.NewLine + @"</form>" + Environment.NewLine + @"</forms>" +
                               Environment.NewLine + @"</project>" + Environment.NewLine;

            Util.OpenProjectXml(xmlString);

            Assert.AreEqual(xmlString, Project.Current.ToXmlForSaving());
        }

        [Test]
        [Ignore("Runs green by itself, but runs red when entire AcceptanceTest suite is run.")]
        public void ForwardReferenceInFieldTableProducesCorrectXml()
        {
            string xmlString = @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                               @"<project name=""Function Test 07"" themePath=""default"" format=""1.7"">" + Environment.NewLine +
                               @"<forms>" + Environment.NewLine +
                               @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" blockBackButton=""false"">" +
                               Environment.NewLine + @"<items>" + Environment.NewLine + @"<text label=""T1""" +
                               XmlConstants.DefaultTextItemStyleAttribute +
                               @"><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                               @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                               @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                               @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                               @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                               @"position=""9360""/><tabStop position=""10080""/></tabPositions><font face=""Arial"" size=""200"" " +
                               @"color=""000000""><itemization-table " +
                               @"version=""1""><number-of-columns>1</number-of-columns><column><header>Test</header><contents><field " +
                               @"name=""Record:Form 1:Q1:a"" /></contents></column><conditions><form name=""Form " +
                               @"1""/></conditions></itemization-table></font></paragraph></text>" + Environment.NewLine +
                               @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                               @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                               @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                               @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                               @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                               @"position=""9360""/><tabStop position=""10080""/></tabPositions><font face=""Arial"" size=""200"" " +
                               @"color=""000000"">FIB: </font><blank label=""a"" length=""10"" required=""false""></blank></paragraph></fib>" +
                               Environment.NewLine + @"</items>" + Environment.NewLine + @"</form>" + Environment.NewLine + @"</forms>" +
                               Environment.NewLine + @"</project>" + Environment.NewLine;

            Util.OpenProjectXml(xmlString);

            Assert.AreEqual(xmlString, Project.Current.ToXmlForSaving());
        }

        [Test]
        public void SettingTextItemFunctionRtfProducesCorrectXml()
        {
            string textItemRtfString = @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + Environment.NewLine +
                                       @"{\f0\fswiss\fcharset0\fprq2 Arial;}" + Environment.NewLine +
                                       @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + Environment.NewLine +
                                       @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;\red0\green0\blue0;}" + Environment.NewLine +
                                       @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" +
                                       Environment.NewLine + @"{\*\generator TX_RTF32 12.0.500.502;}" + Environment.NewLine +
                                       @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard\itap0\tx720\tx1" +
                                       @"440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\tx9360\tx10080\plain\f0\fs2" +
                                       @"0\cf3{\*\txfieldstart\txfieldtype0\txfieldflags219\" +
                                       String.Format(@"txfielddataval{0}\", createItemizationTableFunction().InstanceId) + @"txfielddata " +
                                       @"4600460024003100330035003100370030000000}<<FIELD TABLE(1, Record:Form 1:Q1:a, " +
                                       @"...)>>{\*\txfieldend}\par }";

            textItem.Rtf = textItemRtfString;

            string expectedXml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                                 @"<project name=""Untitled"" themePath=""default"" format=""" + Project.XmlFormatVersion +
                                 @""" designerBuild=""0"">" + Environment.NewLine + @"<pageHeader></pageHeader>" + @"<forms>" +
                                 Environment.NewLine +
                                 @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" blockBackButton=""false"">" +
                                 Environment.NewLine + @"<items>" + Environment.NewLine + "<fib label=\"Q1\"" + defaultFibStyleAtttribute +
                                 ">" +
                                 @"<paragraph indent=""0"" align=""left""><font face=""Arial"" size=""180"" color=""000000"">FIB: </font>" +
                                 @"<blank label=""a"" length=""10"" required=""false""></blank>" +
                                 @"</paragraph></fib>" + Environment.NewLine +
                                 @"<text label=""T1""" + XmlConstants.DefaultTextItemStyleAttribute +
                                 @"><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                                 @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                                 @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                                 @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                                 @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                                 @"position=""9360""/><tabStop position=""10080""/></tabPositions><font face=""Arial"" size=""200"" color=""000000"">" +
								 @"<itemization-table version=""2"">" + @"<show-print-control>false</show-print-control><show-export-control>false</show-export-control>" +
                                 @"<column><header><string value=""Test""/>" + Environment.NewLine +
                                 @"</header><contents><field name=""Form 1:Q1:a"" /></contents></column>" +
                                 @"<conditions><form name=""Form 1"" /></conditions>" + @"</itemization-table>" +
                                 @"</font></paragraph></text>" + Environment.NewLine + @"</items>" + Environment.NewLine + @"</form>" +
                                 Environment.NewLine + @"</forms>" + Environment.NewLine + @"</project>" + Environment.NewLine;

            Assert.AreEqual(expectedXml, Project.Current.ToXmlForSaving());
        }
    }
}