// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Functions.Runtime;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Processes;

namespace Tawala.Projects.Forms
{
	public interface IForm : IComponent, IFunctionParameterXml
	{
		FormItemList ItemList { get; set; }
		FieldList GetAllFields();
		
		IProcess ConnectedProcess { get; set; }
		IProcess ConnectedPreProcess { get; set; }
		
		IFormItem GetFormItem(string formItemLabel);
		IFormItem GetFormItem(int formItemId);
		string GetDefaultLabel(IFormItem formItem);
		Process GetSkipInstructions(ProcessStatement statement);
		bool IsDataSource { get; }
		string DataSourceName { get; set; }
		bool StartingPoint { get; set; }
		FieldList GetFormItemFields();
		FieldList GetFields();
		FieldList GetMCFields();
		FieldList GetFormItemFieldsAndRecordVariables();
		SkipToDestinationList SkipToDestinations { get; }
		bool DataEntryOnly { get; set; }
		ISkipInstructionsItem ActiveSkipToItem { get; set; }
		void ResolveProcessReferences();
	}
}
