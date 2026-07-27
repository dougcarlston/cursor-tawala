// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Tawala.Functions.Runtime
{
    public static class RuntimeTypeResolver
    {
        private static bool initialized;

        public static void Init()
        {
            if (initialized)
            {
                return;
            }

            Cache.Init();
            initialized = true;
        }

        public static object Construct(string typeName, params object[] args)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }

            Type t = Cache.FindType(typeName);

            var argTypes = new Type[args.Length];
            for (int i = 0; i < argTypes.Length; ++i)
            {
                argTypes[i] = args[i].GetType();
            }
            var modifiers = new ParameterModifier[argTypes.Length];

            ConstructorInfo construct = t.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null,
                                                         argTypes, modifiers);

            return construct.Invoke(args);
        }

        #region Nested type: Cache

        private static class Cache
        {
            private static List<Assembly> assemblyList;
            private static Dictionary<string, RuntimeTypeHandle> typeCache;

            public static void Init()
            {
                assemblyList = new List<Assembly>();
                typeCache = new Dictionary<string, RuntimeTypeHandle>();
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

                string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).ToLowerInvariant();

                foreach (Assembly a in assemblies)
                {
                    string location = Path.GetDirectoryName(a.Location).ToLowerInvariant();

                    if (dir.CompareTo(location) != 0)
                    {
                        continue;
                    }

                    assemblyList.Add(a);
                }

                AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                AppDomain.CurrentDomain.TypeResolve += CurrentDomain_TypeResolve;
            }

            public static Type FindType(string typeName)
            {
                if (!typeCache.ContainsKey(typeName))
                {
                    Type type = null;
                    foreach (Assembly a in assemblyList)
                    {
                        type = a.GetType(typeName, false, false);
                        if (type != null)
                        {
                            break;
                        }
                    }

                    if (type != null)
                    {
                        typeCache[typeName] = type.TypeHandle;
                    }
                }

                if (typeCache.ContainsKey(typeName))
                {
                    return Type.GetTypeFromHandle(typeCache[typeName]);
                }

                return null;
            }

            private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
            {
                if (!assemblyList.Contains(args.LoadedAssembly))
                {
                    assemblyList.Add(args.LoadedAssembly);
                }
            }

            private static Assembly CurrentDomain_TypeResolve(object sender, ResolveEventArgs args)
            {
                Type t = FindType(args.Name);

                return t != null ? t.Assembly : null;
            }

            private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
            {
                string resolvedName = args.Name;

                foreach (Assembly a in assemblyList)
                {
                    AssemblyName assemblyName = a.GetName();
                    if (assemblyName.Name.CompareTo(resolvedName) == 0)
                    {
                        return a;
                    }
                    if (assemblyName.FullName.CompareTo(resolvedName) == 0)
                    {
                        return a;
                    }
                }

                return null;
            }
        }

        #endregion
    }
}