// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using Tawala.Common;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Forms;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FormItems
{
    /// <summary>
    /// Acceptance tests for story 2388(Dynamic MCQs).
    /// </summary>
    [TestFixture]
    public class DynamicMCQs2388 : FunctionTestBase
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            functionSetup();

            Util.NewTestProject();
        }

        [TearDown]
        public void TearDown()
        {
            functionTearDown();
        }

        #endregion

        private static readonly string dynamicMcqXml =
            "<mc label=\"Q1\" onlyone=\"true\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute +
            "><question><paragraph indent=\"0\" align=\"left\">" +
            "<tabPositions><tabStop position=\"2880\"/></tabPositions><font face=\"Arial\" size=\"200\" " +
            "color=\"000000\">[Replace this with your question. Use Enter key to add choices " +
            "below.]</font></paragraph></question>" +
            "<data-provider><dynamic-mcq version=\"1\">" +
            "<display-expression>" +
            "<field name=\"Record:Form 1:Q1:a\"/>" + Environment.NewLine +
            "</display-expression>" +
            "<value-expression><field name=\"Record:Form 1:Field1\"/>" + Environment.NewLine +
            "</value-expression>" +
            "<sort-expression>" +
            "</sort-expression>" +
            "<record-selector><form name=\"Form 1\" /><conditions>" + Environment.NewLine +
            "<equals field=\"Record:Form 1:Q1:a\">" + Environment.NewLine +
            "<string value=\"test\"/>" + Environment.NewLine +
            "</equals>" + Environment.NewLine +
            "</conditions></record-selector></dynamic-mcq></data-provider></mc>" + Environment.NewLine;

        private static readonly string projectXmlWithDynamicMcq =
            "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine +
            "<project name=\"DynamicMCQ\" themePath=\"default\" format=\"1.9\">" + Environment.NewLine +
            "<forms>" + Environment.NewLine +
            "<form name=\"Form 1\" startPoint=\"true\" themePath=\"default\" blockBackButton=\"false\">" + Environment.NewLine +
            "<items>" + Environment.NewLine +
            "<fib label=\"Q1\" style=\"topLabels\"><paragraph indent=\"0\" align=\"left\"><tabPositions><tabStop " +
            "position=\"2880\"/></tabPositions><font face=\"Arial\" size=\"200\" color=\"000000\">[Replace this with your question. " +
            "Underscores create blanks.] </font><blank label=\"a\" length=\"20\" required=\"false\"></blank></paragraph></fib>" +
            Environment.NewLine +
            "<field name=\"Field1\"/>" + Environment.NewLine +
            "</items>" + Environment.NewLine +
            "</form>" + Environment.NewLine +
            "<form name=\"Form 2\" startPoint=\"false\" themePath=\"default\" blockBackButton=\"false\">" + Environment.NewLine +
            "<items>" + Environment.NewLine +
            "<mc label=\"Q1\" onlyone=\"true\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute +
            "><question><paragraph indent=\"0\" " +
            "align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions><font face=\"Arial\" size=\"200\" " +
            "color=\"000000\">[Replace this with your question. Use Enter key to add choices " +
            "below.]</font></paragraph></question><data-provider><dynamic-mcq version=\"1\"><display-expression><field " +
            "name=\"Record:Form 1:Q1:a\"/></display-expression><value-expression><field name=\"Record:Form 1:Field1\" " +
            "/></value-expression><record-selector><form name=\"Form 1\" /><conditions>" + Environment.NewLine +
            "<equals field=\"Record:Form 1:Q1:a\">" + Environment.NewLine +
            "<string value=\"test\"/>" + Environment.NewLine +
            "</equals>" + Environment.NewLine +
            "</conditions></record-selector></dynamic-mcq></data-provider></mc>" + Environment.NewLine +
            "</items>" + Environment.NewLine +
            "</form>" + Environment.NewLine +
            "</forms>" + Environment.NewLine +
            "</project>";

        private IFunction createDynamicMcqFunction()
        {
            return functions["dynamic-mcq"].CreateInstance();
        }

        [Test]
        public void ChoiceDisplayParameterRecordListName()
        {
            IParameterInfo parameterInfo = functions["dynamic-mcq"].Parameters["display-expression"];
            Assert.AreEqual("Record", parameterInfo.RecordListName);
        }

        [Test]
        public void ChoiceDisplayParameterRestrictedToRecordIteration()
        {
            IParameterInfo parameterInfo = functions["dynamic-mcq"].Parameters["display-expression"];

            Assert.IsTrue(parameterInfo.Restrictions == ParameterRestrictions.RecordIterationAlways);
        }

        [Test]
        public void ChoiceValueParameterRecordListName()
        {
            IParameterInfo parameterInfo = functions["dynamic-mcq"].Parameters["value-expression"];
            Assert.AreEqual("Record", parameterInfo.RecordListName);
        }

        [Test]
        public void ChoiceValueParameterRestrictedToRecordIteration()
        {
            IParameterInfo parameterInfo = functions["dynamic-mcq"].Parameters["value-expression"];
            Assert.IsTrue(parameterInfo.Restrictions == ParameterRestrictions.RecordIterationAlways);
        }

        [Test]
        public void DefaultMCQDoesNotHaveAttachedDynamicMCQFunction()
        {
            IForm form = Project.Current.AddForm();

            var mcq = new McqItem();
            form.ItemList.Add(mcq);

            string xml = mcq.ToXml("Q1");

            Assert.IsTrue(xml.Contains("<mc"));
            Assert.IsFalse(xml.Contains("<dynamic-mcq"));
        }

        [Test]
        public void DefaultParameterRestrictionsForOtherFunctions()
        {
            IParameterInfo parameterInfo = functions["record-count"].Parameters["form-name"];
            Assert.IsTrue(parameterInfo.Restrictions == ParameterRestrictions.Default);
            Assert.AreEqual("", parameterInfo.RecordListName);
        }

        [Test]
        public void DynamicMCQCanBeSerialized()
        {
            IForm form = Project.Current.AddForm();

            var mcq = new McqItem();
            mcq.DataSourceFunction = createDynamicMcqFunction();
            form.ItemList.Add(mcq);

            try
            {
                McqItem mcqCopy = Cloner.Clone(mcq);

                Assert.IsNotNull(mcqCopy.DataSourceFunction);
            }
            catch (SerializationException se)
            {
                Assert.Fail(se.ToString());
            }
        }

        [Test]
        public void DynamicMCQFunctionExists()
        {
            Assert.IsTrue(functions.Contains("dynamic-mcq"));
        }

        [Test]
        public void DynamicMcqFunctionRestoredFromProjectXml()
        {
            Util.OpenProjectXml(projectXmlWithDynamicMcq);

            IForm form = Project.Current.GetForm("Form 2");
            var mcq = form.ItemList[0] as McqItem;

            Assert.IsNotNull(mcq.DataSourceFunction);
            Assert.AreEqual(dynamicMcqXml, mcq.ToXml("Q1"));
        }

        [Test]
        public void DynamicMcqHasFunctionXml()
        {
            IForm form = Project.Current.AddForm();

            var mcq = new McqItem();
            mcq.DataSourceFunction = createDynamicMcqFunction();
            form.ItemList.Add(mcq);

            string xml = mcq.ToXml("Q1");

            Assert.IsTrue(xml.Contains("<mc"));
            Assert.IsTrue(xml.Contains("<dynamic-mcq"));
        }
    }
}