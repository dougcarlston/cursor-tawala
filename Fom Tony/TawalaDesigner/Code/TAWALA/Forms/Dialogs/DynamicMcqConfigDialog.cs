using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Tawala.Forms
{
	public partial class DynamicMcqConfigDialog : System.Windows.Forms.Form
	{
		public DynamicMcqConfigDialog(Tawala.Projects.MCItem mcItem)
			: this()
		{
			this.mcItem = mcItem;
		}

		protected DynamicMcqConfigDialog()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			comboBoxForms.DataSource = createFormsCollection();

			if (mcItem.ChoiceSourceForm != null)
			{
				checkBoxChoiceFromStoredData.Checked = true;

				comboBoxForms.SelectedItem = mcItem.ChoiceSourceForm;
				comboBoxChoiceFields.SelectedItem = mcItem.ChoiceSourceNameField;
				comboBoxIdFields.SelectedItem = mcItem.ChoiceSourceIdField;
			}

			base.OnLoad(e);
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if (checkBoxChoiceFromStoredData.Checked)
			{
				mcItem.ChoiceSourceForm = comboBoxForms.SelectedItem as Tawala.Projects.Form;
				mcItem.ChoiceSourceNameField = comboBoxChoiceFields.SelectedItem as Tawala.Projects.IPaletteField;
				mcItem.ChoiceSourceIdField = comboBoxIdFields.SelectedItem as Tawala.Projects.IPaletteField;
			}
			else
			{
				mcItem.ChoiceSourceForm = null;
				mcItem.ChoiceSourceIdField = null;
				mcItem.ChoiceSourceNameField = null;
			}

			Close();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void synchronizeFields(Tawala.Projects.Form form)
		{
			Collection<Tawala.Projects.IPaletteField> fields = createFieldsCollection(form);

			comboBoxChoiceFields.DataSource = new BindingList<Tawala.Projects.IPaletteField>(fields);
			comboBoxIdFields.DataSource = new BindingList<Tawala.Projects.IPaletteField>(fields);
		}

		private void checkBoxChoiceFromStoredData_CheckedChanged(object sender, EventArgs e)
		{
			groupBox1.Enabled = checkBoxChoiceFromStoredData.Checked;
		}

		private void comboBoxForms_SelectedValueChanged(object sender, EventArgs e)
		{
			synchronizeFields(comboBoxForms.SelectedItem as Tawala.Projects.Form);
		}

		private static Collection<Tawala.Projects.Form> createFormsCollection()
		{
			Collection<Tawala.Projects.Form> formCollection = new Collection<Tawala.Projects.Form>();

			foreach (Tawala.Projects.Form form in Tawala.Projects.Project.Current.AllForms)
			{
				foreach (Tawala.Projects.IPaletteField field in form.GetAllFields())
				{
					if (field is Tawala.Projects.Blank || field is Tawala.Projects.HiddenField)
					{
						formCollection.Add(form);
						break;
					}
				}
			}

			return formCollection;
		}

		private static Collection<Tawala.Projects.IPaletteField> createFieldsCollection(Tawala.Projects.Form form)
		{
			Collection<Tawala.Projects.IPaletteField> fields = new Collection<Tawala.Projects.IPaletteField>();

			foreach (Tawala.Projects.IPaletteField field in form.GetAllFields())
			{
				if (field is Tawala.Projects.Blank || field is Tawala.Projects.HiddenField)
				{
					fields.Add(field);
				}
			}
			return fields;
		}

		private Tawala.Projects.MCItem mcItem;
	}
}
