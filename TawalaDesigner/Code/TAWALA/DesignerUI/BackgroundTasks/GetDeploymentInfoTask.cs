// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using Tawala.Common;
using Tawala.Projects;

namespace Tawala.DesignerUI
{
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // !!! PLEASE See Common.BackgroundTaskQueue.Task for IMPORTANT
    // !!! info and restrictions on methods in this class
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public class GetDeploymentInfoTask : BackgroundTaskQueue.Task
    {
        public override void DoWorkOnBackgroundThread(BackgroundTaskQueue btq, DoWorkEventArgs e)
        {
            e.Result = DeploymentInfo.Error.General;
            e.Result = DeploymentInfo.QueryServer(GlobalSettings.CredentialsElement());
        }

        public override void RunWorkerCompletedOk(BackgroundTaskQueue btq, Object result)
        {
            Project.Events.RaiseDeploymentInfoChangedEvent(EventArgs.Empty);
        }

        public override void RunWorkerCompletedError(BackgroundTaskQueue btq, Exception ignore)
        {
            // **** WRINKLE ****
            // Still raise this event -- the DeploymentInfo class caches data so if there is an error
            // it doesn't really change but the wrinkle is that the current project may have changed
            // something DeploymentInfo knows nothing about but ProjectNameSource certainly cares about.

            Project.Events.RaiseDeploymentInfoChangedEvent(EventArgs.Empty);
        }
    }
}