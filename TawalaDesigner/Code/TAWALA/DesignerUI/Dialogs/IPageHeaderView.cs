// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.DesignerUI
{
    public interface IPageHeaderView : IWin32Window
    {
        string Text { get; set; }
        string ImageLocation { get; set; }
    }
}