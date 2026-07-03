// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Tawala.Functions.Runtime.CodeGen;
using Tawala.Functions.Runtime.Private;

namespace Tawala.Functions.Runtime
{
    public static class FunctionLoader
    {
        public static IFunctionRepository Repository { get; private set; }

        public static bool Load(string path)
        {
            if (Repository != null)
            {
                return true;
            }

            if (!File.Exists(path))
            {
                return false;
            }

            Assembly functionAssembly = loadAssemblyIntoMemory(path);
            createRepositoryObject(functionAssembly);
            return Repository != null;
        }

        public static bool BuildAndLoad(string xml)
        {
            if (xml.Contains("component-repository "))
            {
                CompilationInfo info = rebuild(xml);
                if (info != null)
                {
                    return Load(info.Path);
                }
            }
            return false;
        }

        /// <summary>The dll should be located under Function\[designer-version]_[last-known-repository-signature]\Tawala.Functions.Generated.dll</summary>
        public static string GetPossibleFunctionDllLocation()
        {
            string subdir = Path.Combine(FunctionsLocalApplicationDataPath, getEntryAssemblyVer(0) + "_" + readLastKnownSignature());
            return Path.Combine(subdir, generatedDllName);
        }

        public static string GetLastKnownSignature()
        {
            return readLastKnownSignature();
        }

        #region PRIVATE - Ignore the man behind the curtains

        private static string localApplicationDataPath = string.Empty;
        private static string nunitNamespaceRandomize = string.Empty;

        static FunctionLoader()
        {
            RuntimeTypeResolver.Init();
        }

        private static string LocalApplicationDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(localApplicationDataPath))
                {
                    localApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    localApplicationDataPath = Path.Combine(localApplicationDataPath, getAttribute<AssemblyCompanyAttribute>().Company);
                    localApplicationDataPath = Path.Combine(localApplicationDataPath, getAttribute<AssemblyProductAttribute>().Product);
                    localApplicationDataPath = Path.Combine(localApplicationDataPath,
                                                            getAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
                }
                return localApplicationDataPath;
            }
        }

        private static string FunctionsLocalApplicationDataPath { get { return Path.Combine(LocalApplicationDataPath, "Functions"); } }

        private static string GeneratedCodeNamespace { get { return "Tawala.Functions.Generated" + nunitNamespaceRandomize; } }

        private static string generatedDllNameWithoutExt { get { return "Tawala.Functions.Generated" + nunitNamespaceRandomize; } }

        private static string generatedDllName { get { return generatedDllNameWithoutExt + ".dll"; } }

        private static T getAttribute<T>() where T : Attribute
        {
            return typeof(FunctionLoader).Assembly.GetCustomAttributes(typeof(T), false)[0] as T;
        }

        private static Assembly loadAssemblyIntoMemory(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Generated Functions not found", path);
            }

            byte[] assemblyBytes = File.ReadAllBytes(path);

            string pdb = path.Replace(".dll", ".pdb");
            if (File.Exists(pdb))
            {
                byte[] assemblyPDB = File.ReadAllBytes(pdb);
                return AppDomain.CurrentDomain.Load(assemblyBytes, assemblyPDB);
            }

            return AppDomain.CurrentDomain.Load(assemblyBytes);
        }

        private static void createRepositoryObject(Assembly assembly)
        {
            const BindingFlags flags = BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static;

            try
            {
                Type type = assembly.GetType(GeneratedCodeNamespace + ".FunctionRepository", true, false);
                Repository = type.InvokeMember("CreateInstance", flags, null, null, null) as IFunctionRepository;
                type.InvokeMember("InitializeInstance", flags, null, null, null);
                writeSignatureToFileOnSuccess(Repository);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        private static CompilationInfo rebuild(string xml)
        {
            CompilationInfo compilationInfo = null;

            try
            {
                Repository = null;
                long ticks = DateTime.Now.Ticks/100;
                randomizeNamespaceNameUnderNUnit(ticks);

                var buildInfo = new BuildInfo(xml, FunctionsLocalApplicationDataPath, getEntryAssemblyVer(ticks), generatedDllNameWithoutExt,
                                              GeneratedCodeNamespace);
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
                if (compilationInfo == null || !File.Exists(compilationInfo.Path) || string.IsNullOrEmpty(compilationInfo.Signature))
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

        private static void writeSignatureToFileOnSuccess(IFunctionRepository repository)
        {
            if (repository != null)
            {
                File.WriteAllText(getSignatureFilePath(), repository.Signature);
            }
        }

        #region Nested type: BuildInfo

        public class BuildInfo
        {
            public BuildInfo(string xml, string path, string ver, string _dllName, string _nameSpace)
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