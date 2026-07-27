// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.YouthSportsServices
{
    [TestFixture]
    public class ShowUrlStatementTest2798
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
            form = Project.Current.AddForm();
            process = Project.Current.AddProcess();

            fibItem = new FibItem();
            fibItem.BlankList.Clear();
            blank = new Blank(fibItem, 1);
            fibItem.BlankList.Add(blank);
            form.ItemList.Add(fibItem);
        }

        [TearDown]
        public void TearDown()
        {
        }

        #endregion

        private Process process;
        private IForm form;
        private FibItem fibItem;
        private Blank blank;

        private const string NEWLINE = "\r\n";

        [Test]
        public void CanConstructShowUrlStatementFromXml()
        {
            string xmlString =
                "<show>" +
                "<url>" +
                "<string value=\"http://www.google.com\"/>" +
                "</url>" +
                "</show>";

            var showUrlStatement = new ShowUrlStatement(new XmlElement(xmlString));

            Assert.AreEqual(xmlString, showUrlStatement.ToXml());
        }

        [Test]
        public void ShowUrlLineHasCorrectAttributes()
        {
            var showUrlStatement = new ShowUrlStatement("http://www.google.com");
            ProcessLine line = new ShowUrlLine(showUrlStatement);

            Assert.AreEqual(false, line.SelectsGroup);
            Assert.AreEqual(true, line.IsSelectable);
            Assert.AreEqual(true, line.IsDeletable);
            Assert.AreEqual(true, line.CanInsertBefore);
            Assert.AreEqual(0, line.IndentLevel);
        }

        [Test]
        public void ShowUrlLineProducesCorrectDisplayString()
        {
            var showUrlStatement = new ShowUrlStatement("http://www.google.com");
            ProcessLine line = new ShowUrlLine(showUrlStatement);

            Assert.AreEqual("Show URL http://www.google.com", line.ToString());
        }

        [Test]
        public void ShowUrlStatementProducesCorrectXml()
        {
            string xmlString =
                "<show>" +
                "<url>" +
                "<string value=\"http://www.google.com\"/>" +
                "</url>" +
                "</show>";

            var showUrlStatement = new ShowUrlStatement("http://www.google.com");

            Assert.AreEqual(xmlString, showUrlStatement.ToXml());
        }
    }
}