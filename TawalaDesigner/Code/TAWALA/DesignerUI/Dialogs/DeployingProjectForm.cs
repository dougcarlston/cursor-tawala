// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Common;
using Tawala.DesignerUI.Properties;

namespace Tawala.DesignerUI
{
    public partial class DeployingProjectForm : Form
    {
        private BackgroundTaskQueue queue;
        private DeployProjectTask task;

        public DeployingProjectForm(BackgroundTaskQueue queue)
        {
            InitializeComponent();
            this.queue = queue;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            task = new DeployProjectTask();
            task.Success += new DeployProjectTask.SuccessHandler(task_Success);
            task.AuthenticationError += new DeployProjectTask.AuthenticationErrorHandler(task_AuthenticationError);
            task.GeneralError += new DeployProjectTask.GeneralErrorHandler(task_GeneralError);
            task.ExceptionError += new DeployProjectTask.ExceptionErrorHandler(task_ExceptionError);
            queue.Add(task);
        }

        private void task_Success(object sender, DeploymentResponse.StartingPointList startingPoints)
        {
            var dpForm = new DeployedProjectForm();

            dpForm.StartingPoingList = startingPoints;

            dpForm.LabelSuccess.Text = Resources.DeploySuccess;
            dpForm.LabelMessage.Text = Resources.DeploySuccess2;

            dpForm.ShowDialog(this);

            //// SUCCESSFULLY DEPLOYED project so queue a task to update deployment info
            //// mostly because we know we must have good credentials or if the user changes
            //// projects or starts a new project this info will be up-to-date
            queue.Add(new GetDeploymentInfoTask());

            Close();
        }

        private void task_GeneralError(object sender, string msg)
        {
            var dpForm = new DeployedProjectForm();

            dpForm.LabelSuccess.Text = Resources.DeployFailure;
            dpForm.LabelMessage.Text = msg;

            dpForm.ShowDialog(this);

            Close();
        }

        private void task_AuthenticationError(object sender)
        {
            // prompt the user to enter them
            if (GlobalSettings.PromptForCredentials(this) == DialogResult.OK)
            {
                queue.Add(sender as DeployProjectTask); // retry by requeueing
            }
            else
            {
                Close();
            }
        }

        private void task_ExceptionError(object sender, Exception e)
        {
            // Generally exceptions are caused by web request failures

            var failureForm = new WebRequestFailure();
            failureForm.ErrorMessage = Resources.WebRequestExceptionThrown + "\r\n" + e.Message;

            if (failureForm.ShowDialog(this) == DialogResult.Retry)
            {
                queue.Add(sender as DeployProjectTask);
            }
            else
            {
                Close();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (task != null)
            {
                task.Success -= new DeployProjectTask.SuccessHandler(task_Success);
                task.AuthenticationError -= new DeployProjectTask.AuthenticationErrorHandler(task_AuthenticationError);
                task.GeneralError -= new DeployProjectTask.GeneralErrorHandler(task_GeneralError);
                task.ExceptionError -= new DeployProjectTask.ExceptionErrorHandler(task_ExceptionError);
                task = null;
            }

            base.OnFormClosing(e);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // The background task events will be ignored because only this dialog now handles 
            // the deploying events.  But the task doesn't really get canceled at this point.
            Close();
        }
    }
}