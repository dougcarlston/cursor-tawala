using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Tawala.FunctionConfiguration;
using Tawala.Proj;
using Tawala.XmlSupport;

namespace Tawala.FunctionConfiguration
{
	/// <summary>
	/// A composite control consisting of a label for the parameter name, a control for data entry, and a label for the parameter type.
	/// </summary>
	public partial class FunctionParameterControl : UserControl
	{
		static Dictionary<string, string> friendlyParameterTypeNames;

		static FunctionParameterControl()
		{
			friendlyParameterTypeNames = new Dictionary<string, string>();

			friendlyParameterTypeNames.Add("enumeration", "pick one");
			friendlyParameterTypeNames.Add("tawala-mcq", "multiple choice");
			friendlyParameterTypeNames.Add("tawala-blank", "blank");
			friendlyParameterTypeNames.Add("tawala-form", "pick one");
			friendlyParameterTypeNames.Add("tawala-contents-field", "any field");
			friendlyParameterTypeNames.Add("text", "type any text");
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
			labelParameterType.Text = friendlyParameterTypeString(parameter.Type);
		}

		private void setDataEntryControl(IConfigurableParameter parameter)
		{
			dataEntryControl = parameter.DataEntryControl;
			Controls.Add(dataEntryControl);
		}

		/// <summary>
		/// The control used to enter the parameter value.
		/// </summary>
		private Control dataEntryControl;

		public Control DataEntryControl
		{
			get { return dataEntryControl; }
		}

		private static string friendlyParameterTypeString(string parameterName)
		{
			return ("- " + friendlyParameterTypeNames[parameterName]);
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
					labelParameterName.Location = new Point(3, 5);
					labelParameterName.Width = (Parent.Width * 1 / 3) - 20;

					dataEntryControl.Location = new Point(Parent.Width * 1 / 3, 0);
					dataEntryControl.Width = Parent.Width * 1 / 3;

					labelParameterType.Location = new Point((Parent.Width * 2 / 3) + 10, 5);
					labelParameterType.Width = (Parent.Width * 1 / 3) - 20;

					locationSet = true;
				}
			}
		}
	}

}
