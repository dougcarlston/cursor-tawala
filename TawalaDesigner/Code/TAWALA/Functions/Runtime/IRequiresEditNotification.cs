// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Functions.Runtime
{
    public interface IRequiresEditNotification
    {
        void BeginEdit();
        void EndEdit();
    }
}