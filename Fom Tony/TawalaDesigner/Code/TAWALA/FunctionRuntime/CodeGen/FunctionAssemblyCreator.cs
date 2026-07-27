// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Resources;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.CSharp;
using Tawala.Functions.Runtime.Properties;

#pragma warning disable 414 // unrefereced variable

namespace Tawala.Functions.Runtime.CodeGen
{
    internal sealed class FunctionAssemblyCreator : Creator
    {
        private static readonly string[] assemblyReferences = {
                                                                  "mscorlib.dll", "System.dll", "System.Data.dll",
                                                                  "System.Windows.Forms.dll",
                                                                  "System.Xml.dll", "Functions.dll", "FunctionRuntime.dll", "Projects.dll"
                                                              };

        private readonly FunctionLoader.BuildInfo buildInfo;

        private readonly CSharpCodeProvider csharp = new CSharpCodeProvider();
        private readonly XPathNavigator repositoryRoot;
        private readonly string signature;

        private readonly Collection<StringBuilder> sourceUnits = new Collection<StringBuilder>();
        private CSharpCompilerParameters compilerParameters;
        private string logPath;

        private DateTime now = DateTime.Now;
        private string outputPath;
        private StringBuilder sbAssemblyInfo;

        internal FunctionAssemblyCreator(XPathNavigator xPathRoot, FunctionLoader.BuildInfo _buildInfo)
        {
            buildInfo = _buildInfo;
            repositoryRoot = xPathRoot;
            signature = repositoryRoot.GetAttribute("signature", "");
        }

        internal CompilationInfo BuildFunctionAssembly()
        {
            createOutputDirectory();

            createAssemblyInfo();
            createAssemblyClass();

            createSupportClass(Resources.UTILITY);

            compilerParameters = new CSharpCompilerParameters(outputPath, buildInfo.DllName);

            createFunctionClasses();

            addRepositoryXmlAsResource();

            addAssemblyRefs(assemblyReferences);

            createSupportClass(Resources.CATEGORY_INFO);
            createSupportClass(Resources.CATEGORY_INFO_COLLECTION);
            createSupportClass(Resources.COMPOSITE_PARAMETER_BASE);
            createSupportClass(Resources.COMPOSITE_PARAMETER_COLLECTION_BASE);
            createSupportClass(Resources.FUNCTION_INFO_COLLECTION);
            //            createSupportClass(Resources.PARAMETER_CONTAINER_BASE);
            createSupportClass(Resources.PROPERTY_COLLECTION);

            var sourceCompileUnits = new string[sourceUnits.Count];
            for (int i = 0; i < sourceCompileUnits.Length; ++i)
            {
                StringBuilder sourceUnit = sourceUnits[i];
                sourceUnit.Replace("\t", "    ");
                string source = sourceUnit.ToString();

                if (source.Contains("${"))
                {
                    Debugger.Break();
                }

                sourceCompileUnits[i] = source;
            }

            CompilerResults results = csharp.CompileAssemblyFromSource(compilerParameters, sourceCompileUnits);

            if (results.Errors.Count > 0)
            {
                foreach (CompilerError error in results.Errors)
                {
                    logFunctionError(error.ToString());
                }
            }

            return new CompilationInfo(results, signature);
        }

        private void addRepositoryXmlAsResource()
        {
            string resourcePath = Path.Combine(outputPath, "functions.resources");
            using (var resourceWriter = new ResourceWriter(resourcePath))
            {
                resourceWriter.AddResource("REPOSITORY_XML", reformatXmlToBeReadable());
                resourceWriter.Generate();
                compilerParameters.EmbeddedResources.Add(resourcePath);
            }
        }

        internal void AddFunction(string gid, string typeName, StringBuilder sbFunction)
        {
            sourceUnits.Add(sbFunction);
        }

        internal void AddFunctionInfo(StringBuilder sbFunctionInfo)
        {
            sourceUnits.Add(sbFunctionInfo);
        }

        private void createAssemblyInfo()
        {
            sbAssemblyInfo = new StringBuilder(Resources.ASSEMBLY_INFO);
            sbAssemblyInfo.Replace("${YEAR}", now.Year.ToString());

            string version = string.Format("{0}.{1}.{2}.{3}", now.Year, now.Month, now.Day, now.TimeOfDay.Minutes);
            sbAssemblyInfo.Replace("${ASSEMBLYVERSION}", version);

            sourceUnits.Add(sbAssemblyInfo);
        }

