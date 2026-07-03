// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;

namespace Tawala.Functions.Runtime
{
    public interface IFunctionInfoCollection : IList<IFunctionInfo>
    {
        IFunctionInfo this[string id] { get; }
        IFunctionInfo this[Type type] { get; }
        bool Contains(string id);
        bool Contains(Type type);
    }
}