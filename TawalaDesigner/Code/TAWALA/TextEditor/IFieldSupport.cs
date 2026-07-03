using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.TextEditor
{
	public interface IFieldSupport
	{
		bool IsFieldIdInUse(int id);
		void AddField(int id, string name);
		void UpdateFieldNames(Dictionary<int, string> fieldMap);
	}
}
