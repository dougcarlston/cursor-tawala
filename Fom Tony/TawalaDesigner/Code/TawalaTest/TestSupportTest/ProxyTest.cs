// $Workfile: ProxyTest.cs $
// $Revision: 4 $	$Date: 2/29/08 10:02a $
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
	public class ProxyTest
	{
		[Test]
		public void ConstructProxy()
		{
			Proxy proxy = Proxy.Construct("TestSupport", "TawalaTest.TestSupport.InternalClassForTesting");

			Assert.IsNotNull(proxy);
			Assert.IsNotNull(proxy.ProxiedObject);
			Assert.IsNotNull(proxy.ProxiedType);
			Assert.AreSame(proxy.ProxiedType, proxy.ProxiedObject.GetType());
		}

		[Test]
		public void ConstructProxyOfNestedClass()
		{
			Proxy proxy = Proxy.Construct("TestSupport", "TawalaTest.TestSupport.InternalClassForTesting+PrivateNestedClass");

			Assert.IsNotNull(proxy);
		}

		[Test]
		public void GetInstanceField()
		{
			Proxy proxy = createProxiedInstance();
			object value = proxy.GetField("pi");
			Assert.AreEqual(3.1415926, (double)value);
		}

		[Test]
		public void SetInstanceField()
		{
			string msg = "To force heaven, Mars shall have a new angel";
			Proxy proxy = createProxiedInstance();
			proxy.SetField("message", msg);
			object value = proxy.GetField("message");
			Assert.AreEqual(msg, value as string);
		}

		[Test]
		public void GetInstanceProperty()
		{
			Proxy proxy = createProxiedInstance();
			object value = proxy.GetProperty("PI");
			Assert.AreEqual(3.1415926, (double)value);
		}

		[Test]
		public void SetInstanceProperty()
		{
			string msg = "To force heaven, Mars shall have a new angel";
			Proxy proxy = createProxiedInstance();
			proxy.SetProperty("Message", msg);
			object value = proxy.GetProperty("Message");
			Assert.AreEqual(msg, value as string);
		}

		[Test]
		public void GetStaticField()
		{
			Proxy proxy = createProxiedInstance();
			object value = proxy.GetField("staticRef");

			Assert.IsNotNull(value);
			Assert.AreNotSame(proxy.ProxiedObject, value);
			Assert.AreSame(proxy.ProxiedType, value.GetType());
		}

		[Test]
		public void InvokeMethodWithArgsAndReturn()
		{
			Proxy proxy = createProxiedInstance();
			string value = proxy.InvokeMethod<string>("combine", "string", 5000);
			Assert.AreEqual("string5000", value);
		}

		private Proxy createProxiedInstance()
		{
			return Proxy.Construct("TestSupport", "TawalaTest.TestSupport.InternalClassForTesting+PrivateNestedClass");
		}
	}
}
