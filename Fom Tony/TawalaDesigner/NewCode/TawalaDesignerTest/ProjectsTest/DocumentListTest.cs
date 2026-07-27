// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;

namespace TawalaTest.ProjectsTest
{
    /// <summary>
    /// Summary description for DocumentListTest.
    /// </summary>
    [TestFixture]
    public class DocumentListTest
    {
        [Test]
        public void AddDocument()
        {
            IDocument doc = ComponentMaker.MakeDocumentObject("Test");

            var list = new DocumentList();
            list.Add(doc);

            //Assertions 
            Assert.IsNotNull(doc);
            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void GetXml()
        {
            IDocument doc1 = ComponentMaker.MakeDocumentObject("Document 1");
            IDocument doc2 = ComponentMaker.MakeDocumentObject("Document 2");

            var docList = new DocumentList();
            docList.Add(doc1);
            docList.Add(doc2);

            string expString =
                "<documents>\r\n" +
                "<document name=\"Document 1\">\r\n" +
                "<xmlData>" +
                "</xmlData>\r\n" +
                "</document>\r\n" +
                "<document name=\"Document 2\">\r\n" +
                "<xmlData>" +
                "</xmlData>\r\n" +
                "</document>\r\n" +
                "</documents>\r\n";

            //Assertions 
            Assert.AreEqual(expString, docList.ToXml());
        }

        [Test]
        public void GetXmlWhenEmpty()
        {
            // make list with no documents
            var docList = new DocumentList();

            //Assertions 
            Assert.AreEqual("", docList.ToXml());
        }

        [Test]
        public void RemoveDocument()
        {
            IDocument test;

            IDocument doc1 = ComponentMaker.MakeDocumentObject("Document 1");
            IDocument doc2 = ComponentMaker.MakeDocumentObject("Document 2");

            // add documents to list
            var list = new DocumentList();
            list.Add(doc2);
            list.Add(doc1);

            // assert that document 1 is in list
            test = list["Document 1"];
            Assert.IsNotNull(test);
            Assert.AreEqual("Document 1", test.Name);

            // assert that document 2 is in list
            test = list[doc2.Name];
            Assert.IsNotNull(test);
            Assert.AreEqual("Document 2", test.Name);

            // remove document 1
            list.Remove(doc1);

            // assert that document 1 is not in list
            test = list["Document 1"];
            Assert.IsNull(test);
        }
    }
}