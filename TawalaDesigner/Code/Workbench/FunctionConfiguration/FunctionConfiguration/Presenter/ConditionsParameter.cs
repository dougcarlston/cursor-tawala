using System;
using System.Windows.Forms;
using System.Text;
using Tawala.XmlSupport;
using Tawala.Controls;
using Tawala.Proj;

namespace Tawala.FunctionConfiguration
{
	public class ConditionsParameter : ConfigurableParameter
	{
		private ConditionGroupCollection conditionGroups;
		private Conditions conditions;
		private Control containerControl;
		private ComboBox comboBoxAndOr;

		protected ConditionsParameter(IXmlElement element) : base(element)
		{
		}

		/// <summary>
		/// Constructs a ConditionsParameter object from a &lt;parameter type="tawala-conditions"&gt; XML element.
		/// </summary>
		public ConditionsParameter(IXmlElement element, IConfiguredFunction configuredFunction) : this(element)
		{
			string functionXml = configuredFunction.ToXml();

			if (functionXml != "")
			{
				IXmlElement functionElement = new XmlElement(configuredFunction.ToXml());
				IXmlElement conditionsElement = functionElement.GetChild("conditions");

				if (conditionsElement.Name == "conditions")
				{
					this.conditions = new Conditions(conditionsElement, Process.NULL);
					this.Value = this.conditions as IParameterValue;
				}
			}
		}

		#region IConfigurableParameter interface

		public override Control DataEntryControl
		{
			get
			{
				return containerControl;
			}
		}

		public override string ToXml()
		{
			if (conditionGroups.Conditions.Count > 0)
			{
				return conditionGroups.Conditions.ToXml();
			}

			return "";
		}

		#endregion

		/// <summary>
		/// Sets the control used to contain the ConditionGroupCollection
		/// </summary>
		public Control ContainerControl
		{
			set
			{
				containerControl = value;
				setConditionGroups();
			}
		}

		/// <summary>
		/// Sets the combo box control used to switch the condition logic between "and" and "or"
		/// </summary>
		public ComboBox AndOrComboBox
		{
			set
			{
				comboBoxAndOr = value;
				setConditionGroups();
			}
		}

		/// <summary>
		/// Lazy initializer for condition groups.
		/// </summary>
		private void setConditionGroups()
		{
			if (conditionGroups == null)
			{
				if (containerControl != null && comboBoxAndOr != null)
				{
					conditionGroups = new ConditionGroupCollection(containerControl, comboBoxAndOr, true);

					if (conditions != null)
					{
						conditionGroups.Conditions = conditions;
					}
				}
			}
		}
	}
}
