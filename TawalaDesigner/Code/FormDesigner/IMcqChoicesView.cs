using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.FormDesigner
{
	public interface IMcqChoicesView
	{
		string ChoiceStrings { get; set; }
		void InsertFieldString(string fieldString);
		void SetChoiceSource(int choiceSourceIndex);
		void EnableChoiceConfiguration(bool enable);
	}
}
