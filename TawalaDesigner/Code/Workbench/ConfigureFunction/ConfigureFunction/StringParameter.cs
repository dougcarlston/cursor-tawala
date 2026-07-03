using System;
using System.Text;
using Tawala.XmlSupport;

namespace Tawala.ConfigurableFunction
{
	public class StringParameter : ConfigurableParameter
	{
		private string text;

		/// <summary>
		/// Constructs a StringParameter from a &lt;parameter type="string"&gt; XML element.
		/// </summary>
		public StringParameter(IXmlElement element) : base(element)
		{
		}

		#region IConfigurableParameter interface

		public override string Value
		{
			get
			{
				return "";
			}
		}

		#endregion

		public string Text
		{
			set { text = value; }
		}
	}
}
