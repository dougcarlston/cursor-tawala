// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using Tawala.XmlSupport;
using Process=Tawala.Projects.Processes.Process;

namespace Tawala.Projects.Factories
{
    /// <summary>
    /// Produces Tawala objects from XML elements.
    /// </summary>
    public class Factory<T>
    {
        private readonly ElementSignatureList elementSignatures = new ElementSignatureList();

        public int RegisteredEntries
        {
            get { return elementSignatures.Count; }
        }

        /// <summary>
        /// Associate XML element name with class type
        /// </summary>
        public void Register(string elementName, Type classType)
        {
            register(elementName, classType, new string[0]);
        }

        /// <summary>
        /// Associate XML element and attribute names with class type
        /// </summary>
        public void Register(string elementName, string attributeName, Type classType)
        {
            register(elementName, classType, new string[1] {attributeName});
        }

        /// <summary>
        /// Associate XML element and variable number of attribute names with class type
        /// </summary>
        public void Register(string elementName, Type classType, params string[] attributeNames)
        {
            register(elementName, classType, attributeNames);
        }

        /// <summary>
        /// Associate XML element with factory method
        /// </summary>
        public void RegisterFactoryMethod(Type classType, string factoryMethodName, string elementName)
        {
            elementSignatures.Set(new ElementSignature(classType, factoryMethodName, elementName, new string[0]));
        }

        /// <summary>
        /// Associate XML element and attribute name with factory method
        /// </summary>
        public void RegisterFactoryMethod(Type classType, string factoryMethodName, string elementName, string attributeName)
        {
            elementSignatures.Set(new ElementSignature(classType, factoryMethodName, elementName, new string[1] {attributeName}));
        }

        private void register(string elementName, Type classType, string[] attributeNames)
        {
            elementSignatures.Set(new ElementSignature(elementName, classType, attributeNames));
        }

        protected Boolean isNull(IXmlElement element)
        {
            return (element == null || element == XmlElement.NULL);
        }

        protected Boolean isRegistered(IXmlElement element)
        {
            return (getSignature(element) != null);
        }

        /// <summary>
        /// Creates and returns a new object of type T from information in the specified XML element.
        /// </summary>
        public T MakeObject(IXmlElement element)
        {
            if (isNull(element) || !isRegistered(element))
            {
                return default(T);
            }

            Type classType = typeToConstruct(element);

            if (classType == null)
            {
                return default(T);
            }

            string factoryMethodName = getFactoryMethodName(element);
            MethodInfo factoryMethod = locateFactoryMethod(classType, factoryMethodName);

            if (factoryMethod == null)
            {
                //Console.WriteLine("Factory.MakeObject: Cannot find factory method {0}.{1}(IXmlElement) for XML element <{2}>", classType.ToString(), factoryMethodName, element.Name);

                ConstructorInfo constructor = locateConstructor(classType);

                if (constructor == null)
                {
                    Console.WriteLine("Factory.MakeObject: Cannot find constructor {0}(IXmlElement) for XML element <{1}>", classType,
                                      element.Name);
                }

                return invokeConstructor(element, constructor);
            }
            else
            {
                return (invokeFactoryMethod(element, factoryMethod));
            }
        }

        /// <summary>
        /// Locates a constructor for classType that has IXmlElement argument
        /// </summary>
        private static ConstructorInfo locateConstructor(Type classType)
        {
            var argTypes = new Type[1];
            argTypes[0] = typeof(IXmlElement);
            ConstructorInfo constructor = classType.GetConstructor(argTypes);
            return constructor;
        }

        /// <summary>
        /// Invokes the specified constructor and returns new object
        /// </summary>
        private static T invokeConstructor(IXmlElement element, ConstructorInfo constructor)
        {
            // invoke constructor and return new object
            var args = new Object[1];
            args[0] = element;
            return (T)constructor.Invoke(args);
        }

        /// <summary>
        /// Locates a factory method for classType that has IXmlElement argument
        /// </summary>
        private static MethodInfo locateFactoryMethod(Type classType, string factoryMethodName)
        {
            if (factoryMethodName == null)
            {
                return null;
            }

            var argTypes = new Type[1];
            argTypes[0] = typeof(IXmlElement);
            MethodInfo factoryMethod = classType.GetMethod(factoryMethodName, argTypes);
            return factoryMethod;
        }

        /// <summary>
        /// Invokes the specified static factory method and returns new object
        /// </summary>
        private static T invokeFactoryMethod(IXmlElement element, MethodInfo factoryMethod)
        {
            // invoke factory method and return new object
            var args = new Object[1];
            args[0] = element;
            return (T)factoryMethod.Invoke(null, args);
        }

