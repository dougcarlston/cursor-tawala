// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Projects;

namespace Tawala.Functions.Controls
{
    internal static class ParameterUtils
    {
        public static IPaletteField FieldFromDataObject(IDataObject dataObject)
        {
            string[] dataFormats = dataObject.GetFormats();

            foreach (string dataFormat in dataFormats)
            {
                var field = dataObject.GetData(dataFormat) as IPaletteField;

                if (field != null)
                {
                    return field;
                }
            }

            return null;
        }
    }
}