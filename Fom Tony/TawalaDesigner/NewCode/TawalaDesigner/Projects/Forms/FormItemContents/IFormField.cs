using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.Projects.Forms.FormItemContents
{
	public interface IFormField : IPaletteField
	{
		string AlternateLabel { get; set; }
		string ToString();
	}
}
