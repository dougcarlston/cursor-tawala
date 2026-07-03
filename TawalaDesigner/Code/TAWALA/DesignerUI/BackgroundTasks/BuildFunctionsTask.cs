// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using Tawala.Common;

namespace Tawala.DesignerUI
{
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // !!! PLEASE See Common.BackgroundTaskQueue.Task for IMPORTANT
    // !!! info and restrictions on methods in this class
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public class BuildFunctionsTask : BackgroundTaskQueue.Task
    {
        public override void DoWorkOnBackgroundThread(BackgroundTaskQueue btq, DoWorkEventArgs e)
        {
            e.Result = true;
            FunctionRepositoryInfo.Build();
            e.Result = FunctionRepositoryInfo.IsLoaded;
        }

        public override void RunWorkerCompletedOk(BackgroundTaskQueue btq, Object result)
        {
        }

        public override void RunWorkerCompletedError(BackgroundTaskQueue btq, Exception ignore)
        {
        }
    }
}