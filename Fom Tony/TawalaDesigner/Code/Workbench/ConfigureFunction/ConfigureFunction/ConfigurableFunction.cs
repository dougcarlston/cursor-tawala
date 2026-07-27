using System;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.XmlSupport;

namespace ConfigureFunction
{
	public class ConfigurableFunction : IConfigurableFunction
	{
		private IXmlElement element;
		private Collection<IConfigurableParameter> parameters;

		public ConfigurableFunction()
		{
		}

		public ConfigurableFunction(IXmlElement element) : this()
		{
			this.element = element;

			this.parameters = new Collection<IConfigurableParameter>();

			Collection<XmlElement> parameterElements = element.GetChildren("tr:parameter");

			foreach (IXmlElement parameterElement in parameterElements)
			{
				parameters.Add(makeParameter(parameterElement));
			}
		}

		private IConfigurableParameter makeParameter(IXmlElement element)
		{
			IConfigurableParameter parameter = null;

			switch (element.GetAttribute("type"))
			{
				case "enumeration":
					parameter = new EnumerationParameter(element);
					break;

				case "string":
				default:
					parameter = new StringParameter(element);
				break;
			}

			return parameter;
		}

		public Collection<IConfigurableParameter> Parameters
		{
			get
			{
				return parameters;
			}
		}

		public string ToXml()
		{
			return "";
		}
	}
}
