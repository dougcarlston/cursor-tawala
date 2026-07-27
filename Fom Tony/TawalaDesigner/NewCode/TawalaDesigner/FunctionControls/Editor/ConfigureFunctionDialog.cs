// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Functions.Runtime;
using Tawala.Functions.ViewPresenter;
using Tawala.Projects.Function;

namespace Tawala.Functions.Controls
{
    public partial class ConfigureFunctionDialog : Form, IConfigureFunctionView
    {
        private static ConfigureFunctionDialog dialog;
        private readonly IConfigureFunctionControl configFunctionControl;

        private IFunction editFunctionInstance, originalFunctionInstance;
        private IConfigureFunctionPresenter presenter;

        private ConfigureFunctionDialog()
        {
            InitializeComponent();

            configFunctionControl = configureFunctionControl;
            presenter = new ConfigureFunctionPresenter(this);
        }

        public static bool Exists { get { return dialog != null; } }

        public static ConfigureFunctionDialog Current
        {
            get
            {
                if (dialog == null)
                {
                    dialog = new ConfigureFunctionDialog();
                }
                return dialog;
            }
        }

        public static IConfigureFunctionPresenter Presenter { get { return Current.presenter; } }

        #region IConfigureFunctionView Members

        void IConfigureFunctionView.SetFunction(IFunction instance, bool isNewInstance)
        {
            originalFunctionInstance = instance;
            editFunctionInstance = isNewInstance ? instance : createEditInstance(instance);

            Form designer = null;

            if (Application.OpenForms.Count != 0) // Just to prevent tests from crashing
            {
                designer = Application.OpenForms[0];
                Owner = designer;
            }

            configFunctionControl.EditFunction(editFunctionInstance);

            PerformLayout();

            Show();

            StartPosition = FormStartPosition.CenterParent;

            if (designer != null)
            {
                Left = designer.Left + (designer.Width - Width)/2;
                Top = designer.Top + (designer.Height - Height)/2;
            }
        }

        #endregion

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            Application.Idle += onApplicationIdle;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            cleanupDialog();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            cleanupDialog();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            Activate();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (ActiveControl == null)
            {
                Focus();
            }
        }

        private static IFunction createEditInstance(IFunction original)
        {
            return new XmlToFunctionConverter().CloneFunction(original);
        }

        private void onApplicationIdle(object sender, EventArgs e)
        {
            if (IsHandleCreated && configFunctionControl != null)
            {
                bool okState = configFunctionControl.IsOK();
                if (buttonOK.Enabled != okState)
                {
                    buttonOK.Enabled = okState;
                }
            }
        }

        private void cleanupDialog()
        {
            Application.Idle -= onApplicationIdle;
            Owner = null;
            dialog = null;
            presenter = null;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            ParameterControlManager.CommitPendingChanges();
            presenter.ConfigurationCompleted(originalFunctionInstance, editFunctionInstance, true);
            originalFunctionInstance = editFunctionInstance = null;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            presenter.ConfigurationCompleted(originalFunctionInstance, editFunctionInstance, false);
            originalFunctionInstance = editFunctionInstance = null;
            Close();
        }

        private void panelButtons_Layout(object sender, LayoutEventArgs e)
        {
            int buttonWidth = (buttonOK.Width + buttonCancel.Width + 20)/2;
            int width = panelButtons.ClientSize.Width/2;
            int height = panelButtons.ClientSize.Height/2;
            buttonOK.Left = width - buttonWidth;
            buttonCancel.Left = buttonOK.Right + 10;
        }
    }
}