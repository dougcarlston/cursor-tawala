// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;

namespace Tawala.Functions.Controls
{
    internal class BlankParameterTextBox : FieldParameterTextBox<FunctionBlank>
    {
        public override bool IsAcceptedData(object o)
        {
            if (o is IDataObject)
            {
                IPaletteField field = ParameterUtils.FieldFromDataObject(o as IDataObject);
                if (field is IHiddenField || field is IBlank)
                {
                    return true;
                }
                var rf = field as RecordField;

                if (rf != null && (rf.ReferenceField is IHiddenField || rf.ReferenceField is IBlank))
                {
                    return true;
                }
            }

            return FunctionBlank.Parse(o) != null;
        }

        public override void AcceptData(object o)
        {
            if (o is IDataObject)
            {
                o = ParameterUtils.FieldFromDataObject(o as IDataObject);
            }
            CustomDataSource = FunctionBlank.Parse(o);
        }
    }
}