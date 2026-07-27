// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Functions.Runtime;

namespace Tawala.Functions.Controls
{
    /// <summary>
    /// FunctionFieldWrapper derived classes which have an inherited StringConverter.
    /// (FunctionBlank, FunctionMCItem, FunctionContentsFields)
    /// </summary>
    public class FieldTypeConverterBinder : ParameterBinder
    {
        protected FieldTypeConverterBinder(IFunction instance, IParameterInfo info)
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