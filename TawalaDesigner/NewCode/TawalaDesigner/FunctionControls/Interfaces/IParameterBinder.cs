// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.Functions.Controls
{
    public interface IParameterBinder
    {
        void Initialize(IParameterControl control);
        void SetLabels(Label name, Label description);
        void SetSingleLineEditLabels(Label name, Label tagLine);
    }
}