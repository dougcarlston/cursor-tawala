using System;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Projects;
using Tawala.Projects.Forms.FormItemContents;

namespace Tawala.Projects.Forms.NewFormItems
{
	public interface ITextItem : IFormItem
	{
		IFormItemContents Contents { get; set; }
		string ToXml(string label);
	}
}
