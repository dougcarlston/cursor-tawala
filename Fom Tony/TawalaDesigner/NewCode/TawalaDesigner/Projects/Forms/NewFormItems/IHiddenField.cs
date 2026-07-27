using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.Projects.Forms.NewFormItems
{
	public interface IHiddenField : IFormItem, IPaletteField, IOperatorDataSource
	{
		string Name { get; set; }
	}
}
