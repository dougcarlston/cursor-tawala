// $Workfile: Proxy.cs $
// $Revision: 3 $	$Date: 2/28/08 2:15p $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Reflection;
using System.IO;


namespace TawalaTest.TestSupport
{
	///
	/// Proxys an instance of a class not visible in a test project
	/// This will not work for a static class.
	///
	public class Proxy
	{
		public V GetField<V>(string name)
		{
			return (V)GetField(name);
		}

		public object GetField(string name)
		{
			return proxiedType.GetField(name, bindFieldOrProperty).GetValue(proxiedObject);
		}

		public void SetField(string name, object value)
		{
			proxiedType.GetField(name, bindFieldOrProperty).SetValue(proxiedObject, value);
		}

		public V GetProperty<V>(string name)
		{
			return (V)GetProperty(name);
		}

		public object GetProperty(string name)
		{
			return proxiedType.GetProperty(name, bindFieldOrProperty).GetValue(proxiedObject, null);
		}

		public void SetProperty(string name, object value)
		{
			proxiedType.GetProperty(name, bindFieldOrProperty).SetValue(proxiedObject, value, null);
		}

		public void InvokeMethod(string method, params object[] args)
		{
			invoke(method, args);
		}

		public V InvokeMethod<V>(string method, params object[] args)
		{
			return (V)invoke(method, args);
		}

		public object ProxiedObject
		{
			get { return proxiedObject; }
			set { proxiedObject = value; }
		}

		public Type ProxiedType
		{
			get { return proxiedType; }
			set { proxiedType = value; }
		}

		public static Proxy Construct(string assemblyName, string className, params object[] args)
		{
			return Construct(createInstance(assemblyName, className, args));
		}

		public static Proxy Construct(object o)
		{
			return o != null ? new Proxy(o) : null;
		}

		private Proxy(object proxied)
		{
			proxiedObject = proxied;
			proxiedType = proxied.GetType();
		}

		private static object createInstance(string assemblyName, string className, params object[] args)
		{
			return AppDomain.CurrentDomain.CreateInstanceAndUnwrap(assemblyName, className, true, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, args, null, null, null);
		}

		private object invoke(string method, params object[] args)
		{
			return proxiedType.InvokeMember(method, BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, proxiedObject, args);
		}

		private object proxiedObject;

		private Type proxiedType;

		private const BindingFlags bindFieldOrProperty = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		private const BindingFlags bindMethod = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod;
	}
}
