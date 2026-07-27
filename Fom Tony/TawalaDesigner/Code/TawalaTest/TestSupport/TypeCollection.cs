// $Workfile: TypeCollection.cs $
// $Revision: 3 $	$Date: 1/22/07 9:36a $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Reflection;
using System.IO;

namespace TawalaTest.TestSupport
{
    public sealed class TypeCollection : Collection<Type>
    {
        public static readonly IFilter NullFilter = new nullFilter();

        public static TypeCollection Create(Type[] types, IFilter filter)
        {
            return new TypeCollection().addTypes(types, filter);
        }

        public static TypeCollection Create(Assembly a, IFilter filter)
        {
            return new TypeCollection().addTypes(a.GetTypes(), filter);
        }

        public static TypeCollection Create(Assembly[] assemblies, IFilter filter)
        {
            TypeCollection tc = new TypeCollection();

            foreach (Assembly a in assemblies)
            {
                tc. addTypes(a.GetTypes(), filter);
            }

            return tc;
        }

        private TypeCollection()
        {
        }

        public string ToString(string format)
        {
            StringBuilder list = new StringBuilder("    ");

            foreach (Type t in Items)
            {
                if (list.Length > 4)
                {
                    list.Append("\r\n    ");
                }
                list.Append(t.FullName);
            }

            return string.Format(format, list.ToString());
        }

        private TypeCollection addTypes(Type[] types, IFilter filter)
        {
            for (int i = 0; i < types.Length; ++i)
            {
                if (filter.Match(types[i]))
                    Add(types[i]);
            }

            return this;
        }

        public interface IFilter
        {
            bool Match(Type t);
        }

        class nullFilter : IFilter
        {
            public bool Match(Type t)
            {
                return true;
            }
        }
    }

    public class DerivationFilter<T> : TypeCollection.IFilter
    {
        public virtual bool Match(Type testType)
        {
            return typeof(T).IsAssignableFrom(testType);
        }
    }

    public class NotSerializableFilter<T> : DerivationFilter<T>
    {
        public override bool Match(Type testType)
        {
            return base.Match(testType) && !testType.IsSerializable && !testType.IsInterface;
        }
    }
}
