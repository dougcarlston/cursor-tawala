// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Expressions;

namespace Tawala.Projects.Forms
{
    public interface IHiddenField : IFormItem, IPaletteField, IOperatorDataSource
    {
        string Name { get; set; }
    }
}