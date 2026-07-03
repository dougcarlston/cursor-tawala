// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;

namespace Tawala.Functions.Runtime
{
    public interface ICategoryInfoCollection : IList<ICategoryInfo>
    {
        ICategoryInfo this[string name] { get; }
        bool Contains(string name);
    }
}