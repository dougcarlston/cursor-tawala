using System;
using System.Windows.Forms;
using System.Text;
using Tawala.XmlSupport;
using Tawala.Proj;

namespace Tawala.FunctionConfiguration
{
	public class StringParameter : ConfigurableParameter
	{
		/// <summary>
		/// Constructs a StringParameter from a &lt;parameter type="string"&gt; XML element.
		/// </summary>
		protected StringParameter(IXmlElement element) : base(element)
		{
		}

		public StringParameter(IXmlElement element, IConfiguredFunction configuredFunction) : this(element)
		{
			string functionXml = configuredFunction.ToXml();

			if (functionXml != "")
			{
				IXmlElement functionElement = new XmlElement(functionXml);
				IXmlElement stringElement = functionElement.GetChild(Id);
				string valueString = stringElement.Text;
				this.Value = new StringValue(valueString);
			}
		}

		#region IConfigurableParameter interface

		public override Control DataEntryControl
		{
			get
			{
				return new StringTextBox(this);
			}
		}

		#endregion

	}

	public class StringValue : IParameterValue
	{
		private string value;

		public StringValue(string value)
		{
			this.value = value;
		}
	}
}
