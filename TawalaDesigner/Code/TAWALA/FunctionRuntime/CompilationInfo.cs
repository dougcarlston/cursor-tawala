// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;

namespace Tawala.Functions.Runtime
{
    [Serializable] // so that it can cross AppDomain boundaries
    public class CompilationInfo
    {
        private readonly CompilerResults compilerResults;
        private readonly string signature;
        private readonly Collection<string> tempSourceFilesAsSingleString = new Collection<string>();

        public CompilationInfo(CompilerResults results, string _signature)
        {
            signature = _signature;
            compilerResults = results;
            processCompilerResults(results);
        }

        public string Path
        {
            get
            {
                return compilerResults.PathToAssembly;
            }
        }

        public string Signature
        {
            get
            {
                return signature;
            }
        }

        public StringCollection CompilerOutput
        {
            get
            {
                return compilerResults.Output;
            }
        }

        public CompilerErrorCollection Errors
        {
            get
            {
                return compilerResults.Errors;
            }
        }

        public Collection<string> SourceFileText
        {
            get
            {
                return tempSourceFilesAsSingleString;
            }
        }

        private void processCompilerResults(CompilerResults results)
        {
            foreach (string path in results.TempFiles)
            {
                if (path.EndsWith(".cs"))
                {
                    tempSourceFilesAsSingleString.Add(File.ReadAllText(path));
                }
            }

            compilerResults.TempFiles.KeepFiles = false;
            compilerResults.TempFiles.Delete();
        }
    }
}