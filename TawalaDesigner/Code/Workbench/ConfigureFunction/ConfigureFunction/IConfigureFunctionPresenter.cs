using System;
using Tawala.XmlSupport;

namespace Tawala.ConfigurableFunction
{
	public interface IConfigureFunctionPresenter
	{
		/// <summary>
		/// Makes a function available for configuration by initializing the view and making it visible.
		/// </summary>
		void ConfigureFunction();

		/// <summary>
		/// Called when a parameter is selected in the view.
		/// </summary>
		void ParameterSelected(string parameterId);

		/// <summary>
		/// Called when a parameter changes in the view.
		/// </summary>
		void ParameterChanged(string parameterId, string value);

		/// <summary>
		/// Called when the OK button is clicked in the view.
		/// </summary>
		void ConfigurationCompleted();

		/// <summary>
		/// Called when the Cance button is clicked in the view.
		/// </summary>
		void ConfigurationCanceled();

		/// <summary>
		/// Returns an XML string representing the configured function.
		/// </summary>
		string ToXml();
	}
}
