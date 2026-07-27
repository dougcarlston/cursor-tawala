// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Functions.Runtime;

namespace Tawala.Projects.Forms
{
    public interface IBlank : IFormField
    {
        bool Required { get; set; }
        int Length { get; set; }
        IFunction ValidationFunction { get; set; }
    }
}