// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;

namespace Tawala.Projects
{
    /// <summary>
    /// Interface for recursive enumeration.
    /// </summary>
    public interface IRecursiveEnumerable
    {
        IEnumerable RecursiveEnumerator { get; }
    }
}