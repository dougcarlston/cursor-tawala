// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Functions.Runtime;

namespace Tawala.Functions.Controls
{
    public class CompoundExpressionBinder : ParameterBinder
    {
        protected CompoundExpressionBinder(IFunction instance, IParameterInfo info)
            : base(instance, info)
        {
        }

        protected override void Bind(IParameterControl c)
        {
            new ViewBinding(c, "Text", this);
            new EditBinding(c, "CustomDataSource", this);
        }
    }
}