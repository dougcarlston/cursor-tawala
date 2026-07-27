// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Diagnostics;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using TawalaTest.TestSupport;
using Process=Tawala.Projects.Processes.Process;

namespace TawalaTest.AcceptanceTest.DesignerAppPerformance
{
    [TestFixture]
    public class ProcessModificationTest2692
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            setupForm();
            setupProcess();
            form.ConnectedPostProcess = process;
        }

        #endregion

        private IForm form;
        private FibItem fibItem;
        private Process process;
        private GetStatement getStatement;
        private FormList forms;
        private RecordSet recordSet;

        private void setupForm()
        {
            form = Project.Current.AddForm();

            fibItem = new FibItem();
            fibItem.Text = "Fib Item __________";
            form.ItemList.Add(fibItem);
        }

        private Variable variable;

        private readonly Stopwatch stopWatch = new Stopwatch();

        private void setupProcess()
        {
            process = Project.Current.AddProcess();

            getStatement = makeGetStatement();
            addGetStatement(getStatement);
            addForEachStatement(makeForEachStatement());

            for (int i = 0, j = 1; i < 500; i++, j++)
            {
                stopWatch.Start();

                var setStatement = new SetStatement(process);

                if ((j%1) == 0)
                {
                    variable = new Variable("Variable " + (i + 1), true);
                }

                setStatement.Variable = variable;
                setStatement.Expression = new Expression(fibItem.BlankList[0]);

                var setLine = new ProcessLineList(setStatement);

                process.Lines.Insert(i + 3, setLine);

                stopWatch.Stop();

                if ((j%100) == 0)
                {
                    Console.WriteLine("ProcessModificationTest2692.setupProcess: elapsed loop time ({0}) = {1} ms", j,
                                      stopWatch.ElapsedMilliseconds);
                }
            }
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

        private void addGetStatement(GetStatement getStatement)
        {
            process.Lines.Add(getGetLine(getStatement));
        }

        private void addForEachStatement(ForEachStatement forEachStatement)
        {
            process.Lines.Add(getForEachLine(forEachStatement));
        }

        [Test]
        public void CanModifyProcessInUnderOneQuarterSecond()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Reset();

            stopWatch.Start();
            process.Lines.RemoveAt(3);
            process.Lines.ValidateLines();
            stopWatch.Stop();

            Console.WriteLine("ProcessModificationTest2692.CanValidateProcessInUnderOneQuarterSecond: elapsedMilliseconds = {0}",
                              stopWatch.ElapsedMilliseconds);

            int expectedMilliseconds = 250;
            Assert.Less((int)stopWatch.ElapsedMilliseconds, expectedMilliseconds);
            Assert.AreEqual("Set Variable 2 to Form 1:Q1:a", process.Lines[3].ToString());
        }

        [Test]
        public void CheckProcess()
        {
            Assert.AreEqual(504, process.Lines.Count);
            Assert.AreEqual("Get Records from Form 1", process.Lines[0].ToString());
            Assert.AreEqual("For Each Record in Records", process.Lines[1].ToString());
            Assert.AreEqual("(", process.Lines[2].ToString());
            Assert.AreEqual("Set Variable 1 to Form 1:Q1:a", process.Lines[3].ToString());
            Assert.AreEqual("Set Variable 500 to Form 1:Q1:a", process.Lines[502].ToString());
            Assert.AreEqual(")", process.Lines[503].ToString());
        }
    }
}