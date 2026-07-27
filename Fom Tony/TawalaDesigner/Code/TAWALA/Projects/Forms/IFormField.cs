// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Projects.Forms
{
    public interface IFormField : IPaletteField
    {
        string AlternateLabel { get; set; }
        string ToString();
    }
}