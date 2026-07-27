using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Proj;

namespace Tawala.FunctionConfiguration
{
	public class ParameterValue : IParameterValue
	{
		public static IParameterValue NULL = new NullParameterValue();

		private class NullParameterValue : ParameterValue
		{
			public override string ToString()
			{
				return "Null Parameter Value";
			}
		}
	}
}
