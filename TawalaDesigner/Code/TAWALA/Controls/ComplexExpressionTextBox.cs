// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Projects;

namespace Tawala.Controls
{
    public class ComplexExpressionTextBox : ProcessTextBox
    {
        /// <summary>
        /// Indicates whether this text box accepts drag/drop of the specified data.
        /// </summary>
        public override bool AcceptsDropOf(IDataObject data)
        {
            if (data.GetDataPresent(typeof(IPaletteField)))
            {
                return true;
            }

            return false;
        }

        public IField DraggedField(IDataObject data)
        {
            IField field = null;

            if (data.GetDataPresent(typeof(IPaletteField)))
            {
                field = (IField)data.GetData(typeof(IPaletteField));
            }

            return field;
        }

        public void Insert(IField field)
        {
            if (field != null)
            {
                SelectedText = field.FieldString;
            }
        }
    }
}