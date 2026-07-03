using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Proj;

namespace Tawala.FunctionConfiguration
{
	public interface IParameterControl
	{
		IParameterValue Value { get; set; }
	}
}
