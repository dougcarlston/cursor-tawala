// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Functions.Runtime;

namespace Tawala.Functions.Controls
{
    internal interface IConfigureFunctionControl
    {
        void CommitPendingEdits();
        void EditFunction(IFunction function);
        bool IsOK();
        void FreezeLayout();
        void ThawLayout();
    }
}