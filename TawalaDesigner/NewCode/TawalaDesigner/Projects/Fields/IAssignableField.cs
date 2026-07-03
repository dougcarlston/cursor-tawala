using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.Projects
{
	public interface IAssignableField : IPaletteField, IOperatorDataSource
	{
		string AssignmentName
		{
			get;
		}
	}
}
