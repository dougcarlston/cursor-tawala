// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Reflection;
using System.Xml.XPath;

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

        Assembly GeneratedAssembly { get; }
        XPathNavigator FindByGid(string gid);
    }
}