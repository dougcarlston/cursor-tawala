using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Tawala.Functions.Runtime;

namespace Tawala.Functions.ViewPresenter
{
    public interface IConfigureFunctionPresenter
    {
        void CreateFunction(IFunctionInfo info, EventHandler<FunctionConfiguredEventArgs> notifyOwner);
        void EditFunction(IFunction instance, EventHandler<FunctionConfiguredEventArgs> notifyOwner);
        void ConfigurationCompleted(IFunction oldInstance, IFunction newInstance, bool success);

        IFunction Function
        {
            get;
        }
    }
}
