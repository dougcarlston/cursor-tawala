using System;
using System.Windows.Forms;
using System.Text;
using Tawala.Proj;
using Tawala.XmlSupport;

namespace Tawala.FunctionConfiguration
{
	public class BlankParameter : ConfigurableParameter
	{
		private static NullBlank nullBlank = new NullBlank();

		protected BlankParameter(IXmlElement element) : base(element)
		{
		}

		/// <summary>
		/// Constructs a BlankParameter from a &lt;parameter type="tawala-blank"&gt; XML element.
		/// </summary>
		public BlankParameter(IXmlElement element, IConfiguredFunction configuredFunction) : this(element)
		{
			this.Value = nullBlank;

			string functionXml = configuredFunction.ToXml();

			if (functionXml != "")
			{
				IXmlElement functionElement = new XmlElement(functionXml);
				IXmlElement blankElement = functionElement.GetChild(Id);
				string valueString = blankElement.Text;
				this.Value = (valueString == "" ? nullBlank : Project.FieldMapByName[valueString] as IParameterValue);
			}
		}

		#region IConfigurableParameter interface

		public override Control DataEntryControl
		{
			get
			{
				return new BlankTextBox(this);
			}
		}

		#endregion

		private class NullBlank : Blank
		{
			public override string ToString()
			{
				return "";
			}
		}

	}
}
