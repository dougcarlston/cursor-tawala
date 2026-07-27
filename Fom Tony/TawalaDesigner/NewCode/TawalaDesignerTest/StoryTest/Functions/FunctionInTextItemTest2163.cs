using System;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.StoryTest.Functions
{
    [TestFixture]
    public class FunctionInTextItemTest2163 : FunctionTestBase
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            form = Project.Current.AddForm();

            fibItem = new NewFibItem();
            form.ItemList.Add(fibItem);
            blank = fibItem.BlankList[0];

            textItem = new NewTextItem();
            form.ItemList.Add(textItem);
        }

        #endregion

        private IForm form;
        private IFibItem fibItem;
        private IBlank blank;
        private ITextItem textItem;
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

            function.Info.Parameters["number-of-columns"].SetValue(function, 1);

            IParameterInfo compositeCollection = function.Info.Parameters["column"];
            var collection = Activator.CreateInstance(compositeCollection.PropertyType) as ICompositeParameterCollection;
            string compositeName = compositeCollection.PropertyType.FullName.Replace("__composite_collection_",
                                                                                     "__composite_");

            Type compositeType = Type.GetType(compositeName);
            var composite1 = Activator.CreateInstance(compositeType) as ICompositeParameter;
            var expression1 =
                new FunctionCompoundExpression(new XmlElement("<container><string value=\"Test\"/></container>"));
            composite1.GetType().GetProperty("Header").SetValue(composite1, expression1, null);
            var contents1 = new FunctionContentsField(blank);
            composite1.GetType().GetProperty("Contents").SetValue(composite1, contents1, null);

            collection.Add(composite1);

            Assert.AreEqual(1, collection.Count);

            compositeCollection.SetValue(function, collection);

            var functionConditions = new FunctionConditions();
            functionConditions.Forms = new FunctionFormCollection(form);
            function.SetValue("conditions", functionConditions);

            Project.FunctionMapById.AddUnique(function);

            return function;
        }

        [Test]
        [Ignore("Runs green by itself, but runs red when entire AcceptanceTest suite is run.")]
        public void ForwardBlankReferenceProducesCorrectXml()
        {
            string xmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""ForwardBlankReference"" themePath=""default"" format=""1.7"">" + Environment.NewLine +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"">" + Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<text label=""T1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions><font face=""Arial"" size=""200"" " +
                @"color=""000000"">" +
                @"<simple-list version=""1""><simple-list-field>Record:Form " +
                @"1:Q1:a</simple-list-field>" +
                @"</simple-list>" +
                @"</font></paragraph></text>" + Environment.NewLine +
                @"<fib label=""Q1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions><font face=""Arial"" size=""200"" " +
                @"color=""000000"">Name: </font><blank label=""a"" length=""10"" required=""false""/></paragraph></fib>" +
                Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Util.OpenProjectXml(xmlString);

            Assert.AreEqual(xmlString, Project.Current.ToXmlForSaving());
        }

        [Test]
        [Ignore("Runs green by itself, but runs red when entire AcceptanceTest suite is run.")]
        public void ForwardMCQReferenceProducesCorrectXml()
        {
            string xmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""MCQ Function in Text Item Test 03"" themePath=""default"" format=""1.7"">" +
                Environment.NewLine +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"">" + Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<text label=""T1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
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
                Environment.NewLine +
                @"<fib label=""Q3""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
                @"position=""720""/><tabStop position=""1440""/><tabStop position=""2160""/><tabStop " +
                @"position=""2880""/><tabStop position=""3600""/><tabStop position=""4320""/><tabStop " +
                @"position=""5040""/><tabStop position=""5760""/><tabStop position=""6480""/><tabStop " +
                @"position=""7200""/><tabStop position=""7920""/><tabStop position=""8640""/><tabStop " +
                @"position=""9360""/><tabStop position=""10080""/></tabPositions><font face=""Arial"" size=""200"" " +
                @"color=""000000"">FIB: </font><blank label=""a"" length=""10"" required=""false""/></paragraph></fib>" +
                Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Util.OpenProjectXml(xmlString);

            Assert.AreEqual(xmlString, Project.Current.ToXmlForSaving());
        }

        [Test]
        [Ignore("Runs green by itself, but runs red when entire AcceptanceTest suite is run.")]
        public void ForwardReferenceInFieldTableProducesCorrectXml()
        {
            string xmlString =
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
                @"<project name=""Function Test 07"" themePath=""default"" format=""1.7"">" + Environment.NewLine +
                @"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"">" + Environment.NewLine +
                @"<items>" + Environment.NewLine +
                @"<text label=""T1""><paragraph indent=""0"" align=""left""><tabPositions><tabStop " +
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
                @"color=""000000"">FIB: </font><blank label=""a"" length=""10"" required=""false""/></paragraph></fib>" +
                Environment.NewLine +
                @"</items>" + Environment.NewLine +
                @"</form>" + Environment.NewLine +
                @"</forms>" + Environment.NewLine +
                @"</project>" + Environment.NewLine;

            Util.OpenProjectXml(xmlString);

            Assert.AreEqual(xmlString, Project.Current.ToXmlForSaving());
        }
    }
}