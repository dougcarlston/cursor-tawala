// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Functions.Runtime;
using Tawala.Projects;

namespace Tawala.Functions.Controls
{
    public class ContentFieldControl : FieldTextBaseControl<FunctionContentsField>
    {
        public override bool IsAcceptedData(IPaletteField field)
        {
            if (FunctionContentsField.AcceptedType(field))
            {
                return true;
            }

            return false;
        }

        public override void AcceptData(IPaletteField field)
        {
            field = FunctionUtil.ApplyParameterRestrictions(ControlManager.LookupParameterInfo(this), field);
            Value = FunctionContentsField.Parse(field);
        }
    }
}