using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Tawala.XmlSupport;
using Tawala.ConfigurableFunction;
using Tawala.Controls;
using Tawala.Proj;

namespace Program
{
	public partial class ApplicationForm : System.Windows.Forms.Form
	{
		private string displayComponentXmlString =
			"<tr:display-component id=\"popular-choice-correlation-table\" name=\"POPULAR CHOICE CORRELATION TABLE\" version=\"1\" xmlns:tr=\"http://www.tawala.com/component-repository\">" +
			"<tr:description>Displays a table showing the most popular choice for a given multiple choice question, and correlates that question's choices with those from a second multiple choice question.</tr:description>" +
			"<tr:parameter id=\"rank\" type=\"enumeration\" name=\"Rank\" required=\"true\">" +
			"<tr:description>Indicates the ranking of the popular choice, e.g. first, second, etc.</tr:description>" +
			"<tr:choice value=\"1\" description=\"first\" />" +
			"<tr:choice value=\"2\" description=\"second\" />" +
			"<tr:choice value=\"3\" description=\"third\" />" +
			"</tr:parameter>" +
			"<tr:parameter id=\"choice-available-field-name\" type=\"tawala-mcq\" name=\"Main Question\" required=\"true\">" +
			"<tr:description>The multiple choice question to display popular choice information for.</tr:description>" +
			"</tr:parameter>" +
			"<tr:parameter id=\"choice-preferred-field-name\" type=\"tawala-mcq\" name=\"Second Question\" required=\"true\">" +
			"<tr:description>The multiple choice question to correlate with the main question.</tr:description>" +
			"</tr:parameter>" +
			"<tr:parameter id=\"popular-choice-display-field-name\" type=\"tawala-blank\" name=\"Column One Contents\" required=\"true\">" +
			"<tr:description>The blank whose values will be shown in the first column of the table.</tr:description>" +
			"</tr:parameter>" +
			"</tr:display-component>";

		private string functionXmlString =
			"<tr:function id=\"record-count\" name=\"RECORD COUNT\" version=\"1\" return-type=\"int\" xmlns:tr=\"http://www.tawala.com/component-repository\">" +
			"<tr:description>Returns the number of records submitted for a particular form.</tr:description> " +
			"<tr:parameter id=\"simple-list-form-name\" type=\"tawala-form\" name=\"Form\" required=\"true\">" +
			"<tr:description>The form whose records you wish to count.</tr:description> " +
			"</tr:parameter>" +
			"</tr:function>";

		private Tawala.Proj.Form form1;
		private Tawala.Proj.Form form2;
		private MCItem mcItem1;
		private MCItem mcItem2;
		private FibItem fibItem;
		private Blank blank;
		private IConfigureFunctionPresenter presenter;

		public ApplicationForm()
		{
			InitializeComponent();

			Project.NewTestProject();

			form1 = Project.Current.AddForm();
			form2 = Project.Current.AddForm();

			mcItem1 = new MCItem();
			form1.Add(mcItem1);
			mcItem2 = new MCItem();
			form1.Add(mcItem2);

			fibItem = new FibItem();
			form1.Add(fibItem);
			blank = fibItem.BlankList[0];

			ConfigureFunctionPresenter.FunctionConfigurationCompleted += new EventHandler<ConfigurationCompletedEventArgs>(functionConfigurationCompleted);
		}

		void functionConfigurationCompleted(object sender, ConfigurationCompletedEventArgs e)
		{
			textBoxOutputXml.Text = presenter.ToXml();
		}

		private void ApplicationForm_Load(object sender, EventArgs e)
		{
			setupFieldsPalette();
		}

		private void setupFieldsPalette()
		{
			TreeNode rootNode = treeViewFieldsPalette.Nodes.Add(form1.Name);

			rootNode.Nodes.Add(makeFieldNode(mcItem1));
			rootNode.Nodes.Add(makeFieldNode(mcItem2));
			rootNode.Nodes.Add(makeFieldNode(blank));

			treeViewFieldsPalette.ExpandAll();
		}

		private TreeNode makeFieldNode(IField field)
		{
			TreeNode fieldNode = new TreeNode(field.FieldName);
			fieldNode.Tag = field;

			return fieldNode;
		}

		private void treeViewFieldsPalette_ItemDrag(object sender, ItemDragEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				TreeNode node = e.Item as TreeNode;
				if (node != null && node.Tag is IField)
				{
					treeViewFieldsPalette.SelectedNode = node;
					DataObject dataObject = new DataObject();

					if (node.Tag is MCItem)
					{
						dataObject.SetData(typeof(MCItem), node.Tag);
					}
					else if (node.Tag is Blank)
					{
						dataObject.SetData(typeof(Blank), node.Tag);
					}

					DoDragDrop(dataObject, DragDropEffects.Copy);
				}
			}
		}

		private void buttonConfigureDisplayComponent_Click(object sender, EventArgs e)
		{
			IXmlElement functionElement = new XmlElement(displayComponentXmlString);
			presenter = new ConfigureFunctionPresenter(new ConfigureFunctionDialogPhase2(), functionElement);

			presenter.ConfigureFunction();
		}

		private void buttonConfigureFunction_Click(object sender, EventArgs e)
		{
			IXmlElement functionElement = new XmlElement(functionXmlString);
			presenter = new ConfigureFunctionPresenter(new ConfigureFunctionDialogPhase2(), functionElement);

			presenter.ConfigureFunction();
		}
	}
}