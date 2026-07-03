// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Reflection;

namespace Tawala.Functions.Runtime
{
    public interface IFunctionRepository : IXPathNavigatorProvider
    {
        /// <summary>IFunctionInfo objects for every generated class.  Indexers:  [int index] and [string id]</summary>
        IFunctionInfoCollection Functions { get; }

        /// <summary>ICategoryInfo provides a FunctionInfoCollection for associated functions.</summary>
        ICategoryInfoCollection Categories { get; }

        string Signature { get; }

        string Created { get; }

        /// <summary>
        /// This is the only interface member not implemented in class FunctionAssembly
        /// It is implemented in the generated assembly by a class that is derived from
        /// class FunctionAssembly 
        /// </summary>
        Assembly GeneratedAssembly { get; }
    }
}