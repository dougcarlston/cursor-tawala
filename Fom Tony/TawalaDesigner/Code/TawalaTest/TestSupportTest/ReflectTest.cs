// $Workfile: ReflectTest.cs $
// $Revision: 4 $	$Date: 2/28/08 10:42a $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Reflection;
using NUnit.Framework;
using TawalaTest.TestSupport;

namespace TawalaTest.TestSupportTest
{
	[TestFixture]
	public class ReflectTest
	{
		private TestClass testClass;

		[SetUp]
		public void SetUp()
		{
			testClass = new TestClass();
		}

		[TearDown]
		public void TearDown()
		{
			testClass = null;
		}

		[Test]
		public void GetInstanceField()
		{
			Assert.AreEqual(3141, Reflect<TestClass>.GetField<int>("instanceField", testClass));
		}

		[Test]
		public void SetInstanceField()
		{
			Reflect<TestClass>.SetField<int>("instanceField", testClass, 186282);
			Assert.AreEqual(186282, Reflect<TestClass>.GetField<int>("instanceField", testClass));
		}

		[Test]
		public void GetStaticField()
		{
			Assert.AreEqual("foo", Reflect<TestClass>.GetStaticField<string>("staticField"));
		}

		[Test]
		public void SetStaticField()
		{
			Reflect<TestClass>.SetStaticField<string>("staticField", "Different String");
			Assert.AreEqual("Different String", Reflect<TestClass>.GetStaticField<string>("staticField"));
		}

		[Test]
		public void GetInstanceProperty()
		{
			Assert.AreEqual(2718, Reflect<TestClass>.GetProperty<int>("InstanceProperty", testClass));
		}

		[Test]
		public void SetInstanceProperty()
		{
			Reflect<TestClass>.SetProperty<int>("InstanceProperty", testClass, 299792458);
			Assert.AreEqual(299792458, Reflect<TestClass>.GetProperty<int>("InstanceProperty", testClass));
		}

		[Test]
		public void GetStaticProperty()
		{
			Assert.AreEqual("bar", Reflect<TestClass>.GetStaticProperty<string>("StaticProperty"));
		}

		[Test]
		public void SetStaticProperty()
		{
			Reflect<TestClass>.SetStaticProperty<string>("StaticProperty", "The speed of light");
			Assert.AreEqual("The speed of light", Reflect<TestClass>.GetStaticProperty<string>("StaticProperty"));
		}

		[Test]
		public void InvokeInstanceMethodNoArgs()
		{
			Reflect<TestClass>.InvokeMethod("noReturnNoArgs", testClass);
		}

		[Test]
		public void InvokeStaticMethodWithReturnAndArgs()
		{
			string result = Reflect<TestClass>.InvokeStaticMethod<string>("staticWith2Args", "string1", "string2");
			Assert.AreEqual("string1+string2", result);
		}
	}

#pragma warning disable 414

	internal class TestClass
	{
		private int instanceField = 3141;
		private static string staticField = "foo";

		private int instanceProperty = 2718;

		public int InstanceProperty
		{
			get { return instanceProperty; }
			set { instanceProperty = value; }
		}

		private static string staticProperty = "bar";

		public static string StaticProperty
		{
			get { return TestClass.staticProperty; }
			set { TestClass.staticProperty = value; }
		}

		private void noReturnNoArgs()
		{
			Console.WriteLine("noReturnNoArgs()");
		}

		private static string staticWith2Args(string arg1, string arg2)
		{
			return arg1 + "+" + arg2;
		}
	}
}