        /// <summary>
        /// Creates and returns a new object of type T from information in the specified XML element,
        /// potentially using the field resolver to resolve text (such as "Q1:a") to field references.
        /// </summary>
        public T MakeObject(IXmlElement element, IField fieldResolver)
        {
            if (isNull(element) || !isRegistered(element))
            {
                return default(T);
            }

            Type classType = typeToConstruct(element);

            if (classType == null)
            {
                return default(T);
            }

            ConstructorInfo constructor = locateIFieldConstructor(classType);
            Debug.Assert(constructor != null);

            if (constructor == null)
            {
                Console.WriteLine("Factory.MakeObject: Cannot find constructor {0}(IXmlElement, fieldResolver) for XML element <{1}>",
                                  classType, element.Name);
            }

            return invokeIFieldConstructor(element, fieldResolver, constructor);
        }

        /// <summary>
        /// Locates a constructor for classType that has IXmlElement and IField arguments
        /// </summary>
        private static ConstructorInfo locateIFieldConstructor(Type classType)
        {
            var argTypes = new Type[2];
            argTypes[0] = typeof(IXmlElement);
            argTypes[1] = typeof(IField);
            ConstructorInfo constructor = classType.GetConstructor(argTypes);

            return constructor;
        }

        /// <summary>
        /// Invokes the specified constructor and returns new object
        /// </summary>
        private static T invokeIFieldConstructor(IXmlElement element, IField fieldResolver, ConstructorInfo constructor)
        {
            var args = new Object[2];
            args[0] = element;
            args[1] = fieldResolver;
            return (T)constructor.Invoke(args);
        }

        /// <summary>
        /// Creates and returns a new object of type T from information in the specified XML element,
        /// potentially using the process name to inform subsequently-called constructors of the context
        /// in which process statements are being created.
        /// </summary>
        public T MakeObject(IXmlElement element, string processName)
        {
            if (isNull(element))
            {
                Console.WriteLine("Factory.MakeObject: {0} isNull(element)", element.Name);
            }

            if (!isRegistered(element))
            {
                Console.WriteLine("Factory.MakeObject: {0} !isRegistered(element)", element.Name);
            }

            if (isNull(element) || !isRegistered(element))
            {
                return default(T);
            }

            Type classType = typeToConstruct(element);

            if (classType == null)
            {
                Console.WriteLine("Factory.MakeObject: {0} classType is null", element.Name);
                return default(T);
            }

            ConstructorInfo constructor = locateStringConstructor(classType);
            Debug.Assert(constructor != null);

            if (constructor == null)
            {
                Console.WriteLine("Factory.MakeObject: Cannot find constructor {0}(IXmlElement, string) for XML element <{1}>", classType,
                                  element.Name);
            }
            return invokeStringConstructor(element, processName, constructor);
        }

        /// <summary>
        /// Locates a constructor for classType that has IXmlElement and string arguments
        /// </summary>
        private static ConstructorInfo locateStringConstructor(Type classType)
        {
            var argTypes = new Type[2];
            argTypes[0] = typeof(IXmlElement);
            argTypes[1] = typeof(string);
            ConstructorInfo constructor = classType.GetConstructor(argTypes);

            return constructor;
        }

        /// <summary>
        /// Invokes the specified constructor and returns new object
        /// </summary>
        private static T invokeStringConstructor(IXmlElement element, string processName, ConstructorInfo constructor)
        {
            var args = new Object[2];
            args[0] = element;
            args[1] = processName;
            return (T)constructor.Invoke(args);
        }

        /// <summary>
        /// Creates and returns a new object of type T from information in the specified XML element,
        /// potentially using the process to inform subsequently-called constructors of the context
        /// in which process statements are being created.
        /// </summary>
        public T MakeObject(IXmlElement element, Process process)
        {
            if (isNull(element))
            {
                Console.WriteLine("Factory.MakeObject: {0} isNull(element)", element.Name);
            }

            if (!isRegistered(element))
            {
                Console.WriteLine("Factory.MakeObject: {0} !isRegistered(element)", element.Name);
            }

            if (isNull(element) || !isRegistered(element))
            {
                return default(T);
            }

            Type classType = typeToConstruct(element);

            if (classType == null)
            {
                Console.WriteLine("Factory.MakeObject: {0} classType is null", element.Name);
                return default(T);
            }

            ConstructorInfo constructor = locateProcessConstructor(classType);
            Debug.Assert(constructor != null);

            if (constructor == null)
            {
                Console.WriteLine("Factory.MakeObject: Cannot find constructor {0}(IXmlElement, Process) for XML element <{1}>", classType,
                                  element.Name);
            }

            return invokeProcessConstructor(element, process, constructor);
        }

