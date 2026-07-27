// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Forms.NewFormItems;

namespace Tawala.Functions.Controls
{
    internal class MCItemParameterTextBox : FieldParameterTextBox<FunctionMCItem>
    {
        public override bool IsAcceptedData(object o)
        {
            if (o is IDataObject)
            {
                IPaletteField field = ParameterUtils.FieldFromDataObject(o as IDataObject);
                if (field is IMcqItem)
                {
                    return true;
                }
                if (field is RecordField && ((RecordField)field).ReferenceField is IMcqItem)
                {
                    return true;
                }
            }

            return FunctionMCItem.Parse(o, ParameterControlManager.LookupParameterInfo(this)) != null;
        }

        public override void AcceptData(object o)
        {
            if (o is IDataObject)
            {
                o = ParameterUtils.FieldFromDataObject(o as IDataObject);
            }
            CustomDataSource = FunctionMCItem.Parse(o, ParameterControlManager.LookupParameterInfo(this));
        }
    }
}