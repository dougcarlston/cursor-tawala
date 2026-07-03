// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;

namespace Tawala.Functions.Runtime
{
    public interface IParameterInfoCollection : IList<IParameterInfo>
    {
        IParameterInfo this[string id] { get; }
        ICollection<string> Ids { get; }
        bool Contains(string id);
        string ToDisplayString(IFunction function);
    }
}