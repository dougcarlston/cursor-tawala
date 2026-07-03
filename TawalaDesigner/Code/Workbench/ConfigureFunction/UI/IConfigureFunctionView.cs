using System;
using System.Text;
using Tawala.XmlSupport;

namespace ConfigureFunction
{
	public interface IConfigureFunctionView
	{
		void SetFunction(IXmlElement functionElement);
	}
}
