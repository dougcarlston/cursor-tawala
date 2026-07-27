// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Tawala.Functions.Runtime.CodeGen;

namespace Tawala.Functions.Runtime
{
    public static class FunctionLoader
    {
        public static IFunctionRepository Current { get { return functionRepositoryInstance; } }

        public static IFunctionRepository Load(string path)
        {
            return getIFunctionRepository(path);
        }

        public static CompilationInfo Rebuild(string xml)
        {
            return xml.Contains("component-repository ") ? rebuild(xml) : null;
        }

        /// <summary>The dll should be located under Function\[designer-version]_[last-known-repository-signature]\Tawala.Functions.Generated.dll</summary>
        public static string GetPossibleFunctionDllLocation()
        {
            string subdir = Path.Combine(FunctionsLocalApplicationDataPath,
                                         getEntryAssemblyVer(0) + "_" + readLastKnownSignature());
            return Path.Combine(subdir, generatedDllName);
        }

        public static string GetLastKnownSignature()
        {
            return readLastKnownSignature();
        }

        #region PRIVATE - Ignore the man behind the curtains

        private static IFunctionRepository functionRepositoryInstance;
        private static string localApplicationDataPath = string.Empty;
        private static string nunitNamespaceRandomize = string.Empty;

        static FunctionLoader()
        {
            RuntimeTypeResolver.Init();
        }