        private void createAssemblyClass()
        {
            var sb = new StringBuilder();
            sb.Append(Resources.CODE_PREAMBLE);
            sb.Append(Resources.FUNCTION_REPOSITORY);

            sb.Replace("${NAMESPACE}", buildInfo.NameSpace);
            sb.Replace("${ENTRYASSEMBLYVERSION}", buildInfo.EntryAssemblyVer);
            sb.Replace("${REPOSITORYSIGNATURE}", signature);
            sb.Replace("${CREATIONTIME}", now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.Replace("${YEAR}", now.Year.ToString());

            string version = string.Format("{0}.{1}.{2}.{3}", now.Year, now.Month, now.Day, now.TimeOfDay.Minutes);
            sb.Replace("${ASSEMBLYVERSION}", version);

            sourceUnits.Add(sb);
        }

        private void addAssemblyRefs(string[] assemblies)
        {
            foreach (string name in assemblies)
            {
                compilerParameters.ReferencedAssemblies.Add(name);
            }
        }

        private void createOutputDirectory()
        {
            outputPath = Path.Combine(buildInfo.BaseOutputPath, buildInfo.EntryAssemblyVer + "_" + signature);
            logPath = Path.Combine(buildInfo.BaseOutputPath, "function.log");
            File.WriteAllText(logPath, string.Empty);

            try
            {
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }
            }
            catch
            {
            }
        }

        private void createSupportClass(string classDef)
        {
            var sb = new StringBuilder();
            sb.Append(Resources.CODE_PREAMBLE);
            sb.Replace("${YEAR}", DateTime.Now.Year.ToString());
            sb.Replace("${NAMESPACE}", buildInfo.NameSpace);

            sb.Append(classDef);
            sourceUnits.Add(sb);
        }

        private void createFunctionClasses()
        {
            XPathNodeIterator iterator = getAllComponentTypesFromXmlSortedByName();

            iterator.MoveNext(); // skip categories

            while (iterator.MoveNext())
            {
                try
                {
                    var classCreator = new FunctionClassCreator(this, iterator.Current.Clone(), buildInfo.NameSpace);
                    classCreator.Create();
                }
                catch (Exception e)
                {
                    logFunctionError(e.ToString());
                }
            }
        }

        private void logFunctionError(string message)
        {
            if (logPath != null)
            {
                var sb = new StringBuilder();
                sb.AppendLine(message);

                File.AppendAllText(logPath, sb.ToString());
            }
        }

        private XPathNodeIterator getAllComponentTypesFromXmlSortedByName()
        {
            XPathExpression expr = XPathExpression.Compile("/component-repository/child::node()");
            expr.AddSort("@name", StringComparer.Ordinal);
            XPathNodeIterator iterator = repositoryRoot.Select(expr);
            return iterator;
        }

        private string reformatXmlToBeReadable()
        {
            var doc = new XmlDocument();
            doc.LoadXml(repositoryRoot.OuterXml);
            doc.Save(Path.Combine(outputPath, "repository.xml"));
            return doc.OuterXml;
        }

        #region Nested type: CSharpCompilerParameters

        internal class CSharpCompilerParameters : CompilerParameters
        {
            internal CSharpCompilerParameters(string outputPath, string assemblyName)
            {
                setParameters(outputPath, assemblyName);
            }

            private void setParameters(string outputPath, string assemblyName)
            {
                GenerateExecutable = false;
                GenerateInMemory = false;
                WarningLevel = 4;

#if DEBUG
                CompilerOptions = "/nologo /optimize- /debug:full /nowarn:414 /langversion:default";
                IncludeDebugInformation = true;
#else
				CompilerOptions = "/nologo /optimize+ /debug:pdbonly /nowarn:414 /langversion:default";
				IncludeDebugInformation = false;
#endif
                TempFiles = new TempFileCollection(outputPath, true);
                OutputAssembly = Path.Combine(outputPath, assemblyName + ".dll");
            }
        }

        #endregion
    }
}