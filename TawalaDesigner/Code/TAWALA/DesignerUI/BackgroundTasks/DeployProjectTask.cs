// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Diagnostics;
using Tawala.Common;
using Tawala.Projects;

namespace Tawala.DesignerUI
{
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // !!! PLEASE See Common.BackgroundTaskQueue.Task for IMPORTANT
    // !!! info and restrictions on methods in this class
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    internal class DeployProjectTask : BackgroundTaskQueue.Task
    {
        #region Delegates

        public delegate void AuthenticationErrorHandler(object sender);

        public delegate void ExceptionErrorHandler(object sender, Exception e);

        public delegate void GeneralErrorHandler(object sender, string msg);

        public delegate void SuccessHandler(object sender, DeploymentResponse.StartingPointList startingPoints);

        #endregion

        public event SuccessHandler Success;
        public event AuthenticationErrorHandler AuthenticationError;
        public event GeneralErrorHandler GeneralError;
        public event ExceptionErrorHandler ExceptionError;

        public override void DoWorkOnBackgroundThread(BackgroundTaskQueue btq, DoWorkEventArgs e)
        {
#if DEBUG
            DateTime before = DateTime.Now;

            try
            {
#endif
                var transceiver = new XMLTransceiver(Config.ClientURL);

                // send up our XML Project data
                transceiver.Transmit(Project.Current.ToXmlForUpload(GlobalSettings.CredentialsElement()));

                // get back the result
                e.Result = transceiver.Receive();
#if DEBUG
            }
            finally
            {
                double seconds = (DateTime.Now.Ticks - before.Ticks)/10000000.0;
                Debug.WriteLine(string.Format("DeployProjectTask background thread finished after {0} seconds", seconds));
            }
#endif
        }

        public override void RunWorkerCompletedOk(BackgroundTaskQueue btq, Object result)
        {
            var requestResult = result as string;
            var response = new DeploymentResponse(requestResult);

            if (response.Successful)
            {
                if (Success != null)
                {
                    Success(this, response.StartingPoints);
                }
            }
            else
            {
                // if login credentials weren't accepted
                if (response.ErrorID == "auth.failed")
                {
                    if (AuthenticationError != null)
                    {
                        AuthenticationError(this);
                    }
                }
                else
                {
                    if (GeneralError != null)
                    {
                        GeneralError(this, response.ErrorMessage);
                    }
                }
            }
        }

        public override void RunWorkerCompletedError(BackgroundTaskQueue btq, Exception exception)
        {
            if (ExceptionError != null)
            {
                ExceptionError(this, exception);
            }
        }
    }
}