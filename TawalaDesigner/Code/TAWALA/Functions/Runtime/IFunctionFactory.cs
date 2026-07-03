// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Functions.Runtime
{
    public interface IFunctionFactory
    {
        void RegisterFunction(string id, string name);
        IFunction CreateInstanceFromId(string id);
        IFunction CreateInstanceFromInfo(IFunctionInfo functionInfo);
    }
}