// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Functions.Runtime
{
    public interface ICategoryInfo : IXPathNavigatorProvider
    {
        string Name { get; }

        /// <summary>FunctionInfo objects for function classes in this category</summary>
        IFunctionInfoCollection Functions { get; }
    }
}