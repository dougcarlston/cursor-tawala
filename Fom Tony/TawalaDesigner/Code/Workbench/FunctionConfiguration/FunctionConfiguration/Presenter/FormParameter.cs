using System;
using System.Windows.Forms;
using System.Text;
using Tawala.Proj;
using Tawala.XmlSupport;

namespace Tawala.FunctionConfiguration
{
	public class FormParameter : ConfigurableParameter
	{
		/// <summary>
		/// Constructs a FormParameter object from a &lt;parameter type="tawala-form"&gt; XML element.
		/// </summary>
		protected FormParameter(IXmlElement element) : base(element)
		{
		}

		public FormParameter(IXmlElement element, IConfiguredFunction configuredFunction) : this(element)
		{
			string functionXml = configuredFunction.ToXml();

			if (functionXml != "")
			{
				IXmlElement functionElement = new XmlElement(functionXml);
				IXmlElement formElement = functionElement.GetChild(Id);
				string valueString = formElement.Text;
				this.Value = Project.Current.GetForm(valueString);
			}
		}

		#region IConfigurableParameter interface

		public override Control DataEntryControl
		{
			get
			{
				return new FormComboBox(this);
			}
		}

		#endregion
	}
}
