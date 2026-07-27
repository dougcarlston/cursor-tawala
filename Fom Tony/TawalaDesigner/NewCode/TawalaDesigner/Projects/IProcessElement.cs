using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.Projects
{
	public interface IProcessElement : IRecursiveEnumerable
	{
		Boolean IsValid { get; }
	}
}
