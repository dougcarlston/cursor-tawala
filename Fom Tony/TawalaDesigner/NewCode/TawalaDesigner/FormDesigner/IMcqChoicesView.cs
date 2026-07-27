using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.FormDesigner
{
	public interface IMcqChoicesView
	{
		string ChoicesText { get; }
		string ChoicesHtml { get; set; }
		void InsertField(string fieldName);
		void SetChoiceSource(int choiceSourceType);
		void EnableChoiceConfiguration(bool enable);
	}
}
