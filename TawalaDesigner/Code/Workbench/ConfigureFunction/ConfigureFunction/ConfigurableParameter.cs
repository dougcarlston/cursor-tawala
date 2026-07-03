using System;
using System.Collections.Generic;
using System.Text;
using Tawala.XmlSupport;

namespace Tawala.ConfigurableFunction
{
	public abstract class ConfigurableParameter : IConfigurableParameter
	{
		private IXmlElement element;
		private string id;
		private string name;
		private string type;
		private bool required;
		private string description;

		public ConfigurableParameter(IXmlElement element)
		{
			this.element = element;

			this.id = element.GetAttribute("id");
			this.name = element.GetAttribute("name");
			this.type = element.GetAttribute("type");
			this.required = Convert.ToBoolean(element.GetAttribute("required"));
			this.description = element.GetChild("tr:description").Text;
		}

		#region IConfigurableParameter interface

		public string Id
		{
			get
			{
				return id;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public string Type
		{
			get
			{
				return type;
			}
		}

		public bool Required
		{
			get
			{
				return required;
			}
		}

		public string Description
		{
			get
			{
				return description;
			}
		}

		public abstract string Value { get; }

		public virtual string ToXml()
		{
			return string.Format("<{0}>{1}<{0}/>\r\n", Id, Value);
		}

		#endregion
	}
}
