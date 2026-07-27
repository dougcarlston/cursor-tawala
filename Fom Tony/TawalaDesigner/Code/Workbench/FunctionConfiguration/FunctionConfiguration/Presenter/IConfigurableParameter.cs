using System;
using System.Windows.Forms;
using Tawala.Proj;

namespace Tawala.FunctionConfiguration
{
	public interface IConfigurableParameter
	{
		/// <summary>
		/// Gets the parameter's identifier.
		/// </summary>
		string Id { get; }

		/// <summary>
		/// Gets the parameter's Name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the parameter's type (e.g. "tawala-mcq", "enumeration", etc.).
		/// </summary>
		string Type { get; }

		/// <summary>
		/// Gets a boolean value indicating whether the parameter is required.
		/// </summary>
		bool Required { get; }

		/// <summary>
		/// Gets the parameter's description.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Gets or sets the parameter's Value.
		/// </summary>
		IParameterValue Value { get; set; }

		/// <summary>
		/// Returns a control suitable for entering data into the parameter.
		/// </summary>
		Control DataEntryControl { get; }

		/// <summary>
		/// Returns an XML string representing the configured parameter.
		/// </summary>
		string ToXml();
	}
}
