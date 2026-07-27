// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.Interfaces
{
    public interface IBackgroundTask
    {
        string Description { get; }
        void PerformTaskOnBackgroundThread();
        void TaskCompleted();
    }
}
