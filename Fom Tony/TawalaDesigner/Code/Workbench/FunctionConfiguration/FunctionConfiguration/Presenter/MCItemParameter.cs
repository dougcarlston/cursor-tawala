using System;
using System.Windows.Forms;
using System.Text;
using Tawala.Proj;
using Tawala.XmlSupport;

namespace Tawala.FunctionConfiguration
{
	public class MCItemParameter : ConfigurableParameter
	{
		private static NullMCItem nullMCItem = new NullMCItem();

		/// <summary>
		/// Constructs an MCQParameter from a &lt;parameter type="tawala-mcq"&gt; XML element.
		/// </summary>
		protected MCItemParameter(IXmlElement element) : base(element)
		{
		}

		public MCItemParameter(IXmlElement element, IConfiguredFunction configuredFunction) : this(element)
		{
			this.Value = nullMCItem;

			string functionXml = configuredFunction.ToXml();

			if (functionXml != "")
			{
				IXmlElement functionElement = new XmlElement(functionXml);
				IXmlElement mcItemElement = functionElement.GetChild(Id);
				string valueString = mcItemElement.Text;
				this.Value = (valueString == "" ? nullMCItem : Project.FieldMapByName[valueString] as IParameterValue);
			}
		}


		#region IConfigurableParameter interface

		public override Control DataEntryControl
		{
			get
			{
				return new MCItemTextBox(this);
			}
		}

		#endregion

		private class NullMCItem : MCItem
		{
			public override string ToString()
			{
				return "";
			}
		}

	}

}
