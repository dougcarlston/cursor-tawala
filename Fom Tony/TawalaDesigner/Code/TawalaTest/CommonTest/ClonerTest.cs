// $Workfile: ClonerTest.cs $
// $Revision: 1 $	$Date: 8/22/05 7:45p $
// Copyright © 2005 Douglas G. Carlston. All rights reserved.

using System;
using NUnit.Framework;
using Tawala.Common;

namespace TawalaTest.CommonTest
{
	[TestFixture]
	public class ClonerTest
	{
		// execute this once at beginning of tests
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
		}

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
		}

		[Test]
		public void CloneTest()
		{
			string serializable = "A string object is serializable so it should be cloneable.";
			string s = Cloner.Clone<string>(serializable);
			Assert.AreEqual(s, serializable);
		}

		// execute this after each test method runs
		[TearDown]
		public void TearDown()
		{
		}

		// execute this once at end of all tests
		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
		}
	}
}
