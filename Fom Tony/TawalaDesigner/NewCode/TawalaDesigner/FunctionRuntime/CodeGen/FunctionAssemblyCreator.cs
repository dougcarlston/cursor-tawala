// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using Microsoft.CSharp;
using Tawala.Functions.Runtime.Properties;

#pragma warning disable 414 // unrefereced variable

namespace Tawala.Functions.Runtime.CodeGen
{
    internal sealed class FunctionAssemblyCreator : Creator
    {
        private static readonly string[] assemblyReferences =
            {
                "mscorlib.dll",
                "System.dll",
                "System.Data.dll",
                "System.Windows.Forms.dll",
                "System.Xml.dll",
                "FunctionRuntime.dll",
                "Projects.dll"
            };

        private readonly FunctionLoader.BuildInfo buildInfo;

        private readonly CSharpCodeProvider csharp = new CSharpCodeProvider();
        private readonly XPathNavigator repositoryRoot;
        private readonly string signature;

        private readonly Collection<StringBuilder> sourceUnits = new Collection<StringBuilder>();
        private CSharpCompilerParameters compilerParameters;
        private int loggedMessages;
        private string logPath;

        private DateTime now = DateTime.Now;
        private string outputPath;
        private IResourceWriter resourceWriter;
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

            compilerParameters = new CSharpCompilerParameters(outputPath, buildInfo.DllName, repositoryRoot);

            string resourcePath = Path.Combine(outputPath, "functions.resources");
            resourceWriter = new ResourceWriter(resourcePath);

            addStringResource("REPOSITORY_XML", reformatXmlToBeReadable());

            createFunctionClasses();

            addAssemblyRefs(assemblyReferences);

            resourceWriter.Generate();
            compilerParameters.EmbeddedResources.Add(resourcePath);
            resourceWriter.Dispose();

            var sourceCompileUnits = new string[sourceUnits.Count];
            for (int i = 0; i < sourceCompileUnits.Length; ++i)
            {
                sourceCompileUnits[i] = Regex.Replace(sourceUnits[i].ToString(), "\t", "    ");
                Debug.Assert(!sourceCompileUnits[i].Contains("${"));
            }

            CompilerResults results = csharp.CompileAssemblyFromSource(compilerParameters, sourceCompileUnits);

            if (results.Errors.Count > 0)
            {
                logFunctionError(string.Format("Compile time errors -- see appropriate *.out file in {0}", outputPath));
            }

            return new CompilationInfo(results, signature);
        }

        internal void AddFunction(string gid, string typeName)
        {
            sbAssemblyInfo.AppendLine(string.Format("[assembly: FC(\"{0}\", typeof({1}.{2}))]", gid, buildInfo.NameSpace,
                                                    typeName));
        }

        internal void addStringResource(string name, string value)
        {
            resourceWriter.AddResource(name, value);
        }

        private void createAssemblyInfo()
        {
            sbAssemblyInfo = new StringBuilder(Resources.ASSEMBLY_INFO_TEMPLATE);
            sbAssemblyInfo.Replace("${YEAR}", now.Year.ToString());

            string version = string.Format("{0}.{1}.{2}.{3}", now.Year, now.Month, now.Day, now.TimeOfDay.Minutes);
            sbAssemblyInfo.Replace("${ASSEMBLYVERSION}", version);

            sourceUnits.Add(sbAssemblyInfo);
        }

        private void createAssemblyClass()
        {
            var sb = new StringBuilder();
            sb.Append(Resources.CODE_PREAMBLE_TEMPLATE);
            sb.Append(Resources.FUNCTION_REPOSITORY_TEMPLATE);

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
            try
            {
                Directory.CreateDirectory(outputPath);
            }
            catch (Exception)
            {
            }
        }

        private void createSupportClass(string classDef)
        {
            var sb = new StringBuilder();
            sb.Append(Resources.CODE_PREAMBLE_TEMPLATE);
            sb.Replace("${YEAR}", DateTime.Now.Year.ToString());
            sb.Replace("${NAMESPACE}", buildInfo.NameSpace);

            sb.Append(classDef);
            sourceUnits.Add(sb);
        }

        private void createFunctionClasses()
        {
            FunctionClassCreator classCreator = null;

            XPathNodeIterator iterator = getAllComponentTypesFromXmlSortedByName();

            while (iterator.MoveNext())
            {
                try
                {
                    classCreator = new FunctionClassCreator(this, iterator.Current.Clone(), buildInfo.NameSpace);
                    StringBuilder sbClassDef = classCreator.Create();
                    sourceUnits.Add(sbClassDef);
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
                if (loggedMessages == 0)
                {
                    sb.AppendLine();
                    sb.AppendLine(DateTime.Now.ToString("=============== yyyy-MM-dd  HH:mm:ss ==============="));
                }
                sb.AppendLine();
                sb.AppendLine(message);

                loggedMessages++;

                File.AppendAllText(logPath, sb.ToString());
            }
        }

        private XPathNodeIterator getAllComponentTypesFromXmlSortedByName()
        {
            XPathExpression expr = XPathExpression.Compile("descendant::display-component | descendant::function");
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
            private static readonly string debugCompilerOptions = "/nologo /optimize- /debug:full /nowarn:414 /langversion:default";
            private static readonly string releaseCompilerOptions = "/nologo /optimize+ /debug:pdbonly /nowarn:414 /langversion:default";

            internal CSharpCompilerParameters(string outputPath, string assemblyName, XPathNavigator repositoryRoot)
            {
                setParameters(outputPath, assemblyName);
            }

            private void setParameters(string outputPath, string assemblyName)
            {
                GenerateExecutable = false;
                GenerateInMemory = false;
                WarningLevel = 4;

#if DEBUG
                CompilerOptions = debugCompilerOptions;
                IncludeDebugInformation = true;
#else
				CompilerOptions = releaseCompilerOptions;
				IncludeDebugInformation = false;
#endif
                TempFiles = new TempFileCollection(outputPath, true);
                OutputAssembly = Path.Combine(outputPath, assemblyName + ".dll");
            }
        }

        #endregion
    }
}