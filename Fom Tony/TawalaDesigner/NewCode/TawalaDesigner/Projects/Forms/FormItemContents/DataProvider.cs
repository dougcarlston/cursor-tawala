using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Factories;
using Tawala.Projects.Function;
using Tawala.XmlSupport;
using Tawala.Functions.Runtime;

namespace Tawala.Projects.Forms.FormItemContents
{
	[Serializable]
	public class DataProvider : FormItemContents
	{
		private IFunction function;
		private IXmlElement element;

		public DataProvider()
		{
		}

		public DataProvider(IFunction function)
		{
			Function = function;
		}

		public DataProvider(IXmlElement element)
		{
			this.element = element;

			if (element.HasChild("dynamic-mcq"))
			{
				Function = new XmlToFunctionConverter().ConvertFrom(element.GetChild("dynamic-mcq"));
			}
		}

		public override string ToXml()
		{
			return @"<data-provider>" + function.ToXml() + @"</data-provider>";
		}

		public override string ToXhtml(IFormItem formItem)
		{
			return string.Format("<t:Choice functionId=\"{0}\" style=\"color:#0000C0;\">Choices are populated from stored data.</t:Choice>", function.InstanceId);
		}

		public IFunction Function
		{
			get { return function; }
			
			set
			{
				function = value;
				Project.FunctionMapById.AddUnique(function);
			}
		}

		public override void ResolveFunctionReferences()
		{
			if (element != null)
			{
				Function = new XmlToFunctionConverter().ConvertFrom(element.GetChild("dynamic-mcq"));
			}
		}
	}

	[Serializable]
	public class DataXmlProvider : DataProvider
	{
		public DataXmlProvider(IXmlElement element) : base(element)
		{
		}
	}

	[Serializable]
	public class DataXhtmlProvider : DataProvider
	{
		public DataXhtmlProvider(IXmlElement element)
		{
			int id = Convert.ToInt32(element.GetAttribute("functionId"));
			Function = Project.FunctionMapById[id];
		}
	}
}
