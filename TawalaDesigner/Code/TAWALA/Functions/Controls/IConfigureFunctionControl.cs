// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using Tawala.Functions.Runtime;

namespace Tawala.Functions.Controls
{
    public interface IConfigureFunctionControl
    {
        void EditFunction(IFunction function);
        void SetCurrentParameterInfo(IParameterInfo info, Dictionary<string, string> replacements);
        void HookButton(string suffix, EventHandler clickHandler);
    }
}