// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Projects.Components
{
    public interface IProjectComponent : IProjectComponentXml
    {
        string Name { get; set; }
        string UserVisibleComponentTypeName { get; }
        string ToString();
    }
}