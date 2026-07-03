using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Projects;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Functions.Runtime;

namespace Tawala.Projects.Forms.NewFormItems
{
	public interface IMcqItem : IFormItem, IOperatorDataSource, IPaletteField
	{
		IFormItemContents Contents { get; set; }
		bool SelectOnlyOne { get; set; }
		bool RequireAtLeastOne { get; set; }
		int ColumnCount { get; set; }
		string ToXml(string label);
		int ChoiceSourceType { get; set; }
		Question Question { get; }
		IFormItemContents QuestionContents { get; }
		IChoiceList Choices { get; }
		IFormItemContents ChoiceContents { set; }
		IFunction DataSourceFunction { get; }
		string ChoicesXhtml { get; }
	}
}
