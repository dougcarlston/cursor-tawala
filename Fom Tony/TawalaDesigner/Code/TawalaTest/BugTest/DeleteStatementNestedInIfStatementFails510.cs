// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.BugTest
{
    [TestFixture]
    public class DeleteStatementNestedInIfStatementFails510
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
            form = Project.Current.AddForm();
            form.ItemList.Add(new McqItem());

            process = Project.Current.AddProcess();
        }

        #endregion

        private const string NEWLINE = "\r\n";

        private IForm form;
        private Process process;

        [Test]
        public void DeleteStatementNestedInIfStatementGeneratesValidProcessLineList()
        {
            const string xmlString = "<if>" + NEWLINE +
                                     "<conditions>" + NEWLINE +
                                     "<mcEquals field=\"Form 1:Q1\" value=\"a\"/>" + NEWLINE +
                                     "</conditions>" + NEWLINE +
                                     "<trueSet>" + NEWLINE +
                                     "<delete><form name=\"Form 1\"/></delete>" + NEWLINE +
                                     "</trueSet>" + NEWLINE +
                                     "</if>";

            IXmlElement element = new XmlElement(xmlString);
            var ifStatement = new IfStatement(element, process);

            var lineList = new ProcessLineList(ifStatement);
            Assert.AreEqual(4, lineList.Count);
            Assert.AreEqual("If Form 1:Q1 equals a", lineList[0].ToString());
            Assert.AreEqual("(", lineList[1].ToString());
            Assert.AreEqual("Delete records from Form 1", lineList[2].ToString());
            Assert.AreEqual(")", lineList[3].ToString());
        }
    }
}