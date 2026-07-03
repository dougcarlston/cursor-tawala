using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Tawala.ConfigurableFunction;
using Tawala.Proj;
using Tawala.XmlSupport;

namespace Tawala.Controls
{
	public partial class FunctionParameterControl : UserControl
	{
		static Dictionary<string, string> friendlyParameterNames;

		static FunctionParameterControl()
		{
			friendlyParameterNames = new Dictionary<string, string>();

			friendlyParameterNames.Add("enumeration", "pick one");
			friendlyParameterNames.Add("tawala-mcq", "multiple choice");
			friendlyParameterNames.Add("tawala-blank", "blank");
			friendlyParameterNames.Add("tawala-form", "pick one");
		}

		protected FunctionParameterControl()
		{
			InitializeComponent();
		}

		public FunctionParameterControl(IConfigurableParameter parameter) : this()
		{
			setParameterName(parameter);
			setParameterType(parameter);

			setDataEntryControl(parameter);
		}

		private void setParameterName(IConfigurableParameter parameter)
		{
			labelParameterName.Text = parameter.Name;
		}

		private void setParameterType(IConfigurableParameter parameter)
		{
			labelParameterType.Text = friendlyParameterString(parameter.Type);
		}

		private void setDataEntryControl(IConfigurableParameter parameter)
		{
			dataEntryControl = makeDataEntryControl(parameter);
			Controls.Add(dataEntryControl);
		}

		private Control makeDataEntryControl(IConfigurableParameter parameter)
		{
			Control dataEntryControl;

			switch (parameter.Type)
			{
				case "enumeration":
					dataEntryControl = new EnumerationComboBox(parameter as EnumerationParameter);
					break;

				case "tawala-mcq":
					dataEntryControl = new MCQTextBox(parameter as MCQParameter);
					break;

				case "tawala-blank":
					dataEntryControl = new BlankTextBox(parameter as BlankParameter);
					break;

				case "tawala-form":
					dataEntryControl = new FormComboBox(parameter as FormParameter);
					break;

				case "string":
				default:
					dataEntryControl = new StringTextBox(parameter as StringParameter);
					break;
			}

			return dataEntryControl;
		}

		/// <summary>
		/// The combo box or text box used to enter the parameter value.
		/// </summary>
		private Control dataEntryControl;

		public Control DataEntryControl
		{
			get { return dataEntryControl; }
		}

		private static string friendlyParameterString(string parameterName)
		{
			return ("- " + friendlyParameterNames[parameterName]);
		}

		private void FunctionParameterControl_Load(object sender, EventArgs e)
		{
			Application.Idle += new EventHandler(application_Idle);
		}

		private void application_Idle(object sender, EventArgs e)
		{
			setDataEntryControlLocation();
		}

		private bool locationSet = false;

		private void setDataEntryControlLocation()
		{
			if (!locationSet)
			{
				if (Parent != null)
				{
					dataEntryControl.Location = new Point(Parent.Width * 1 / 3, 0);
					dataEntryControl.Width = Parent.Width * 1 / 3;

					locationSet = true;
				}
			}
		}

	}

}
