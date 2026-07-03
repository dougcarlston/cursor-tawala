using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Forms;
using Tawala.XmlSupport;
using Tawala.Proj;

namespace Tawala.FunctionConfiguration
{
	public class EnumerationParameter : ConfigurableParameter
	{
		private Collection<Choice> choices;

		protected EnumerationParameter(IXmlElement element) : base(element)
		{
			choices = new Collection<Choice>();

			Collection<XmlElement> choiceElements = element.GetChildren("tr:choice");

			foreach (IXmlElement choiceElement in choiceElements)
			{
				choices.Add(new Choice(choiceElement));
			}

			this.Value = choices[0];
		}

		/// <summary>
		/// Constructs an EnumerationParameter object from a &lt;parameter type="enumeration"&gt; XML element.
		/// </summary>
		public EnumerationParameter(IXmlElement element, IConfiguredFunction configuredFunction) : this(element)
		{
			string functionXml = configuredFunction.ToXml();

			if (functionXml != "")
			{
				IXmlElement functionElement = new XmlElement(functionXml);
				IXmlElement choiceElement = functionElement.GetChild(Id);
				string valueString = choiceElement.Text;
				int choiceIndex = (valueString == "" ? 0 : Convert.ToInt32(valueString) - 1);
				this.Value = choices[choiceIndex];
			}
		}

		public Collection<Choice> Choices
		{
			get { return choices; }
		}

		#region IConfigurableParameter interface

		public override Control DataEntryControl
		{
			get
			{
				return new EnumerationComboBox(this);
			}
		}

		#endregion

	}

	public class Choice : IParameterValue
	{
		private string name;
		private int value;

		/// <summary>
		/// Constructs a Choice object from a &lt;choice&gt; XML element.
		/// </summary>
		public Choice(IXmlElement element)
		{
			this.name = element.GetAttribute("description");
			this.value = Convert.ToInt32(element.GetAttribute("value"));
		}

		public string Name
		{
			get { return name; }
		}

		public int Value
		{
			get { return value; }
		}

		#region IParameterValue interface

		public override string ToString()
		{
			return value.ToString();
		}

		#endregion
	}
}
