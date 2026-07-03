using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.Projects
{
	public interface IAnyField
	{
		int Id { get; }
		string QualifiedFieldName { get; }
	}
}
