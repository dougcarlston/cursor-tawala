// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest.ProcessStatements
{
    [TestFixture]
    public class ProcessStatementListTest
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

            // create FIB items
            var fibItem1 = new FibItem();
            var fibItem2 = new FibItem();

            // add new FIB items to form
            form.ItemList.Add(fibItem1);
            form.ItemList.Add(fibItem2);

            // add fields to Project's field mapper
            process.MappedFields.Fields.Add(((FibItem)form.ItemList[0]).BlankList[0]);
            process.MappedFields.Fields.Add(((FibItem)form.ItemList[1]).BlankList[0]);
            process.MappedFields.Map();
        }

        #endregion

        private IForm form;
        private Process process;

        // execute this before each test method runs

        [Test]
        public void IfWithCommentFromXml()
        {
            string xmlString =
                "<if>" +
                " <conditions>" +
                "  <equals field=\"Q1:a\">" +
                "   <string value=\"One\"/>" +
                "  </equals>" +
                " </conditions>" +
                " <trueSet>" +
                " <comment>I gotcher comment right here!</comment>" +
                " </trueSet>" +
                "</if>";

            IXmlElement element = new XmlElement(xmlString);
            var statement1 = new IfStatement(element, process.Name);

            Assert.AreEqual(1, statement1.TrueSet.Count);
            Assert.AreEqual("I gotcher comment right here!", ((CommentStatement)statement1.TrueSet[0]).Text);
        }

        [Test]
        public void NestedIfFromXml()
        {
            string xmlString =
                "<if>" +
                " <conditions>" +
                "  <equals field=\"Q1:a\">" +
                "   <string value=\"One\"/>" +
                "  </equals>" +
                " </conditions>" +
                " <trueSet>" +
                "  <if>" +
                "   <conditions>" +
                "    <equals field=\"Q1:a\">" +
                "     <string value=\"Two\"/>" +
                "    </equals>" +
                "   </conditions>" +
                "   <trueSet>" +
                "    <if>" +
                "     <conditions>" +
                "      <equals field=\"Q1:a\">" +
                "       <string field=\"Q2:a\"/>" +
                "      </equals>" +
                "     </conditions>" +
                "     <trueSet>" +
                "     </trueSet>" +
                "    </if>" +
                "   </trueSet>" +
                "  </if>" +
                " </trueSet>" +
                "</if>";

            IXmlElement element = new XmlElement(xmlString);
            var statement1 = new IfStatement(element, process.Name);
            Assert.AreEqual(1, statement1.TrueSet.Count);
            Assert.AreEqual("If Form 1:Q1:a equals \"One\"", statement1.ToString());

            var statement2 = (IfStatement)statement1.TrueSet[0];
            Assert.AreEqual(1, statement2.TrueSet.Count);
            Assert.AreEqual("If Form 1:Q1:a equals \"Two\"", statement2.ToString());

            var statement3 = (IfStatement)statement2.TrueSet[0];
            Assert.AreEqual(0, statement3.TrueSet.Count);
            Assert.AreEqual("If Form 1:Q1:a equals Form 1:Q2:a", statement3.ToString());
        }
    }
}