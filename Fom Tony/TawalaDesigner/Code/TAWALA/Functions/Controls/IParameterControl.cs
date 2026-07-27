// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Tawala.Functions.Controls
{
    public interface IParameterControl : IDropTarget, ISynchronizeInvoke, IWin32Window, IBindableComponent
    {
        bool CustomControl { get; }
        void CommitPendingChanges();
        event EventHandler HandleDestroyed;
    }
}