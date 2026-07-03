// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Processes;
using TawalaTest.TestSupport;

namespace TawalaTest.FunctionalityTest
{
    /// <summary>
    /// Class for testing behavior of Variable deleted by virtue of deleting a SET statement
    /// </summary>
    [TestFixture]
    public class RemoveSetStatementTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            process1 = Project.Current.AddProcess();

            variable1 = new Variable("Variable 1");

            setStatement1 = new SetStatement
                            {
                                Variable = new Variable("Variable 1"),
                                Expression = new Expression("value 1")
                            };

            setLine1 = new SetLine(setStatement1);
            process1.Lines.Add(setLine1);

            ifStatement = new IfStatement
                          {
                              Conditions = new Conditions(variable1, HybridOperator.List["is not blank"])
                          };

            process1.Lines.Add(new ProcessLineList(ifStatement));
        }

        #endregion

        private Process process1;
        private Variable variable1;
        private SetStatement setStatement1;
        private SetLine setLine1;
        private IfStatement ifStatement;

        [Test]
        public void InitialState()
        {
            Assert.AreEqual(4, process1.Lines.Count);
            Assert.AreEqual("Set Variable 1 to \"value 1\"", process1.Lines[0].ToString());
            Assert.AreEqual("If Variable 1 is not blank", process1.Lines[1].ToString());
            Assert.AreEqual("(", process1.Lines[2].ToString());
            Assert.AreEqual(")", process1.Lines[3].ToString());
        }

        [Test]
        public void RemovingAllSetStatementsPreservesVariable()
        {
            Process process2 = Project.Current.AddProcess();

            var setStatement2 = new SetStatement
                                {
                                    Variable = new Variable("Variable 1")
                                };
            setStatement1.Expression = new Expression("value 2");

            var setLine2 = new SetLine(setStatement2);
            process2.Lines.Add(setLine2);

            process1.Lines.Remove(setLine1);
            process2.Lines.Remove(setLine2);

            Assert.AreEqual("If Variable 1 is not blank", process1.Lines[0].ToString());
        }

        //[Test]
        //public void RemovingSoleSetStatementLeavesUnknownField()
        //{
        //    process1.Lines.Remove(setLine1);

        //    Assert.AreEqual(3, process1.Lines.Count);
        //    Assert.AreEqual("If Unknown Field is not blank", process1.Lines[0].ToString());
        //}

        [Test]
        public void RemovingSoleSetStatementPreservesVariable()
        {
            process1.Lines.Remove(setLine1);

            Assert.AreEqual(3, process1.Lines.Count);
            Assert.AreEqual("If Variable 1 is not blank", process1.Lines[0].ToString());
        }

        [Test]
        public void SetVariableInTwoProcessesThenRemoveOne()
        {
            Process process2 = Project.Current.AddProcess();

            var setStatement2 = new SetStatement
                                {
                                    Variable = new Variable("Variable 1")
                                };
            setStatement1.Expression = new Expression("value 2");

            var setLine2 = new SetLine(setStatement2);
            process2.Lines.Add(setLine2);

            process1.Lines.Remove(setLine1);

            Assert.AreEqual("If Variable 1 is not blank", process1.Lines[0].ToString());
        }

        [Test]
        public void SetVariableInTwoStatementsThenRemoveOne()
        {
            var setStatement2 = new SetStatement
                                {
                                    Variable = new Variable("Variable 1")
                                };
            setStatement1.Expression = new Expression("value 2");

            var setLine2 = new SetLine(setStatement2);
            process1.Lines.Add(setLine2);

            process1.Lines.Remove(setLine1);

            Assert.AreEqual("If Variable 1 is not blank", process1.Lines[0].ToString());
        }

        //[Test]
        //public void RemovingAllSetStatementsLeavesUnknownField()
        //{
        //    Process process2 = Project.Current.AddProcess();

        //    SetStatement setStatement2 = new SetStatement();
        //    setStatement2.Variable = new Variable("Variable 1");
        //    setStatement1.Expression = new Expression("value 2");

        //    SetLine setLine2 = new SetLine(setStatement2);
        //    process2.Lines.Add(setLine2);

        //    process1.Lines.Remove(setLine1);
        //    process2.Lines.Remove(setLine2);

        //    Assert.AreEqual("If Unknown Field is not blank", process1.Lines[0].ToString());
        //}
    }
}