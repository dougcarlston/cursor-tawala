using System;
using System.Collections.Generic;
using System.Text;

namespace Tawala.FunctionConfiguration
{
	public interface IIdentityControl
	{
		string Id { get; set; }
		object Value { get; }
	}
}
