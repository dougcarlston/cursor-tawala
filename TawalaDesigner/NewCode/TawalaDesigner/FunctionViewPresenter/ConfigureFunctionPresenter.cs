using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Functions.Runtime;

namespace Tawala.Functions.ViewPresenter
{
    public class ConfigureFunctionPresenter : IConfigureFunctionPresenter
    {
        private IConfigureFunctionView view;
        private IFunction functionObject;
        private EventHandler<FunctionConfiguredEventArgs> notifyOwnerDelegate;

        public ConfigureFunctionPresenter(IConfigureFunctionView view)
        {
            this.view = view;
        }

        #region IConfigureFunctionPresenter Members

        public void CreateFunction(IFunctionInfo info, EventHandler<FunctionConfiguredEventArgs> notifyOwner)
        {
            Control notify = notifyOwner.Target as Control;

            if (notify != null)
            {
                notify.HandleDestroyed += new EventHandler(notify_HandleDestroyed);
            }

            notifyOwnerDelegate = notifyOwner;

            functionObject = null;

            if (info != null)
            {
                functionObject = info.CreateInstance();
                view.SetFunction(functionObject, true);
            }
        }

        public void EditFunction(IFunction instance, EventHandler<FunctionConfiguredEventArgs> notifyOwner)
        {
            notifyOwnerDelegate = notifyOwner;
            functionObject = instance;

            view.SetFunction(functionObject, false);
        }

        public void ConfigurationCompleted(IFunction oldFunction, IFunction newFunction, bool success)
        {
            if (notifyOwnerDelegate != null)
            {
                notifyOwnerDelegate(this, new FunctionConfiguredEventArgs(oldFunction, newFunction, !success));
                notifyOwnerDelegate = null;
            }
        }

        public IFunction Function
        {
            get { return functionObject; }
        }

        #endregion

        private void notify_HandleDestroyed(object sender, EventArgs e)
        {
            notifyOwnerDelegate = null;
        }
    }

    public class FunctionConfiguredEventArgs : EventArgs
    {
        private IFunction functionOld;
		private IFunction functionNew;
		private bool userCanceled = false;

        public FunctionConfiguredEventArgs(IFunction functionOld, IFunction functionNew, bool canceled)
        {
			this.functionOld = functionOld;
			this.functionNew = canceled ? functionOld : functionNew;
			this.userCanceled = canceled;
		}

		/// <summary>If new function instance then this is the same as EditedInstance </summary>
		public IFunction OriginalInstance
		{
			get { return functionOld; }
		}

		/// <summary>If new function instance then this is the same as OriginalInstance </summary>
		public IFunction EditedInstance
        {
			get { return functionNew; }
        }

        public bool Canceled
        {
            get { return userCanceled; }
        }
    }

}
