// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Forms;

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

        public static void UpdateFieldsPaletteChoices(ConditionListControl listControl)
        {
            var mcItemCollection = new Collection<IMcqItem>();

            if (listControl != null)
            {

                foreach (ConditionControl group in listControl.Controls["flowLayout"].Controls)
                {
                    if (group.TextBoxField.Tag is IMcqItem)
                    {
                        mcItemCollection.Add(group.TextBoxField.Tag as IMcqItem);
                    }
                    else if (group.TextBoxField.Tag is RecordField)
                    {
                        var recordField = group.TextBoxField.Tag as RecordField;

                        if (recordField.ReferenceField is IMcqItem)
                        {
                            mcItemCollection.Add(recordField.ReferenceField as IMcqItem);
                        }
                    }
                    else if (group.TextBoxField.Tag is RecordSetField)
                    {
                        var recordSetField = group.TextBoxField.Tag as RecordSetField;

                        if (recordSetField.ReferenceField is IMcqItem)
                        {
                            mcItemCollection.Add(recordSetField.ReferenceField as IMcqItem);
                        }
                    }
                }
            }

            var mcItems = new IMcqItem[mcItemCollection.Count];
            mcItemCollection.CopyTo(mcItems, 0);
            Project.Events.RaiseMCItemSelectedEvent(new MCItemEventArgs(mcItems));
        }
    }
}