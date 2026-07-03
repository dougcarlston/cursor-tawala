using System;

namespace Tawala.ConfigurableFunction
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
		/// Gets the parameter's Value.
		/// </summary>
		string Value { get; }

		/// <summary>
		/// Returns an XML string representing the configured parameter.
		/// </summary>
		string ToXml();
	}
}
