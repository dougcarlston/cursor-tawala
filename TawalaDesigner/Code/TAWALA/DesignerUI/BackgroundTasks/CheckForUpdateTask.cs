// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Xml;
using Tawala.Common;

namespace Tawala.DesignerUI
{
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // !!! PLEASE See Common.BackgroundTaskQueue.Task for IMPORTANT
    // !!! info and restrictions on methods in this class
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    internal class CheckForUpdateTask : BackgroundTaskQueue.Task
    {
        private bool mandatory;
        private string url;

        public override void DoWorkOnBackgroundThread(BackgroundTaskQueue btq, DoWorkEventArgs e)
        {
            e.Result = false; // new version == false

            XmlDocument xml = Config.CheckForNewVersion();

            if (Config.IsNewerVersion(xml))
            {
                url = xml.SelectSingleNode("//url").InnerText;
                mandatory = Config.IsMandatoryUpdate(xml);
                e.Result = true; // new version == true
            }
        }

        public override void RunWorkerCompletedOk(BackgroundTaskQueue btq, Object result)
        {
            if (Convert.ToBoolean(result))
            {
                var form = new UpdateNotification(url, mandatory);
                form.ShowDialog(btq.OwnerForm);
            }
        }
    }
}