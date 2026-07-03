using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.UndoSupport
{
	public interface IMemento
	{
		string ActionText { get; }
	}
}
