// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Reflection;
using NUnit.Framework;
using Tawala.Functions.Controls;
using Tawala.Functions.Runtime;

namespace TawalaTest.TestSupport
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
        protected IFunctionRepository functionRepository;
        protected IFunctionInfoCollection functions;

        protected void functionSetup()
        {
            functionRepository = FunctionLoader.Repository;
            functions = functionRepository.Functions;
        }

        protected void functionTearDown()
        {
            functionRepository = null;
            functions = null;
        }

        protected static Type getReferencedFormsHelperType()
        {
            forceReferenceToFunctonControlsAssembly();

            Assembly functionControls = getFunctionControlsAssembly();
            return functionControls.GetType("Tawala.Functions.Controls.ReferencedFormsHelper");
        }

        private static void forceReferenceToFunctonControlsAssembly()
        {
            Assert.IsFalse(ControlManager.IsFunctionDialogOpen());
        }

        private static Assembly getFunctionControlsAssembly()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly functionControls = null;

            foreach (Assembly a in assemblies)
            {
                Console.WriteLine(a.GetName().Name);
                if (a.GetName().Name.CompareTo("FunctionControls") == 0)
                {
                    functionControls = a;
                    break;
                }
            }

            Assert.IsNotNull(functionControls, "Couldn't find FunctionControls assembly.");
            return functionControls;
        }
    }
}