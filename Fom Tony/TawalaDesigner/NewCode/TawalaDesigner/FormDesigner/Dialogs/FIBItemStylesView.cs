// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Interfaces;

namespace Tawala.FormDesigner.Dialogs
{
	public partial class FibItemStylesView : Form, IFibItemStylesView
	{
		private IFormView activeView = null;

		protected FibItemStylesView()
		{
			InitializeComponent();
		}

		public FibItemStylesView(IFormView view)
			: this()
		{
			activeView = view;
		}

		public bool FibApplyAllSpecified
		{
			get { return applyToAll; }
		}

		public bool FibFreeformSpecified
		{
			get { return radioButtonFreeform.Checked; }
		}

		public bool FibLeftLabelsSpecified
		{
			get { return radioButtonLeftJustified.Checked && !checkBoxAlignRight.Checked; }
		}

		public bool FibRightLabelsSpecified
		{
			get { return radioButtonRightJustified.Checked && !checkBoxAlignRight.Checked; }
		}

		public bool FibLeftLabelsJustifiedSpecified
		{
			get { return radioButtonLeftJustified.Checked && checkBoxAlignRight.Checked; }
		}

		public bool FibRightLabelsJustifiedSpecified
		{
			get { return radioButtonRightJustified.Checked && checkBoxAlignRight.Checked; }
		}

		public bool FibTopLabelsSpecified
		{
			get { return radioButtonAbove.Checked; }
		}

		private void FormItemStylesDialog_Activated(object sender, EventArgs e)
		{
			Application.Idle += new EventHandler(application_Idle);
		}

		void application_Idle(object sender, EventArgs e)
		{
			buttonApplyAll.Enabled = anyStyleSelected();
			buttonApplySelected.Enabled = anyStyleSelected();
			checkBoxAlignRight.Enabled = radioButtonLeftJustified.Checked || radioButtonRightJustified.Checked;
		}

		private bool anyStyleSelected()
		{
			return radioButtonFreeform.Checked || radioButtonLeftJustified.Checked || radioButtonRightJustified.Checked || radioButtonAbove.Checked;
		}

		private bool anyFibItemSelected()
		{
			if (activeView != null)
			{
				return activeView.AnyFibItemSelected();
			}

			return false;
		}

		private bool singleFibItemSelected()
		{
			int count = 0;

			if (activeView != null)
			{
				return activeView.OnlyOneFibItemSelected();
			}

			return (count == 1);
		}

		private bool identicallyStyledFibItemsSelected()
		{
			if (activeView != null)
			{
				return activeView.SelectedFibItemsHaveSameStyle();
			}

			return false;
		}

		private static void incrementStyleOccurrenceCount(Dictionary<string, int> styleOccurrences, string styleString)
		{
			if (!styleOccurrences.ContainsKey(styleString))
			{
				styleOccurrences.Add(styleString, 0);
			}

			styleOccurrences[styleString]++;
		}

		protected override void OnLoad(EventArgs e)
		{
			showApplyOptions();
			checkStyleButtons();
			base.OnLoad(e);
		}

		private void showApplyOptions()
		{
			if (!anyFibItemSelected())
			{
				buttonApplySelected.Visible = false;
				labelApplyOptions.Text = Properties.Resources.ApplyStyleToAllFIBItems;
			}
		}


		private void checkStyleButtons()
		{
			string style = null;

			if (singleFibItemSelected() || identicallyStyledFibItemsSelected())
			{
				style = activeView.GetStyleOfFirstSelectedFibItem();
			}

			if (style == null)
			{
				style = string.Empty;
			}

			radioButtonFreeform.Checked = style == "freeform";
			radioButtonLeftJustified.Checked = style.Contains("leftAlignLabels");
			radioButtonRightJustified.Checked = style.Contains("rightAlignLabels");
			radioButtonAbove.Checked = style.Contains("topLabels");

			checkBoxAlignRight.Checked = style.Contains("Justified");
		}

		private bool applyToAll = true;
		private void buttonApplySelected_Click(object sender, EventArgs e)
		{
			applyToAll = false;
		}

		private void buttonApplyAll_Click(object sender, EventArgs e)
		{
			applyToAll = true;
		}

		private void updatePreview(Control sample)
		{
			panelPreview.Controls.Clear();
			sample.Size = panelPreview.ClientSize;
			sample.Dock = DockStyle.Fill;
			panelPreview.Controls.Add(sample);
		}

		private void radioButtonAbove_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButtonAbove.Checked)
			{
				updatePreview(new FibAboveLabelsSample());
			}
		}

		private void radioButtonLeftJustified_CheckedChanged(object sender, EventArgs e)
		{
			previewLeftLabels();
		}

		private void previewLeftLabels()
		{
			if (radioButtonLeftJustified.Checked)
			{
				if (checkBoxAlignRight.Checked)
				{
					updatePreview(new FibLeftLabelsJustifiedSample());
				}
				else
				{
					updatePreview(new FibLeftLabelsSample());
				}
			}
		}

		private void radioButtonFreeform_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButtonFreeform.Checked)
			{
				updatePreview(new FibFreeformSample());
			}
		}

		private void radioButtonRightJustified_CheckedChanged(object sender, EventArgs e)
		{
			previewRightLabels();
		}

		private void previewRightLabels()
		{
			if (radioButtonRightJustified.Checked)
			{
				if (checkBoxAlignRight.Checked)
				{
					updatePreview(new FibRightLabelsJustifiedSample());
				}
				else
				{
					updatePreview(new FibRightLabelsSample());
				}
			}
		}

		private void checkBoxAlignRight_CheckedChanged(object sender, EventArgs e)
		{
			previewLeftLabels();
			previewRightLabels();
		}

		private void radioButtonAbove_Click(object sender, EventArgs e)
		{
			if (!radioButtonAbove.Checked)
			{
				radioButtonAbove.Checked = true;
			}
		}

		private void radioButtonLeftJustified_Click(object sender, EventArgs e)
		{
			if (!radioButtonLeftJustified.Checked)
			{
				radioButtonLeftJustified.Checked = true;
			}
		}

		private void radioButtonRightJustified_Click(object sender, EventArgs e)
		{
			if (!radioButtonRightJustified.Checked)
			{
				radioButtonRightJustified.Checked = true;
			}
		}

		private void radioButtonFreeform_Click(object sender, EventArgs e)
		{
			if (!radioButtonFreeform.Checked)
			{
				radioButtonFreeform.Checked = true;
			}
		}
	}
}