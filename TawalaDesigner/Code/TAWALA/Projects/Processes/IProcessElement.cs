// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Projects.Processes
{
    public interface IProcessElement : IRecursiveEnumerable
    {
        Boolean IsValid { get; }
    }
}