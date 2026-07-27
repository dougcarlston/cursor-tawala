// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Reflection;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Processes;
using TawalaTest.TestSupport;

namespace TawalaTest.BugTest
{
    [TestFixture]
    public class ShowRecordStatementInsideIf530
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            Util.NewTestProject();

            form = Project.Current.AddForm();
            process = Project.Current.AddProcess();
            var = new Variable("var");
            one = new Expression("1");
        }

        #endregion

        private IForm form;
        private Process process;
        private Variable var;
        private Expression one;

        [Test]
        public void SaveAndLoad()
        {
            var set = new SetStatement();
            set.Variable = var;
            set.Expression = one;

            process.Lines.Add(new ProcessLineList(set));

            var ifStatement = new IfStatement();
            var condition = new Condition(var, HybridOperator.List[HybridOperator.Ops.equals], one);
            ifStatement.Conditions.Add(condition);

            var showRecord = new ShowRecordStatement();
            showRecord.Form = form;
            showRecord.ModifyOnSubmit = true;

            ifStatement.TrueSet.Add(showRecord);
            process.Lines.Add(new ProcessLineList(ifStatement));

            string tmpFile = Util.SaveCurrentProject();
            string xmlSave = Project.Current.ToXmlForSaving();

            Project.ProjectFileOpenResult opened = Project.ProjectFileOpenResult.InvalidFile;

            try
            {
                opened = Util.LoadProject(tmpFile);
            }
            catch (AmbiguousMatchException e)
            {
                Assert.Fail(e.ToString());
            }
        }
    }
}