using System;
using System.Collections.Generic;
using System.Text;

using Tawala.Projects.Forms.FormItemContents;

namespace Tawala.Projects.Forms.NewFormItems
{
	public interface IChoice : IFormItemContents, IPaletteField
	{
		string ContentsXhtml(IFormItem formItem);
		new string Text { get; set; }
		string ToXml(string label);
		string ChoiceLabel { get; set; }
	}
}
