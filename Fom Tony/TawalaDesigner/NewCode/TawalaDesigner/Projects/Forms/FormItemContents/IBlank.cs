using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.Projects.Forms.FormItemContents
{
	public interface IBlank : IFormField
	{
		bool Required { get; set; }
		int Length { get; set; }
	}
}
