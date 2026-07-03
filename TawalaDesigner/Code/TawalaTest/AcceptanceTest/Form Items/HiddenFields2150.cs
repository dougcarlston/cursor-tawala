// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.ProjectUI;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FormItems
{
    [TestFixture]
    public class HiddenFields2150
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            testPalette = new FieldsPalette();

            form = Project.Current.AddForm();

            fibItem = new FibItem();
            hiddenField = new HiddenField();
            mcItem = new McqItem();

            form.ItemList.Add(fibItem);
            form.ItemList.Add(hiddenField);
            form.ItemList.Add(mcItem);
        }

        [TearDown]
        public void Teardown()
        {
            if (testPalette != null)
            {
                testPalette.Dispose();
                testPalette = null;
            }
            clear();
        }

        #endregion

        private string testName = "My Hidden Field";

        private IForm form;
        private FibItem fibItem;
        private McqItem mcItem;
        private HiddenField hiddenField;
        private FieldsPalette testPalette;

        private void clear()
        {
            form = null;
            fibItem = null;
            hiddenField = null;
            testPalette = null;
        }

        [Test]
        public void DefaultName()
        {
            var hiddenField2 = new HiddenField();
            form.ItemList.Add(hiddenField2);
            Assert.AreEqual("Field2", hiddenField2.FieldName);

            var fibItem2 = new FibItem();
            fibItem2.AlternateLabel = "Field3";
            form.ItemList.Add(fibItem2);

            var hiddenField3 = new HiddenField();
            form.ItemList.Add(hiddenField3);
            Assert.AreEqual("Field4", hiddenField3.FieldName);
        }

        [Test]
        public void RoundTripHiddenFieldInProjectXml()
        {
            hiddenField.Name = testName;

            Util.SaveAndReloadCurrentProject();

            IForm reloadedForm = Project.Current.FormList[0];
            Assert.AreNotSame(form, reloadedForm);
            Assert.AreEqual(3, reloadedForm.ItemList.Count);

            var reloadedHiddenField = reloadedForm.ItemList[1] as HiddenField;
            Assert.IsNotNull(reloadedHiddenField);
            Assert.AreEqual(testName, reloadedHiddenField.Name);
            Assert.AreEqual(testName, reloadedHiddenField.FieldName);
        }

        [Test]
        public void RoundTripHiddenFieldInSetStatementProjectXml()
        {
            {
                hiddenField.Name = testName;

                Process process = Project.Current.AddProcess();
                form.ConnectedPostProcess = process;

                var setStatement = new SetStatement(process);
                setStatement.Variable = hiddenField;
                setStatement.Expression = new Expression(fibItem.BlankList[0]);

                process.Lines.Add(new ProcessLineList(setStatement));

                Util.SaveAndReloadCurrentProject();

                clear();
            }

            IForm reloadedForm = Project.Current.FormList[0];
            Process reloadedProcess = Project.Current.ProcessList[0];
            Assert.AreEqual(3, reloadedForm.ItemList.Count);

            Assert.AreEqual(1, reloadedProcess.Lines.Count);

            var reloadedStatement = reloadedProcess.Lines[0].Statement as SetStatement;
            Assert.IsNotNull(reloadedStatement);

            var reloadedHiddenField = reloadedForm.ItemList[1] as HiddenField;
            Assert.IsNotNull(reloadedHiddenField);
            Assert.AreEqual(testName, reloadedHiddenField.Name);
            Assert.AreEqual(testName, reloadedHiddenField.FieldName);

            Assert.AreEqual("<<Form 1:Q1:a>>", reloadedStatement.Expression.ToString());

            Assert.AreEqual(reloadedHiddenField.QualifiedFieldName, reloadedStatement.Variable.QualifiedFieldName);
            Assert.AreSame(reloadedHiddenField, reloadedStatement.Variable);
        }
    }
}