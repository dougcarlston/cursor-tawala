// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Forms;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FormItems
{
    /// <summary>
    /// Acceptance tests for story 2806(Dynamic MCQs with Expressions).
    /// </summary>
    [TestFixture]
    public class DynamicMCQsWithExpressions2806 : FunctionTestBase
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            functionSetup();

            Util.NewTestProject();

            form1 = Project.Current.AddForm();

            var fibItem1 = new FibItem();
            blank1 = fibItem1.BlankList[0];
            form1.ItemList.Add(fibItem1);

            var fibItem2 = new FibItem();
            blank2 = fibItem2.BlankList[0];
            form1.ItemList.Add(fibItem2);
        }

        [TearDown]
        public void TearDown()
        {
            functionTearDown();

            form1 = null;
            blank1 = null;
            blank2 = null;
        }

        #endregion

        private IForm form1;
        private Blank blank1;
        private Blank blank2;

        private static readonly string dynamicMcqXml =
            "<mc label=\"Q3\" onlyone=\"true\" required=\"false\"" + XmlConstants.DefaultMcqItemStyleAttribute +
            "><question><paragraph indent=\"0\" " +
            "align=\"left\"><tabPositions><tabStop position=\"2880\"/></tabPositions><font face=\"Arial\" size=\"200\" " +
            "color=\"000000\">~@!~[Replace this with your question. Use Enter key to add choices " +
            "below.]~@!~</font></paragraph></question>" +
            "<data-provider><dynamic-mcq version=\"1\">" +
            "<display-expression>" +
            "<field name=\"Record:Form 1:Q1:a\"/>" + Environment.NewLine +
            "<string value=\" and \"/>" + Environment.NewLine +
            "<field name=\"Record:Form 1:Q2:a\"/>" + Environment.NewLine +
            "</display-expression>" +
            "<value-expression>" +
            "<field name=\"Record:Form 1:Q2:a\"/>" + Environment.NewLine +
            "<string value=\" and \"/>" + Environment.NewLine +
            "<field name=\"Record:Form 1:Q1:a\"/>" + Environment.NewLine +
            "</value-expression>" +
            "<sort-expression>" +
            "</sort-expression>" +
            "<record-selector></record-selector></dynamic-mcq></data-provider></mc>";

        private IFunction createDynamicMcqFunction()
        {
            return functions["dynamic-mcq"].CreateInstance();
        }

        [Test]
        public void DynamicMCQWithExpressionsGeneratesCorrectXml()
        {
            var mcq = new McqItem();

            IFunction function = createDynamicMcqFunction();

            Assert.IsNotNull(function);

            var recordField1 = new RecordField(new Record("Record"), blank1);
            var recordField2 = new RecordField(new Record("Record"), blank2);

            function.SetValue("display-expression",
                              new FunctionCompoundExpression(recordField1.FieldString + " and " + recordField2.FieldString));
            function.SetValue("value-expression",
                              new FunctionCompoundExpression(recordField2.FieldString + " and " + recordField1.FieldString));

            mcq.DataSourceFunction = function;

            form1.ItemList.Add(mcq);

            string resultXml = Project.Current.ToXml();

            Assert.IsTrue(resultXml.Contains(dynamicMcqXml));
        }
    }
}