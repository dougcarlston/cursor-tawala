// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Expressions;

namespace Tawala.Projects
{
    public interface IAssignableField : IPaletteField, IOperatorDataSource
    {
        string AssignmentName { get; }
    }
}