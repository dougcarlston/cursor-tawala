// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Projects
{
    public interface IAnyField
    {
        int Id { get; }
        string QualifiedFieldName { get; }
    }
}