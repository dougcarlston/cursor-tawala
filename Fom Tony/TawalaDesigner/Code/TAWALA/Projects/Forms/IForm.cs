// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Functions.Runtime;
using Tawala.Projects.Components;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;

namespace Tawala.Projects
{
    public interface IForm : IProjectComponent, IFunctionParameterValue
    {
        FormItemList ItemList { get; set; }

        IProcess ConnectedPostProcess { get; set; }
        IProcess ConnectedPreProcess { get; set; }
        bool IsDataSource { get; }
        string DataSourceName { get; set; }
        bool StartingPoint { get; set; }
        SkipToDestinationList SkipToDestinations { get; }
        bool DataEntryOnly { get; set; }
        SkipInstructionsItem ActiveSkipToItem { get; set; }
        FieldList GetAllFields();

        IFormItem GetFormItem(string formItemLabel);
        IFormItem GetFormItem(int formItemId);
        string GetDefaultLabel(IFormItem formItem);
        Process GetSkipInstructions(ProcessStatement statement);
        FieldList GetFormItemFields();
        FieldList GetFields();
        FieldList GetMCFields();
        FieldList GetFormItemFieldsAndRecordVariables();
        void ResolveProcessReferences();

        bool BlockBackButton { get; set; }
    }
}