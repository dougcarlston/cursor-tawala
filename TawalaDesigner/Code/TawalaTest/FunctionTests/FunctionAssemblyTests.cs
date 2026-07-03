// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;

namespace TawalaTest.FunctionTests
{
    [TestFixture]
    public class FunctionAssemblyTests : FunctionTestBase
    {
        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            FixtureSetup();
        }

        [Test]
        public void ContainsImplementationOfGeneratedFunctionAssembly()
        {
            Assert.IsNotNull(functionRepository);
        }

        [Test]
        public void FunctionAssemblyHasRepositoryRootXml()
        {
            Assert.AreEqual("component-repository", functionRepository.Xml.Name);
        }

        [Test]
        public void HasValidSignature()
        {
            Assert.AreEqual("295897740", functionRepository.Signature);
        }

        [Test]
        public void XmlFunctionCountEqualsFunctionCollectionCount()
        {
            int count = GetAllComponentTypesFromXmlSortedByName().Count-1;
            Assert.IsTrue(count > 0);
            Assert.AreEqual(functionRepository.Functions.Count, count);
        }
    }
}