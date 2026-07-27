using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.Projects.Processes
{
	public interface IProcess : IComponent
	{
		ProcessLineList Lines { get; set; }

		VariableList Variables { get; set; }
	}
}
