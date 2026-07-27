// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using Tawala.Projects.Forms.NewFormItems;

namespace Tawala.FormDesigner.FormItemOptions
{
    public interface IHeadingOptionsView
    {
        string HeadingLabel { get; set; }
        string LabelStatusText { set; }
        HeadingType HeadingType { get; set; }
    }
}