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
    public class MatchMultipleSelectionMcqChoiceWithField2792
    {
        [Test]
        public void MultipleSelectionMcqChoiceComparedToFieldGeneratesCorrectXml()
        {
            Util.NewTestProject();
            IForm form = Project.Current.AddForm();

            var fibItem = new FibItem();
            form.ItemList.Add(fibItem);

            var mcItem = new McqItem();
            var choices = new ChoiceList();
            choices.Add(new Choice("Choice 1"));
            choices.Add(new Choice("Choice 2"));
            choices.Add(new Choice("Choice 3"));
            mcItem.SelectOnlyOne = false;
            form.ItemList.Add(mcItem);

            Process process = Project.Current.AddProcess();

            var condition = new Condition(mcItem, MCManyOperator.List[MCManyOperator.Ops.mcContains], new Expression(fibItem.BlankList[0]));

            var ifStatement = new IfStatement(new Conditions(condition));
            var ifList = new ProcessLineList(ifStatement);
            process.Lines.Add(ifList);

            string expectedXml =
                "<process name=\"Process 1\">" + Environment.NewLine +
                "<if>" + Environment.NewLine +
                "<conditions>" + Environment.NewLine +
                "<mcContains field=\"Form 1:Q2\">" + Environment.NewLine +
                "<string field=\"Form 1:Q1:a\"/>" + Environment.NewLine +
                "</mcContains>" + Environment.NewLine +
                "</conditions>" + Environment.NewLine +
                "<trueSet>" + Environment.NewLine +
                "</trueSet>" + Environment.NewLine +
                "</if>" + Environment.NewLine +
                "</process>" + Environment.NewLine;

            Assert.AreEqual(expectedXml, process.ToXml());
        }
    }
}