// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.DesignerUI
{
    public interface INewProjectView
    {
        TreeNode CategoryRootNode { get; }
        ListView TemplateView { get; }
        string TemplateDescription { get; set; }
        string SelectedTemplateFile { get; }
    }
}