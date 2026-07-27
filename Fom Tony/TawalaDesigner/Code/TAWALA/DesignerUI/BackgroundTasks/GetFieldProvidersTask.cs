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
    public class GetFieldProvidersTask : BackgroundTaskQueue.Task
    {
        public override void DoWorkOnBackgroundThread(BackgroundTaskQueue btq, DoWorkEventArgs e)
        {
            e.Result = true;
            e.Result = FieldProviders.QueryServerAndSaveToFile(GlobalSettings.CredentialsElement());
        }

        public override void RunWorkerCompletedOk(BackgroundTaskQueue btq, Object result)
        {
            FieldProviders.LoadDataSourcesFromFile();
            Project.Events.RaiseFieldProvidersChangedEvent(EventArgs.Empty);
        }

        public override void RunWorkerCompletedError(BackgroundTaskQueue btq, Exception ignore)
        {
            FieldProviders.LoadDataSourcesFromFile(); // will load old if it exists
            Project.Events.RaiseFieldProvidersChangedEvent(EventArgs.Empty);
        }
    }
}