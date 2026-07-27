using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Functions.Runtime;

namespace Tawala.Functions.ViewPresenter
{
    public interface IConfigureFunctionView
    {
        void SetFunction(IFunction functionObject, bool isNewInstance);
    }
}
