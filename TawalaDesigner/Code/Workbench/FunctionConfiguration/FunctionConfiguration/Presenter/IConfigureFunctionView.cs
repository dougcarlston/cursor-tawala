using System;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.XmlSupport;
using Tawala.FunctionConfiguration;

namespace Tawala.FunctionConfiguration
{
	public interface IConfigureFunctionViewPhase2
	{
		/// <summary>
		/// Gets or sets the presenter associated with this view.
		/// </summary>
		IConfigureFunctionPresenter Presenter { get; set; }

		/// <summary>
		/// Gets or sets the name of the function being configured.
		/// </summary>
		string FunctionName { get; set; }

		/// <summary>
		/// Gets or sets the description of the function being configured.
		/// </summary>
		string FunctionDescription { get; set; }

		/// <summary>
		/// Gets or sets the parameters of the function being configured.
		/// </summary>
		Collection<IConfigurableParameter> Parameters { get; set; }

		/// <summary>
		/// Gets or sets the name of the currently-selected parameter.
		/// </summary>
		string ParameterName { get; set; }

		/// <summary>
		/// Gets or sets the description of the currently-selected parameter.
		/// </summary>
		string ParameterDescription { get; set; }

		/// <summary>
		/// Makes this view visible.
		/// </summary>
		void MakeVisible();
	}
}
