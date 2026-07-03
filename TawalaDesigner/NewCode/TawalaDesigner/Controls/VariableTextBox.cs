// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Projects;

namespace Tawala.Controls
{
    public class VariableTextBox : ProcessTextBox
    {
        /// <summary>
        /// Indicates whether this text box accepts drag/drop of the specified data.
        /// </summary>
        public override bool AcceptsDropOf(IDataObject data)
        {
            return data.GetDataPresent(typeof(IAssignableField));

            //if (data.GetDataPresent(typeof(Variable)))
            //{
            //    return true;
            //}

            //if (data.GetDataPresent(typeof(HiddenField)))
            //{
            //    return true;
            //}

            //if (data.GetDataPresent(typeof(RecordField)))
            //{
            //    return true;
            //}

            //if (data.GetDataPresent(typeof(IPaletteField)))
            //{
            //    return true;
            //}

            //return false;
        }

        public IAssignableField DraggedField(IDataObject data)
        {
            IAssignableField field = null;

            if (data.GetDataPresent(typeof(IAssignableField)))
            {
                field = (IAssignableField)data.GetData(typeof(IAssignableField));
            }

            //if (data.GetDataPresent(typeof(Variable)))
            //{
            //    field = (IField)data.GetData(typeof(Variable));
            //}
            //else if (data.GetDataPresent(typeof(HiddenField)))
            //{
            //    field = (IField)data.GetData(typeof(HiddenField));
            //}
            //else if (data.GetDataPresent(typeof(RecordField)))
            //{
            //    field = (IField)data.GetData(typeof(RecordField));
            //}

            //if (data.GetDataPresent(typeof(IPaletteField)))
            //{
            //    field = (IField)data.GetData(typeof(IPaletteField));
            //}

            return field;
        }
    }
}