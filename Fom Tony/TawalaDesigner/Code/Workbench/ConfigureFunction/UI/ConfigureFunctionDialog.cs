using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Tawala.XmlSupport;
using Tawala.ConfigurableFunction;

namespace Tawala.Controls
{
	public partial class ConfigureFunctionDialogPhase2 : Form, IConfigureFunctionViewPhase2
	{
		private IConfigureFunctionPresenter presenter;
		private Collection<IConfigurableParameter> parameters;

		public ConfigureFunctionDialogPhase2()
		{
			InitializeComponent();
		}

		#region IConfigureFunctionView interface

		public IConfigureFunctionPresenter Presenter
		{
			get
			{
				return presenter;
			}
			set
			{
				presenter = value;
			}
		}

		public string FunctionName
		{
			get
			{
				return labelFunctionName.Text;
			}
			set
			{
				labelFunctionName.Text = value;
			}
		}

		public string FunctionDescription
		{
			get
			{
				return labelFunctionDescription.Text;
			}
			set
			{
				labelFunctionDescription.Text = value;
			}
		}

		public string ParameterName
		{
			get
			{
				return labelParameterName.Text;
			}
			set
			{
				labelParameterName.Text = value;
			}
		}

		public string ParameterDescription
		{
			get
			{
				return labelParameterDescription.Text;
			}
			set
			{
				labelParameterDescription.Text = value;
			}
		}

		public Collection<IConfigurableParameter> Parameters
		{
			get
			{
				return parameters;
			}
			set
			{
				parameters = value;

				makeParameterControls();
			}
		}

		public void MakeVisible()
		{
			Show();
		}

		#endregion

		#region helper methods

		private void makeParameterControls()
		{
			int parameterIndex = 0;

			foreach (IConfigurableParameter parameter in parameters)
			{
				FunctionParameterControl parameterControl = new FunctionParameterControl(parameter);
				parameterControl.Location = new Point(0, (parameterIndex++ * parameterControl.Height) + (parameterControl.Height / 4));
				panelParameters.Controls.Add(parameterControl);

				parameterControl.DataEntryControl.Enter += new EventHandler(dataEntryControl_Enter);
				parameterControl.DataEntryControl.TextChanged += new EventHandler(dataEntryControl_TextChanged);

				panelParameters.Controls[0].Select();
				panelParameters.Controls[0].Focus();
			}
		}

		#endregion

		#region event handling

		private void dataEntryControl_Enter(object sender, EventArgs e)
		{
			presenter.ParameterSelected(((IIdentityControl)sender).Id);
		}

		void dataEntryControl_TextChanged(object sender, EventArgs e)
		{
			presenter.ParameterChanged(((IIdentityControl)sender).Id, ((IIdentityControl)sender).Value);
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			presenter.ConfigurationCompleted();
			Close();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			presenter.ConfigurationCanceled();
			Close();
		}

		#endregion
	}
}