using System;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.XmlSupport;

namespace Tawala.ConfigurableFunction
{
	public class FormParameter : ConfigurableParameter
	{
		private string formName;

		/// <summary>
		/// Constructs a FormParameter object from a &lt;parameter type="tawala-form"&gt; XML element.
		/// </summary>
		public FormParameter(IXmlElement element) : base(element)
		{
		}

		public string FormName
		{
			set { formName = value; }
		}

		#region IConfigurableParameter interface

		public override string Value
		{
			get
			{
				return formName;
			}
		}

		#endregion
	}
}