        /// <summary>
        /// Locates a constructor for classType that has IXmlElement and Process arguments
        /// </summary>
        private static ConstructorInfo locateProcessConstructor(Type classType)
        {
            var argTypes = new Type[2];
            argTypes[0] = typeof(IXmlElement);
            argTypes[1] = typeof(Process);
            ConstructorInfo constructor = classType.GetConstructor(argTypes);

            return constructor;
        }

        /// <summary>
        /// Invokes the specified constructor and returns new object
        /// </summary>
        private static T invokeProcessConstructor(IXmlElement element, Process process, ConstructorInfo constructor)
        {
            var args = new Object[2];
            args[0] = element;
            args[1] = process;
            return (T)constructor.Invoke(args);
        }

        /// <summary>
        /// Returns the type of class to construct based on the specified XML element.
        /// </summary>
        protected Type typeToConstruct(IXmlElement element)
        {
            foreach (ElementSignature signature in elementSignatures)
            {
                Type classType = signature.getClassType(element);

                if (classType != null)
                {
                    return classType;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the name of a factory method based on the specified XML element.
        /// </summary>
        protected string getFactoryMethodName(IXmlElement element)
        {
            foreach (ElementSignature signature in elementSignatures)
            {
                Type classType = signature.getClassType(element);

                if (classType != null)
                {
                    return signature.getFactoryMethodName(element);
                }
            }

            return "";
        }

        /// <summary>
        /// Returns the ElementSignature corresponding to the specified element
        /// </summary>
        private ElementSignature getSignature(IXmlElement element)
        {
            foreach (ElementSignature signature in elementSignatures)
            {
                if (signature.Matches(element))
                {
                    return signature;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Represents a "signature" used to characterize an XmlElement
    /// </summary>
    internal class ElementSignature : IComparable
    {
        private readonly StringCollection attributeNames = new StringCollection();
        private readonly Type classType;
        private readonly string elementName;
        private readonly string factoryMethodName;

        public ElementSignature(string elementName, Type classType, string[] attributeNames)
        {
            this.classType = classType;
            this.elementName = elementName;
            this.attributeNames.AddRange(attributeNames);
        }

        public ElementSignature(Type classType, string factoryMethodName, string elementName, string[] attributeNames)
            : this(elementName, classType, attributeNames)
        {
            this.factoryMethodName = factoryMethodName;
        }

        /// <summary>
        /// Returns the class type associated with the specified XML element.
        /// </summary>
        public Type getClassType(IXmlElement element)
        {
            if (Matches(element))
            {
                return classType;
            }

            return null;
        }

        /// <summary>
        /// Returns the factory method name associated with the specified XML element.
        /// </summary>
        public string getFactoryMethodName(IXmlElement element)
        {
            if (Matches(element))
            {
                return factoryMethodName;
            }

            return "";
        }

        /// <summary>
        /// Indicates whether this signature matches that of the specfied XML element.
        /// </summary>
        /// <remarks>
        /// This signature is deemed to match that of the XML element if:
        ///  1) They have the same element name, and
        ///  2) All attributes in this signature also appear in the XML element.
        /// </remarks>
        public bool Matches(IXmlElement element)
        {
            if (elementName.CompareTo(element.Name) != 0)
            {
                return false;
            }

            foreach (string name in attributeNames)
            {
                if (!element.GetAttributeNames().Contains(name))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(Object obj)
        {
            var other = obj as ElementSignature;

            if (elementName.CompareTo(other.elementName) != 0)
            {
                return false;
            }

            if (classType != other.classType)
            {
                return false;
            }

            if (attributeNames.Count != other.attributeNames.Count)
            {
                return false;
            }

            foreach (string name in attributeNames)
            {
                if (!other.attributeNames.Contains(name))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region IComparable Interface

        public int CompareTo(object obj)
        {
            if (obj is ElementSignature)
            {
                var otherSignature = obj as ElementSignature;

                if (elementName == otherSignature.elementName)
                {
                    // signature with greatest number of attributes is the "lesser" signature
                    return (otherSignature.attributeNames.Count - attributeNames.Count);
                }
                else
                {
                    // element names compare alphabetically
                    return (elementName.CompareTo(otherSignature.elementName));
                }
            }

            throw new ArgumentException("object is not an ElementSignature");
        }

        #endregion
    }

    /// <summary>
    /// Collection of ElementSignature objects.
    /// </summary>
    internal class ElementSignatureList : ArrayList
    {
        // Adds the specified signature to the list, or replaces an existing signature
        public void Set(ElementSignature signature)
        {
            for (int i = 0; i < Count; i++)
            {
                var s = (ElementSignature)this[i];

                if (s.Equals(signature))
                {
                    this[i] = signature;
                    return;
                }
            }

            Add(signature);

            // order signatures with same element name by descending number of number of attributes
            Sort();
        }
    }
}