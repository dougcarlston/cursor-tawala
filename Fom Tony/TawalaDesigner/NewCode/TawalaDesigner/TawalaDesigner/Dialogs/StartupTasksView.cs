// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Windows.Forms;

namespace TawalaDesigner.Dialogs
{
    public partial class StartupTasksView : Form, IStartupTasksView
    {
        private StartupTasksPresenter presenter;

        public StartupTasksView()
        {
            InitializeComponent();

            presenter = new StartupTasksPresenter(this);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            presenter.DoAsynchronousTasks();
        }

        #region IStartupTasksView Members

        public void SetTaskDescription(string text)
        {
            labelActivity.Text = text;
            labelActivity.Update();
        }

        public void TasksCompleted()
        {
            Close();
        }

        #endregion

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            presenter = null;
        }
    }
}
