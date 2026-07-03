using System;
using Tawala.Functions.Runtime;

namespace TawalaTest.TestingSupport
{
    ///
    /// Use this base class in conjunction with a namespace wide setup/teardown NUnit class
    /// See AcceptanceTest - NamespaceSetupTeardown.cs
    /// 
    /// <summary>
    /// Base class for other test classes in this theme.
    /// </summary>
    public class FunctionTestBase
    {
        protected IFunctionRepository functionAssembly;
        protected IFunctionInfoCollection functions;

        protected string NEWLINE
        {
            get { return Environment.NewLine; }
        }

        protected void functionSetup()
        {
            functionAssembly = FunctionLoader.Current;
            functions = functionAssembly.Functions;
        }

        protected void functionTearDown()
        {
            functionAssembly = null;
            functions = null;
        }
    }
}