// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;

namespace TawalaTest.FunctionalityTest
{
    /// <summary>
    /// Class for testing IF statement functionality as relates to the Process
    /// </summary>
    [TestFixture]
    public class ProcessIfTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            // create clean project
            Project.NewTestProject();

            // create form
            form = Project.Current.AddForm();

            // create process
            process = Project.Current.AddProcess();

            // connect process to form
            Project.Current.ConnectProcessToForm(process, form.Name);

            // add new FIB item to form
            fibItem1 = new FibItem();
            fibItem1.Text = "Fib Item 1 Text __________";
            form.ItemList.Add(fibItem1);

            // add new MC item to form
            mcItem1 = new McqItem();
            form.ItemList.Add(mcItem1);
        }

        #endregion

        private IForm form;
        private Process process;
        private FibItem fibItem1;
        private McqItem mcItem1;

        // execute this once at beginning of tests
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
        }

        // execute this before each test method runs

        [Test]
        public void AdvancedIfReposition()
        {
            var condition1 = new Condition(form.GetFields()["Q1:a"], HybridOperator.List["is not blank"]);

            var expression = new Expression("a");
            var condition2 = new Condition(form.GetFields()["Q2"], MCOneOperator.List["equals"], expression);

            var conditions = new Conditions
                             {
                                 condition1,
                                 new LogicalOperator("AND"),
                                 condition2
                             };

            // create IF statement ('If Q1:a is not blank AND Q2 equals a')
            var ifStatement = new IfStatement();
            ifStatement.Conditions = conditions;

            // make process line list from IF statement and add to process
            process.Lines.Add(new ProcessLineList(ifStatement));

            Assert.AreEqual("If Form 1:Q1:a is not blank AND Form 1:Q2 equals a", process.Lines[0].ToString());

            // insert FIB item at beginning of form
            var fibItem2 = new FibItem();
            form.ItemList.Insert(0, fibItem2);

            Assert.AreEqual("If Form 1:Q2:a is not blank AND Form 1:Q3 equals a", process.Lines[0].ToString());
        }

        [Test]
        public void EditFibBlanks()
        {
            // create IF statement ('If Q1:a is not blank')
            var ifStatement = new IfStatement();
            ifStatement.Conditions = new Conditions(form.GetFields()["Q1:a"], HybridOperator.List["is not blank"]);

            // make process line list from IF statement and add to process
            process.Lines.Add(new ProcessLineList(ifStatement));

            Assert.AreEqual("If Form 1:Q1:a is not blank", process.Lines[0].ToString());

            fibItem1.Text = "Fib Item 1 Text __________ __________";
            Assert.AreEqual("If Form 1:Q1:a is not blank", process.Lines[0].ToString());

            fibItem1.Text = "Fib Item 1 Text __________";
            Assert.AreEqual("If Form 1:Q1:a is not blank", process.Lines[0].ToString());
        }

        [Test]
        public void EditFibText()
        {
            // create IF statement ('If Q1:a is not blank')
            var ifStatement = new IfStatement();
            ifStatement.Conditions = new Conditions(form.GetFields()["Q1:a"], HybridOperator.List["is not blank"]);

            // make process line list from IF statement and add to process
            process.Lines.Add(new ProcessLineList(ifStatement));

            Assert.AreEqual("If Form 1:Q1:a is not blank", process.Lines[0].ToString());

            fibItem1.Text = "Edited Fib Item 1 Text __________";
            Assert.AreEqual("If Form 1:Q1:a is not blank", process.Lines[0].ToString());
        }

        [Test]
        public void ReplaceNestedIf()
        {
            // create IF statement ('If Q1:a is blank')
            var ifStatement1 = new IfStatement();
            ifStatement1.Conditions = new Conditions(form.GetFields()["Q1:a"], HybridOperator.List["is blank"]);

            // make process line list from IF statement and add to process
            process.Lines.Add(new ProcessLineList(ifStatement1));

            // create IF statement ('If Q1:a is not blank')
            var ifStatement2 = new IfStatement();
            ifStatement2.Conditions = new Conditions(form.GetFields()["Q1:a"], HybridOperator.List["is not blank"]);

            // make process line list from IF statement and add to process
            process.Lines.Insert(2, new ProcessLineList(ifStatement2));

            Assert.AreEqual("If Form 1:Q1:a is blank", process.Lines[0].ToString());
            Assert.AreEqual("If Form 1:Q1:a is not blank", process.Lines[2].ToString());
            Assert.AreEqual(5, process.Lines.LineGroupEndIndex(process.Lines[0]));
            Assert.AreEqual(4, process.Lines.LineGroupEndIndex(process.Lines[2]));

            // create IF statement ('If Q1:a is not blank')
            var ifStatement3 = new IfStatement();
            ifStatement3.Conditions = new Conditions(form.GetFields()["Q1:a"], HybridOperator.List["is not blank"]);

            process.Lines.Replace(0, ifStatement3);

            Assert.AreEqual("If Form 1:Q1:a is not blank", process.Lines[0].ToString());
            Assert.AreEqual("If Form 1:Q1:a is not blank", process.Lines[2].ToString());
            Assert.AreEqual(5, process.Lines.LineGroupEndIndex(process.Lines[0]));
            Assert.AreEqual(4, process.Lines.LineGroupEndIndex(process.Lines[2]));
        }

        [Test]
        public void SimpleIfRemoveFIB()
        {
            // create IF statement ('If Q1:a is not blank')
            var ifStatement = new IfStatement();
            ifStatement.Conditions = new Conditions(form.GetFields()["Q1:a"], HybridOperator.List["is not blank"]);

            // make process line list from IF statement and add to process
            process.Lines.Add(new ProcessLineList(ifStatement));

            Assert.AreEqual("If Form 1:Q1:a is not blank", process.Lines[0].ToString());

            form.ItemList.Remove(fibItem1);

            Assert.AreEqual("If Unknown Field is not blank", process.Lines[0].ToString());
        }

        [Test]
        public void SimpleIfRemoveMCQ()
        {
            // create IF statement ('If Q2 equals a')
            var ifStatement = new IfStatement();
            ifStatement.Conditions = new Conditions(form.GetFields()["Q2"], MCOneOperator.List["equals"], new Choice("a"));

            // make process line list from IF statement and add to process
            process.Lines.Add(new ProcessLineList(ifStatement));

            Assert.AreEqual("If Form 1:Q2 equals a", process.Lines[0].ToString());

            form.ItemList.Remove(mcItem1);

            Assert.AreEqual("If Unknown Field equals a", process.Lines[0].ToString());
        }

        [Test]
        public void SimpleIfReposition()
        {
            // create IF statement ('If Q1:a is not blank')
            var ifStatement = new IfStatement();
            ifStatement.Conditions = new Conditions(form.GetFields()["Q1:a"], HybridOperator.List["is not blank"]);

            // make process line list from IF statement and add to process
            process.Lines.Add(new ProcessLineList(ifStatement));

            Assert.AreEqual("If Form 1:Q1:a is not blank", process.Lines[0].ToString());

            // insert FIB item at beginning of form
            var fibItem2 = new FibItem();
            form.ItemList.Insert(0, fibItem2);

            Assert.AreEqual("If Form 1:Q2:a is not blank", process.Lines[0].ToString());
        }
    }
}