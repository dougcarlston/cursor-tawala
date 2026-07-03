// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Function.Controls.Properties;
using Tawala.Functions.Runtime;
using Tawala.Functions.ViewPresenter;
using Tawala.Projects;
using Tawala.ProjectUI;
using Form=System.Windows.Forms.Form;

namespace Tawala.Functions.Controls
{
    public partial class ConfigureFunctionDialog : Form, IConfigureFunctionView, IConfigureFunctionControl
    {
        private static ConfigureFunctionDialog dialog;
        private readonly Size defaultSize = new Size(796, 496);

        private IFunction editFunctionInstance, originalFunctionInstance;
        private IConfigureFunctionPresenter presenter;

        private ConfigureFunctionDialog()
        {
            InitializeComponent();
            presenter = new ConfigureFunctionPresenter(this);
        }

        public static bool Exists
        {
            get
            {
                return dialog != null;
            }
        }

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

        public static IConfigureFunctionPresenter Presenter
        {
            get
            {
                return Current.presenter;
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return defaultSize;
            }
        }

        public override Size MinimumSize
        {
            get
            {
                return defaultSize;
            }
        }

        #region IConfigureFunctionControl Members

        public void EditFunction(IFunction function)
        {
            configureFunctionInfo.SetFunction(function);
            ControlManager.BeginSession(function, this);
            configureParametersLayoutPanel.CreateParameterControls();
            configureParametersLayoutPanel.PerformLayout();
			if (configureParametersLayoutPanel.Controls.Count > 0)
			{
				configureParametersLayoutPanel.Controls[0].Focus();
			}
			else
			{
				configureFunctionInfo.SetNoParameters();
			}
        }

        public void SetCurrentParameterInfo(IParameterInfo info, Dictionary<string, string> replacements)
        {
            configureFunctionInfo.SetCurrentParameter(info, replacements);
        }

        public void HookButton(string suffix, EventHandler clickHandler)
        {
            configureFunctionButtons.HookButton(suffix, clickHandler);
        }

        #endregion

        #region IConfigureFunctionView Members

        public void SetFunction(IFunction instance, bool isNewInstance)
        {
            originalFunctionInstance = instance;
            editFunctionInstance = isNewInstance ? instance : createEditInstance(instance);

            Form designer = null;

            if (Application.OpenForms.Count != 0) // Just to prevent tests from crashing
            {
                designer = Application.OpenForms[0];
                Owner = designer;
            }

            EditFunction(editFunctionInstance);

            PerformLayout();

            if (designer != null)
            {
                Left = designer.Left + (designer.Width - Width)/2;
                Top = designer.Top + (designer.Height - Height)/2;
            }

            Show();
        }

        #endregion

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            cleanupDialog();
            base.OnFormClosed(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            cleanupDialog();
            base.OnHandleDestroyed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            Icon = Resources.Function;
            configureFunctionButtons.OKClick += buttonOK_Click;
            configureFunctionButtons.CancelClick += buttonCancel_Click;
            base.OnLoad(e);
            FieldsPalette.ConfigureFunctionActive = true;
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

        private void cleanupDialog()
        {
            ControlManager.EndSession();
            FieldsPalette.ConfigureFunctionActive = false;
            configureFunctionButtons.OKClick -= buttonOK_Click;
            configureFunctionButtons.CancelClick -= buttonCancel_Click;
            Owner = null;
            dialog = null;
            presenter = null;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            ValidateChildren();
            ControlManager.CommitPendingChanges();
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
    }
}