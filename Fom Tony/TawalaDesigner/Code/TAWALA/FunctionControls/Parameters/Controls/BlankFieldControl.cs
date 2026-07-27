// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;

namespace Tawala.Functions.Controls
{
    public class BlankFieldControl : FieldTextBaseControl<FunctionBlank>
    {
        public override bool IsAcceptedData(IPaletteField field)
        {
            return FunctionBlank.Parse(field) != null;
        }

        public override void AcceptData(IPaletteField field)
        {
            Value = FunctionBlank.Parse(field);
        }
    }
}