// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.ConditionsTest
{
    /// <summary>
    /// Acceptance tests for story 2792(Designer matches Dynamic MCQ choice with a Field).
    /// </summary>
    [TestFixture]
    public class McqIsAnswered1344
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            oneIsBlank = MCOneOperator.List[MCOneOperator.Ops.mcIsBlank];
            oneIsNotBlank = MCOneOperator.List[MCOneOperator.Ops.mcIsNotBlank];
            manyIsBlank = MCManyOperator.List[MCManyOperator.Ops.mcIsBlank];
            manyIsNotBlank = MCManyOperator.List[MCManyOperator.Ops.mcIsNotBlank];

            form = Project.Current.AddForm();

            mcqItem = new McqItem();
            choices = new ChoiceList
                      {
                          new Choice("Choice 1"),
                          new Choice("Choice 2"),
                          new Choice("Choice 3")
                      };
            form.ItemList.Add(mcqItem);

            process = Project.Current.AddProcess();
        }

        #endregion

        private IForm form;
        private McqItem mcqItem;
        private ChoiceList choices;
        private Process process;

        private MCOneOperator oneIsBlank;
        private MCOneOperator oneIsNotBlank;
        private MCManyOperator manyIsBlank;
        private MCManyOperator manyIsNotBlank;

        private void addIfStatementWithCondition(Condition condition)
        {
            var ifStatement = new IfStatement(new Tawala.Projects.Conditions(condition));
            var ifList = new ProcessLineList(ifStatement);
            process.Lines.Add(ifList);
        }

        private static readonly string expectedXmlFormat =
            "<process name=\"Process 1\">" + Environment.NewLine +
            "<if>" + Environment.NewLine +
            "<conditions>" + Environment.NewLine +
            "<{0} field=\"Form 1:Q1\" />" + Environment.NewLine +
            "</conditions>" + Environment.NewLine +
            "<trueSet>" + Environment.NewLine +
            "</trueSet>" + Environment.NewLine +
            "</if>" + Environment.NewLine +
            "</process>" + Environment.NewLine;

        [Test]
        public void MultipleSelectionMcqIsBlankGeneratesCorrectXml()
        {
            mcqItem.SelectOnlyOne = false;

            var condition = new Condition(mcqItem, manyIsBlank);

            addIfStatementWithCondition(condition);

            Assert.AreEqual(string.Format(expectedXmlFormat, manyIsBlank.XmlTagName), process.ToXml());
        }

        [Test]
        public void MultipleSelectionMcqIsNotBlankGeneratesCorrectXml()
        {
            mcqItem.SelectOnlyOne = false;

            var condition = new Condition(mcqItem, manyIsNotBlank);

            addIfStatementWithCondition(condition);

            Assert.AreEqual(string.Format(expectedXmlFormat, manyIsNotBlank.XmlTagName), process.ToXml());
        }

        [Test]
        public void SingleSelectionMcqIsBlankGeneratesCorrectXml()
        {
            mcqItem.SelectOnlyOne = true;

            var condition = new Condition(mcqItem, oneIsBlank);

            addIfStatementWithCondition(condition);

            Assert.AreEqual(string.Format(expectedXmlFormat, oneIsBlank.XmlTagName), process.ToXml());
        }

        [Test]
        public void SingleSelectionMcqIsNotBlankGeneratesCorrectXml()
        {
            mcqItem.SelectOnlyOne = true;

            var condition = new Condition(mcqItem, oneIsNotBlank);

            addIfStatementWithCondition(condition);

            Assert.AreEqual(string.Format(expectedXmlFormat, oneIsNotBlank.XmlTagName), process.ToXml());
        }
    }
}