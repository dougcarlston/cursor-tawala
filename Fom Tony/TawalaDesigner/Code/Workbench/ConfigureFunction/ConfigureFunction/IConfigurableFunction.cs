using System;
using System.Collections.ObjectModel;

namespace ConfigureFunction
{
	public interface IConfigurableFunction
	{
		Collection<IConfigurableParameter> Parameters { get; }
		
		string ToXml();
	}
}
