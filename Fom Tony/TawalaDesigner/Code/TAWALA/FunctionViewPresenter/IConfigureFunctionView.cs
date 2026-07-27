// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Functions.Runtime;

namespace Tawala.Functions.ViewPresenter
{
    public interface IConfigureFunctionView
    {
        void SetFunction(IFunction functionObject, bool isNewInstance);
    }
}