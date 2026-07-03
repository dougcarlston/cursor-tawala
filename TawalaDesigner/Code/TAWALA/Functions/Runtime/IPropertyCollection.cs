// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Functions.Runtime
{
    public interface IPropertyCollection
    {
        object this[int index] { get; set; }
        object this[string id] { get; set; }
        object this[IParameterInfo info] { get; set; }
        int PropertyCount { get; }
    }
}