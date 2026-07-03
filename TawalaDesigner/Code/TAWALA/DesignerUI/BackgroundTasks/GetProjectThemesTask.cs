// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using Tawala.Common;

namespace Tawala.DesignerUI
{
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // !!! PLEASE See Common.BackgroundTaskQueue.Task for IMPORTANT
    // !!! info and restrictions on methods in this class
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    internal class GetProjectThemesTask : BackgroundTaskQueue.Task
    {
        public override void DoWorkOnBackgroundThread(BackgroundTaskQueue btq, DoWorkEventArgs e)
        {
            XmlDocument xml = Config.GetProjectThemesFromServer();
            xml.Save(Path.Combine(Config.LocalApplicationData, "ProjectThemes.xml"));
        }

        public override void RunWorkerCompletedOk(BackgroundTaskQueue btq, Object result)
        {
            // Nothing to do - all work done in background thread
        }
    }
}