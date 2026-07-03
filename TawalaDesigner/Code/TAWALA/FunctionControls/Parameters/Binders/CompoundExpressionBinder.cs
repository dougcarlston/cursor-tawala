// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Functions.Runtime;

namespace Tawala.Functions.Controls
{
    public class CompoundExpressionBinder : ParameterBinder
    {
        protected CompoundExpressionBinder(IParameterInfo info) : base(info)
        {
        }

        protected override void Bind(IParameterControl c)
        {
            new EditBinding(c, "Text", this);
 //           new EditBinding(c, "Value", this);
        }
    }
}