// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	/// <summary>
	/// Factory class to produce Tawala objects from XML elements that are children of &lt;choice&gt; elements.
	/// </summary>
	public class ChoiceBlockFactory<T> : Factory<T>
	{
		/// <summary>
		/// Creates and returns a new object of type T from information in the specified XML element,
		/// passing the choice label to subsequently-called constructors.
		/// </summary>
		public new T MakeObject(IXmlElement element, string label)
		{
			if (isNull(element))
			{
				Console.WriteLine("ChoiceBlockFactory.MakeObject: {0} isNull(element)", element.Name);
			}

			if (!isRegistered(element))
			{
				Console.WriteLine("ChoiceBlockFactory.MakeObject: {0} !isRegistered(element)", element.Name);
			}

			if (isNull(element) || !isRegistered(element))
			{
				return default(T);
			}

			Type classType = typeToConstruct(element);

			if (classType == null)
			{
				Console.WriteLine("ChoiceBlockFactory.MakeObject: {0} classType is null", element.Name);
				return default(T);
			}

			ConstructorInfo constructor = locateStringConstructor(classType);
			Debug.Assert(constructor != null);

			if (constructor == null)
			{
				Console.WriteLine("ChoiceBlockFactory.MakeObject: Cannot find constructor {0}(IXmlElement, string) for XML element <{1}>", classType.ToString(), element.Name);
			}

			return invokeStringConstructor(element, label, constructor);
		}

		/// <summary>
		/// Locates a constructor for classType that has IXmlElement and string arguments
		/// </summary>
		private static ConstructorInfo locateStringConstructor(Type classType)
		{
			// locate constructor that has IXmlElement and string arguments
			Type[] argTypes = new Type[2];
			argTypes[0] = typeof(IXmlElement);
			argTypes[1] = typeof(string);
			ConstructorInfo constructor = classType.GetConstructor(argTypes);

			return constructor;
		}

		/// <summary>
		/// Invokes the specified constructor and returns new object
		/// </summary>
		private static T invokeStringConstructor(IXmlElement element, string label, ConstructorInfo constructor)
		{
			Object[] args = new Object[2];
			args[0] = element;
			args[1] = label;
			return (T)constructor.Invoke(args);
		}
	}
}
