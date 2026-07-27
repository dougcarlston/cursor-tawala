using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.XmlSupport;
using Tawala.ConfigurableFunction;
using Tawala.Controls;

namespace Tawala.ConfigurableFunction
{
	public class ConfigureFunctionPresenter : IConfigureFunctionPresenter
	{
		private IConfigureFunctionViewPhase2 view;
		private IXmlElement functionElement;
		private Dictionary<string, string> parameterValues;

		public ConfigureFunctionPresenter(IConfigureFunctionViewPhase2 view, IXmlElement functionElement)
		{
			this.view = view;
			this.functionElement = functionElement;

			parameterValues = new Dictionary<string, string>();

			view.Presenter = this;
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

		public void ParameterChanged(string parameterId, string parameterValue)
		{
			parameterValues[parameterId] = parameterValue;
		}

		public static event EventHandler<ConfigurationCompletedEventArgs> FunctionConfigurationCompleted;

		public void ConfigurationCompleted()
		{
			raiseConfigurationCompletedEvent();
		}

		private void raiseConfigurationCompletedEvent()
		{
			if (FunctionConfigurationCompleted != null)
			{
				ConfigurationCompletedEventArgs args = new ConfigurationCompletedEventArgs();
				FunctionConfigurationCompleted(this, args);
			}
		}

		public void ConfigurationCanceled()
		{
		}

		private static readonly string functionElementStartXml = "<{0} version=\"{1}\">\r\n";
		private static readonly string functionElementEndXml = "</{0}>\r\n";

		public string ToXml()
		{
			StringBuilder xmlString = new StringBuilder();

			xmlString.AppendFormat(functionElementStartXml, functionElement.GetAttribute("id"), functionElement.GetAttribute("version"));

			foreach (IConfigurableParameter parameter in view.Parameters)
			{
				xmlString.Append(parameter.ToXml());
			}

			xmlString.AppendFormat(functionElementEndXml, functionElement.GetAttribute("id"));

			return xmlString.ToString();
		}

		#endregion

		#region helper methods

		private void setFunctionNameAndDescription()
		{
			view.FunctionName = functionElement.GetAttribute("name");
			view.FunctionDescription = functionElement.GetChild("tr:description").Text;
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

			Collection<XmlElement> parameterElements = functionElement.GetChildren("tr:parameter");

			foreach (IXmlElement parameterElement in parameterElements)
			{
				parameters.Add(makeParameter(parameterElement));
			}

			return parameters;
		}

		private IConfigurableParameter makeParameter(IXmlElement element)
		{
			IConfigurableParameter parameter = null;

			switch (element.GetAttribute("type"))
			{
				case "enumeration":
					parameter = new EnumerationParameter(element);
					break;

				case "tawala-mcq":
					parameter = new MCQParameter(element);
					break;

				case "tawala-blank":
					parameter = new BlankParameter(element);
					break;

				case "tawala-form":
					parameter = new FormParameter(element);
					break;

				case "string":
				default:
					parameter = new StringParameter(element);
					break;
			}

			return parameter;
		}

		#endregion
	}

	public class ConfigurationCompletedEventArgs : EventArgs
	{
		public ConfigurationCompletedEventArgs()
		{
		}
	}

	public interface IConfiguredFunction
	{
		string ToXml();
	}

	public class ConfiguredFunction : IConfiguredFunction
	{
		public ConfiguredFunction(IXmlElement functionElement, Collection<IConfigurableParameter> functionParameters)
		{
		}

		public string ToXml()
		{
			return "";
		}
	}
}
