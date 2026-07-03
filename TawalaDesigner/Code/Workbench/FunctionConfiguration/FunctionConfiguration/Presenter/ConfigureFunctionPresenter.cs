using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Proj;
using Tawala.XmlSupport;
using Tawala.FunctionConfiguration;

namespace Tawala.FunctionConfiguration
{
	public class ConfigureFunctionPresenter : IConfigureFunctionPresenter
	{
		private IConfigureFunctionViewPhase2 view;
		private IXmlElement functionSpecificationElement;
		private IConfiguredFunction configuredFunction = ConfiguredFunction.NULL;

		public ConfigureFunctionPresenter(IConfigureFunctionViewPhase2 view, IXmlElement functionSpecificationElement)
		{
			this.view = view;
			this.functionSpecificationElement = functionSpecificationElement;

			view.Presenter = this;
		}

		public ConfigureFunctionPresenter(IConfigureFunctionViewPhase2 view, IXmlElement functionSpecificationElement, IXmlElement functionElement)
			: this(view, functionSpecificationElement)
		{
			this.configuredFunction = new ConfiguredFunction(functionElement);
		}

		#region IConfigureFunctionPresenter interface

		public void ConfigureFunction()
		{
			setFunctionNameAndDescription();
			setParameters();

			beginConfiguration();
		}

		public void ParameterSelected(string parameterId)
		{
			setParameterNameAndDescription(parameterId);
		}

		public void ConfigurationCompleted()
		{
			raiseConfigurationCompletedEvent();
		}

		public void ConfigurationCanceled()
		{
		}


		private static readonly string functionElementStartXml = "<{0} version=\"{1}\">\r\n";
		private static readonly string functionElementEndXml = "</{0}>\r\n";

		public string ToXml()
		{
			StringBuilder xmlString = new StringBuilder();

			xmlString.AppendFormat(functionElementStartXml, functionSpecificationElement.GetAttribute("id"), functionSpecificationElement.GetAttribute("version"));

			foreach (IConfigurableParameter parameter in view.Parameters)
			{
				xmlString.Append(parameter.ToXml());
			}

			xmlString.AppendFormat(functionElementEndXml, functionSpecificationElement.GetAttribute("id"));

			return xmlString.ToString();
		}

		#endregion

		#region helper methods

		private void setFunctionNameAndDescription()
		{
			view.FunctionName = functionSpecificationElement.GetAttribute("name");
			view.FunctionDescription = functionSpecificationElement.GetChild("tr:description").Text;
		}

		private void setParameters()
		{
			view.Parameters = makeParameters();
		}

		private void beginConfiguration()
		{
			view.MakeVisible();
		}

		private void setParameterNameAndDescription(string parameterId)
		{
			foreach (IConfigurableParameter parameter in view.Parameters)
			{
				if (parameter.Id == parameterId)
				{
					view.ParameterName = parameter.Name;
					view.ParameterDescription = parameter.Description;
					break;
				}
			}
		}

		private Collection<IConfigurableParameter> makeParameters()
		{
			Collection<IConfigurableParameter> parameters = new Collection<IConfigurableParameter>();

			Collection<XmlElement> parameterElements = functionSpecificationElement.GetChildren("tr:parameter");

			foreach (IXmlElement parameterElement in parameterElements)
			{
				parameters.Add(makeParameter(parameterElement));
			}

			return parameters;
		}

		private IConfigurableParameter makeParameter(IXmlElement parameterSpecificationElement)
		{
			IConfigurableParameter parameter = null;

			switch (parameterSpecificationElement.GetAttribute("type"))
			{
				case "enumeration":
					parameter = new EnumerationParameter(parameterSpecificationElement, configuredFunction);
					break;

				case "tawala-mcq":
					parameter = new MCItemParameter(parameterSpecificationElement, configuredFunction);
					break;

				case "tawala-blank":
					parameter = new BlankParameter(parameterSpecificationElement, configuredFunction);
					break;

				case "tawala-form":
					parameter = new FormParameter(parameterSpecificationElement, configuredFunction);
					break;

				case "tawala-conditions":
					parameter = new ConditionsParameter(parameterSpecificationElement, configuredFunction);
					break;

				case "string":
				default:
					parameter = new StringParameter(parameterSpecificationElement, configuredFunction);
					break;
			}

			return parameter;
		}

		#endregion

		#region event handlers

		public static event EventHandler<ConfigurationCompletedEventArgs> FunctionConfigurationCompleted;

		private void raiseConfigurationCompletedEvent()
		{
			if (FunctionConfigurationCompleted != null)
			{
				IXmlElement functionElement = new XmlElement(this.ToXml());
				ConfigurationCompletedEventArgs args = new ConfigurationCompletedEventArgs(new ConfiguredFunction(functionElement));
				FunctionConfigurationCompleted(this, args);
			}
		}

		#endregion
	}

	public class ConfigurationCompletedEventArgs : EventArgs
	{
		private IConfiguredFunction configuredFunction;

		public ConfigurationCompletedEventArgs(IConfiguredFunction configuredFunction)
		{
			this.configuredFunction = configuredFunction;
		}

		public IConfiguredFunction ConfiguredFunction
		{
			get
			{
				return configuredFunction;
			}
		}
	}
}
