using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Interfaces;
using Tawala.FormDesigner.FormItemOptions;

namespace Tawala.FormDesigner
{
	public interface ITawalaFormDesigner : IProjectComponentDesigner
	{
		IFormView CurrentFormView { get; set; }
	}
}
