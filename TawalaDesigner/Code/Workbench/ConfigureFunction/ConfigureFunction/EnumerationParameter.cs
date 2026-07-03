using System;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.XmlSupport;

namespace Tawala.ConfigurableFunction
{
	public class EnumerationParameter : ConfigurableParameter
	{
		private Collection<Choice> choices;
		private string choiceName;

		/// <summary>
		/// Constructs an EnumerationParameter object from a &lt;parameter type="enumeration"&gt; XML element.
		/// </summary>
		public EnumerationParameter(IXmlElement element) : base(element)
		{
			choices = new Collection<Choice>();

			Collection<XmlElement> choiceElements = element.GetChildren("tr:choice");

			foreach (IXmlElement choiceElement in choiceElements)
			{
				choices.Add(new Choice(choiceElement));
			}

			choiceName = choices[0].Name;
		}

		public Collection<Choice> Choices
		{
			get { return choices; }
		}

		public string ChoiceName
		{
			set { choiceName = value; }
		}

		#region IConfigurableParameter interface

		public override string Value
		{
			get
			{
				return getChoiceValue();
			}
		}

		#endregion

		private string getChoiceValue()
		{
			foreach (Choice choice in Choices)
			{
				if (choice.Name == choiceName)
				{
					return choice.Value.ToString();
				}
			}

			return "";
		}

	}

	public class Choice
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
			get { return this.value; }
		}
	}
}
