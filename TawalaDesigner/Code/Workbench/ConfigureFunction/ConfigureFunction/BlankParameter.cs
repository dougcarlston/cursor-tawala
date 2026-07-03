using System;
using System.Text;
using Tawala.XmlSupport;

namespace Tawala.ConfigurableFunction
{
	public class BlankParameter : ConfigurableParameter
	{
		private string blankName;

		/// <summary>
		/// Constructs a blankParameter from a &lt;parameter type="tawala-blank"&gt; XML element.
		/// </summary>
		public BlankParameter(IXmlElement element) : base(element)
		{
		}

		#region IConfigurableParameter interface

		public override string Value
		{
			get
			{
				return blankName;
			}
		}

		#endregion

		public string BlankName
		{
			set { blankName = value; }
		}
	}
}
