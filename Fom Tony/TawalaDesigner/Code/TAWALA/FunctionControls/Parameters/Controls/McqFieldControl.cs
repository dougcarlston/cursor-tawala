// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Forms;

namespace Tawala.Functions.Controls
{
    public class McqFieldControl : FieldTextBaseControl<FunctionMCItem>
    {
        public override bool IsAcceptedData(IPaletteField field)
        {
            if (field is IMcqItem)
            {
                return true;
            }
            if (field is RecordField && ((RecordField)field).ReferenceField is IMcqItem)
            {
                return true;
            }

            return false;
        }

        public override void AcceptData(IPaletteField field)
        {
            Value = FunctionMCItem.Parse(field, ControlManager.LookupParameterInfo(this));
        }
    }
}