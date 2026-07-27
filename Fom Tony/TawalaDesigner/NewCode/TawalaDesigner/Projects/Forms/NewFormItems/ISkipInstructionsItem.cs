using System;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Projects;
using Tawala.Projects.Forms.FormItemContents;

namespace Tawala.Projects.Forms.NewFormItems
{
	public interface ISkipInstructionsItem : IFormItem
	{
		IFormItemContents Contents { get; set; }
		string GetSummary();
		Process Instructions { get; set; }
	}
}
