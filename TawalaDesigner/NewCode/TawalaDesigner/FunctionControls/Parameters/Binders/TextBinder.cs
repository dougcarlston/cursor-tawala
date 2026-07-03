// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Functions.Runtime;

namespace Tawala.Functions.Controls
{
    public class TextBinder : ParameterBinder
    {
        protected TextBinder(IFunction instance, IParameterInfo info)
            : base(instance, info)
        {
        }

        protected override void Bind(IParameterControl c)
        {
            new EditBinding(c, "Text", this);
        }
    }
}