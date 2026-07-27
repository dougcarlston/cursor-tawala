using System;
using System.Text;
using Tawala.XmlSupport;

namespace Tawala.ConfigurableFunction
{
	public class MCQParameter : ConfigurableParameter
	{
		private string mcqName;

		/// <summary>
		/// Constructs an MCQParameter from a &lt;parameter type="tawala-mcq"&gt; XML element.
		/// </summary>
		public MCQParameter(IXmlElement element) : base(element)
		{
		}

		#region IConfigurableParameter interface

		public override string Value
		{
			get
			{
				return mcqName;
			}
		}

		#endregion

		public string MCQName
		{
			set { mcqName = value; }
		}
	}
}
