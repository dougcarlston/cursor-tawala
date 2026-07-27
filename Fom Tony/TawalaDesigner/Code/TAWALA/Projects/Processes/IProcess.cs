// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Components;
using Tawala.Projects.Fields;

namespace Tawala.Projects.Processes
{
    public interface IProcess : IProjectComponent
    {
        VariableList Variables { get; set; }
    }
}