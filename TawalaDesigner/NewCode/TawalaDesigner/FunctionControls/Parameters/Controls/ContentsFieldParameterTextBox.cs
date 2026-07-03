// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Projects;

namespace Tawala.Functions.Controls
{
    internal class ContentsFieldParameterTextBox : FieldParameterTextBox<FunctionContentsField>
    {
        public override bool IsAcceptedData(object o)
        {
            if (o is IDataObject)
            {
                IPaletteField field = ParameterUtils.FieldFromDataObject(o as IDataObject);

                if (FunctionContentsField.AcceptedType(field))
                {
                    return true;
                }
            }

            return FunctionContentsField.Parse(o) != null;
        }

        public override void AcceptData(object o)
        {
            if (o is IDataObject)
            {
                o = ParameterUtils.FieldFromDataObject(o as IDataObject);
            }
            CustomDataSource = FunctionContentsField.Parse(o);
        }
    }
}