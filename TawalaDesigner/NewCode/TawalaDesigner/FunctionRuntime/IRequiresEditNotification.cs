// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Functions.Runtime
{
    public interface IRequiresEditNotification
    {
        void BeginEdit();
        void EndEdit();
    }
}