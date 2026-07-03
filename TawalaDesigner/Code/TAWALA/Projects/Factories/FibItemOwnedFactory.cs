// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Diagnostics;
using System.Reflection;
using Tawala.Projects.Factories;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
    public class FibItemOwnedFactory<T> : Factory<T>
    {
        /// <summary>
        /// Creates and returns a new object of type T from information in the specified XML element,
        /// passing the owner to subsequently-called constructors.
        /// </summary>
        public T MakeObject(IXmlElement element, FibItem owner)
        {
            if (isNull(element))
            {
                Console.WriteLine("FibItemOwnedFactory.MakeObject: {0} isNull(element)", element.Name);
            }

            if (!isRegistered(element))
            {
                Console.WriteLine("FibItemOwnedFactory.MakeObject: {0} !isRegistered(element)", element.Name);
            }

            if (isNull(element) || !isRegistered(element))
            {
                return default(T);
            }

            Type classType = typeToConstruct(element);

            if (classType == null)
            {
                Console.WriteLine("FibItemOwnedFactory.MakeObject: {0} classType is null", element.Name);
                return default(T);
            }

            ConstructorInfo constructor = locateFibItemConstructor(classType);
            Debug.Assert(constructor != null);

            if (constructor == null)
            {
                Console.WriteLine(
                    "FibItemOwnedFactory.MakeObject: Cannot find constructor {0}(IXmlElement, FibItem) for XML element <{1}>", classType,
                    element.Name);
            }

            return invokeFibItemConstructor(element, owner, constructor);
        }

        /// <summary>
        /// Locates a constructor for classType that has IXmlElement and FibItem arguments
        /// </summary>
        private static ConstructorInfo locateFibItemConstructor(Type classType)
        {
            var argTypes = new Type[2];
            argTypes[0] = typeof(IXmlElement);
            argTypes[1] = typeof(FibItem);
            ConstructorInfo constructor = classType.GetConstructor(argTypes);
            return constructor;
        }

        /// <summary>
        /// Invokes the specified constructor and returns new object
        /// </summary>
        private static T invokeFibItemConstructor(IXmlElement element, FibItem owner, ConstructorInfo constructor)
        {
            var args = new Object[2];
            args[0] = element;
            args[1] = owner;
            return (T)constructor.Invoke(args);
        }
    }
}