        internal static string LocalApplicationDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(localApplicationDataPath))
                {
                    localApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    localApplicationDataPath = Path.Combine(localApplicationDataPath,
                                                            getAttribute<AssemblyCompanyAttribute>().Company);
                    localApplicationDataPath = Path.Combine(localApplicationDataPath,
                                                            getAttribute<AssemblyProductAttribute>().Product);
                    localApplicationDataPath = Path.Combine(localApplicationDataPath,
                                                            getAttribute<AssemblyInformationalVersionAttribute>().
                                                                InformationalVersion);
                }
                return localApplicationDataPath;
            }
        }

        internal static string FunctionsLocalApplicationDataPath { get { return Path.Combine(LocalApplicationDataPath, "Functions"); } }

        private static string GeneratedCodeNamespace { get { return "Tawala.Functions.Generated" + nunitNamespaceRandomize; } }

        private static string generatedDllNameWithoutExt { get { return "Tawala.Functions.Generated" + nunitNamespaceRandomize; } }

        private static string generatedDllName { get { return generatedDllNameWithoutExt + ".dll"; } }

        private static T getAttribute<T>() where T : Attribute
        {
            return typeof(FunctionLoader).Assembly.GetCustomAttributes(typeof(T), false)[0] as T;
        }

        private static Assembly loadAssemblyIntoMemory(string path)
        {
            byte[] assemblyBytes = File.ReadAllBytes(path);

            string pdb = path.Replace(".dll", ".pdb");
            if (File.Exists(pdb))
            {
                byte[] assemblyPDB = File.ReadAllBytes(pdb);
                return AppDomain.CurrentDomain.Load(assemblyBytes, assemblyPDB);
            }

            return AppDomain.CurrentDomain.Load(assemblyBytes);
        }

        private static IFunctionRepository getIFunctionRepository(string path)
        {
            try
            {
                if (!File.Exists(path) || functionRepositoryInstance != null)
                {
                    return functionRepositoryInstance;
                }

                Assembly assembly = loadAssemblyIntoMemory(path);

                functionRepositoryInstance = createIFunctionRepositoryInstance(assembly);
                initializeFunctionRepositoryInstance();

                writeSignatureToFileOnSuccess();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                functionRepositoryInstance = null;
                throw;
            }

            return functionRepositoryInstance;
        }

        private static IFunctionRepository createIFunctionRepositoryInstance(Assembly assembly)
        {
            const BindingFlags methodFlags = BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Instance;
                                            
            foreach (Type type in assembly.GetExportedTypes())
            {
                if (type.GetInterface(typeof(IFunctionRepository).Name) != null)
                {
                    return type.InvokeMember("", methodFlags, null, null, new object[] {}) as IFunctionRepository;
                }
            }

            return null;
        }

        private static void initializeFunctionRepositoryInstance()
        {
            const BindingFlags flags = BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance;
            functionRepositoryInstance.GetType().InvokeMember("Initialize", flags, null, functionRepositoryInstance, null);
        }

        private static CompilationInfo rebuild(string xml)
        {
            CompilationInfo compilationInfo = null;

            try
            {
                functionRepositoryInstance = null;
                long ticks = DateTime.Now.Ticks/100;
                randomizeNamespaceNameUnderNUnit(ticks);

                var buildInfo = new BuildInfo(xml, FunctionsLocalApplicationDataPath, getEntryAssemblyVer(ticks),
                                              generatedDllNameWithoutExt, GeneratedCodeNamespace);
                IGenerator generator = new Generator();

                compilationInfo = generator.Build(buildInfo);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                throw;
            }
            finally
            {
                if (compilationInfo == null || !File.Exists(compilationInfo.Path) ||
                    string.IsNullOrEmpty(compilationInfo.Signature))
                {
                }
            }

            return compilationInfo;
        }

        /// If running under NUnit, randomize namespace
        private static void randomizeNamespaceNameUnderNUnit(long ticks)
        {
            nunitNamespaceRandomize = string.Empty;
            if (Assembly.GetEntryAssembly() == null) // running under nunit for instance
            {
                nunitNamespaceRandomize = ticks.ToString("x");
            }
        }

        private static string getSignatureFilePath()
        {
            return Path.Combine(LocalApplicationDataPath, "repository-signature");
        }

        private static string getEntryAssemblyVer(long ticks) // i.e. designer.exe or null under NUnit
        {
            Assembly entry = Assembly.GetEntryAssembly();
            return entry != null ? entry.GetName().Version.ToString() : "null_" + ticks.ToString("x");
        }

        private static string readLastKnownSignature()
        {
            string signature = string.Empty;
            string sigPath = getSignatureFilePath();
            if (File.Exists(sigPath))
            {
                signature = File.ReadAllText(sigPath);
            }
            return signature;
        }

        private static void writeSignatureToFileOnSuccess()
        {
            if (functionRepositoryInstance != null)
            {
                File.WriteAllText(getSignatureFilePath(), functionRepositoryInstance.Signature);
            }
        }

        #region Nested type: BuildInfo

        public class BuildInfo
        {
            public BuildInfo(string xml, string path, string ver, string _dllName,
                             string _nameSpace)
            {
                NameSpace = _nameSpace;
                BaseOutputPath = path;
                EntryAssemblyVer = ver;
                DllName = _dllName;
                RepositoryXml = stripNamespacesFromXml(xml);
                FunctionDirectory.CreateOrClean(path);
            }

            public string NameSpace { get; private set; }
            public string DllName { get; private set; }
            public string RepositoryXml { get; private set; }
            public string BaseOutputPath { get; private set; }
            public string EntryAssemblyVer { get; private set; }

            private static string stripNamespacesFromXml(string xml)
            {
                xml = xml.Replace("<tr:", "<").Replace("</tr:", "</");
                xml = Regex.Replace(xml, @" xmlns:[^=]+=[^\s]+", "");
                return xml;
            }
        }

        #endregion

        #region Nested type: FunctionDirectory

        private static class FunctionDirectory
        {
            public static void CreateOrClean(string directory)
            {
                if (!directory.Contains(LocalApplicationDataPath) || !directory.Contains(@"\Functions"))
                {
                    return;
                }

                createDirectory(directory);
                string[] directories = getSubdirectories(directory);

                foreach (string name in directories)
                {
                    deleteDirectory(name);
                }
            }

            private static void createDirectory(string directory)
            {
                try
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                }
                catch
                {
                }
            }

            private static void deleteDirectory(string directory)
            {
                try
                {
                    Directory.Delete(directory, true);
                }
                catch
                {
                }
            }

            private static string[] getSubdirectories(string directory)
            {
                var directories = new string[] {};
                try
                {
                    directories = Directory.GetDirectories(directory);
                }
                catch
                {
                }
                return directories;
            }
        }

        #endregion

        #endregion
    }
}