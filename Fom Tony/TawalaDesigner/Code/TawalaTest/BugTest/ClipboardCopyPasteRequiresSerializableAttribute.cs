// $Workfile: ClipboardCopyPasteRequiresSerializableAttribute.cs $
// $Revision: 3 $	$Date: 1/22/07 9:51a $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using Tawala.Projects.Components;
using Tawala.Projects.Documents;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using TawalaTest.TestSupport;
using NUnit.Framework;

namespace TawalaTest.BugTest
{
    //
    // These tests overlap at times with other Components so they are centralized here
    //
    [TestFixture]
    public class ClipboardCopyPasteRequiresSerializableAttribute
    {
        [Test]
        public void Components()
        {
            verifyMarkedSerializable<Component>();
        }

        [Test]
        public void FormItems()
        {
            verifyMarkedSerializable<IFormItem>();
        }

        [Test]
        public void DocumentBlocks()
        {
            verifyMarkedSerializable<IDocumentBlock>();
        }

        [Test]
        public void ParagraphComponents()
        {
            verifyMarkedSerializable<IParagraphComponent>();
        }

        [Test]
        public void ProcessStatements()
        {
            verifyMarkedSerializable<ProcessStatement>();
            verifyMarkedSerializable<ProcessLine>();
            verifyMarkedSerializable<ComparisonOperator>();
            verifyMarkedSerializable<FieldOrLiteral>();
        }

        private void verifyMarkedSerializable<T>()
        {
            TypeCollection notSerializable = TypeCollection.Create(typeof(T).Assembly, new NotSerializableFilter<T>());
            Assert.AreEqual(0, notSerializable.Count, notSerializable.ToString("\r\n    Type and/or derived types not marked Serializable:\r\n{0}"));
        }
    }

}
