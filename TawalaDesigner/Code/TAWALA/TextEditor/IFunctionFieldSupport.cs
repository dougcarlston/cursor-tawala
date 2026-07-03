using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.TextEditor
{
	public interface IFunctionFieldSupport
	{
		event EventHandler<FunctionFieldEventArgs> FunctionFieldDoubleClicked;
		void ChangeSelectedFunctionFieldIdToUniqueId(int oldIdForSanityCheck, int newId);
		void UpdateFunctionFieldByUniqueId(int oldId, int newId, string displayText);
		void InsertFunctionField(int newFunctionId, string displayText);
		int SelectedFunctionFieldInstanceId
		{
			get;
		}
	}
}
