using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Proj;
using Tawala.XmlSupport;

namespace Tawala.FunctionConfiguration
{
	public class ConfiguredFunction : IConfiguredFunction
	{
		public static ConfiguredFunction NULL = new NullConfiguredFunction();
		private IXmlElement functionElement;
		Dictionary<string, string> parameterValueStrings;

		protected ConfiguredFunction()
		{
		}

		public ConfiguredFunction(IXmlElement functionElement)
		{
			this.functionElement = functionElement;

			Collection<XmlElement> parameterElements = functionElement.GetChildren();
			parameterValueStrings = new Dictionary<string, string>();

			foreach (IXmlElement parameterElement in parameterElements)
			{
				parameterValueStrings.Add(parameterElement.Name, parameterElement.Text);
			}
		}

		private static int instanceId = 1;

		public virtual int InstanceId
		{
			get { return instanceId++; }
		}

		public virtual string GetParameterValueString(string parameterId)
		{
			return parameterValueStrings[parameterId];
		}

		public virtual string ToXml()
		{
			return functionElement.OuterXml;
		}

		/// <summary>
		/// Class implementing Null Object for ConfiguredFunction class.
		/// </summary>
		private class NullConfiguredFunction : ConfiguredFunction
		{
			public override int InstanceId
			{
				get { return 0; }
			}

			public override string GetParameterValueString(string parameterId)
			{
				return "";
			}

			public override string ToXml()
			{
				return "";
			}
		}
	}


}
