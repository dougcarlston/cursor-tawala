// $Workfile: Reflect.cs $
// $Revision: 11 $	$Date: 2/28/08 3:13p $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Reflection;
using System.IO;

namespace TawalaTest.TestingSupport
{
	///
	/// The original version of Reflect is below this templated version
	///
	/// T is the Type (class, struct) being reflected on
	/// V is the Type of a field or property
	/// 
	/// instance parameter should be null if operating on a static Field, Property or Method
	///
	public static class Reflect<T>
	{
		public static V GetField<V>(string name, T instance)
		{
			return (V)(GetFieldInfo(name).GetValue(instance));
		}

		public static V GetStaticField<V>(string name)
		{
			return (V)(GetFieldInfo(name).GetValue(null));
		}

		public static void SetField<V>(string name, T instance, V value)
		{
			GetFieldInfo(name).SetValue(instance, value);
		}

		public static void SetStaticField<V>(string name, V value)
		{
			GetFieldInfo(name).SetValue(null, value);
		}

		public static FieldInfo GetFieldInfo(string name)
		{
			return typeof(T).GetField(name, bindFieldOrProperty);
		}

		public static V GetProperty<V>(string name, T instance)
		{
			return (V)(GetPropertyInfo(name).GetValue(instance, null));
		}

		public static V GetStaticProperty<V>(string name)
		{
			return (V)(GetPropertyInfo(name).GetValue(null, null));
		}

		public static void SetProperty<V>(string name, T instance, V value)
		{
			GetPropertyInfo(name).SetValue(instance, value, null);
		}

		public static void SetStaticProperty<V>(string name, V value)
		{
			GetPropertyInfo(name).SetValue(null, value, null);
		}

		public static PropertyInfo GetPropertyInfo(string name)
		{
			return typeof(T).GetProperty(name, bindFieldOrProperty);
		}

		public static void InvokeMethod(string name, T instance, params object[] args)
		{
			typeof(T).InvokeMember(name, bindMethod, null, instance, args);
		}

		public static void InvokeStaticMethod(string name, params object[] args)
		{
			typeof(T).InvokeMember(name, bindMethod, null, null, args);
		}

		public static V InvokeMethod<V>(string name, T instance, params object[] args)
		{
			return (V)(typeof(T).InvokeMember(name, bindMethod, null, instance, args));
		}

		public static V InvokeStaticMethod<V>(string name, params object[] args)
		{
			return (V)(typeof(T).InvokeMember(name, bindMethod, null, null, args));
		}

		private const BindingFlags bindFieldOrProperty = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		private const BindingFlags bindMethod = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod;
	}
}