using System;
using System.Collections.Generic;
using System.Text;
using Tawala.XmlSupport;

namespace Tawala.FunctionConfiguration
{
	public interface IConfiguredFunction
	{
		/// <summary>
		/// Gets an ID that uniquely identifies an instance of a configured function.
		/// </summary>
		int InstanceId { get; }

		/// <summary>
		/// Returns a string representing the value of the specified parameter.
		/// </summary>
		string GetParameterValueString(string parameterId);

		/// <summary>
		/// Returns an XML string representing the configured function.
		/// </summary>
		string ToXml();
	}
}
