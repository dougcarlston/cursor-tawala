// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Tawala.Functions.Controls;
using Tawala.Functions.ViewPresenter;
using Tawala.Functions.Runtime;
using Tawala.Proj;
using Tawala.Proj.Forms.FormItemContents;
using Tawala.Proj.Forms.NewFormItems;
using Tawala.ProjectUI;
using Tawala.Common;

namespace Tawala.FormDesigner
{
	public partial class McqChoicesView : System.Windows.Forms.Form, IMcqChoicesView
	{
		private IMcqChoicesPresenter presenter;

		public McqChoicesView(IMcqItem mcqItem)
		{
			InitializeComponent();

			presenter = new McqChoicesPresenter(this, mcqItem);
		}

		protected override void OnLoad(EventArgs e)
		{
			textBoxChoices.SelectionStart = textBoxChoices.Text.Length;

			if (FieldsPalette.Palette != null)
			{
				FieldsPalette.Palette.FieldNodeDoubleClick += palette_FieldNodeDoubleClick;
			}

			base.OnLoad(e);
		}

		private void toolStripButtonOK_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;

			presenter.ChoicesAccepted();

			Close();
		}

		private void toolStripButtonCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void palette_FieldNodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			presenter.FieldsPaletteDoubleClicked(e.Node.Tag as IField);
		}

		#region IMcqChoicesView Members

		public string ChoiceStrings
		{
			get { return textBoxChoices.Text; }
			set { textBoxChoices.Text = value; }
		}

		public void InsertFieldString(string fieldString)
		{
			textBoxChoices.SelectedText = fieldString;
		}

		private int choiceSourceIndex = NewMcqItem.StaticChoices;

		public void SetChoiceSource(int choiceSourceIndex)
		{
			this.choiceSourceIndex = choiceSourceIndex;
		}

		public void EnableChoiceConfiguration(bool enable)
		{
			linkLabelConfigure.Enabled = enable;
			textBoxChoices.Enabled = !enable;
		}

		#endregion

		private void textBoxChoices_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(IPaletteField)))
			{
				e.Effect = DragDropEffects.Copy;
			}
		}

		private void textBoxChoices_DragDrop(object sender, DragEventArgs e)
		{
			IPaletteField paletteField = e.Data.GetData(typeof(IPaletteField)) as IPaletteField;

			if (paletteField != null)
			{
				presenter.FieldDropped(paletteField);
			}
		}

		private void McqChoicesView_Load(object sender, EventArgs e)
		{
			comboBoxSource.SelectedIndex = choiceSourceIndex;
		}

		private void linkLabelConfigure_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			presenter.ConfigurationRequested();
		}

		private void comboBoxSource_SelectedIndexChanged(object sender, EventArgs e)
		{
			presenter.ChoiceSourceChanged(comboBoxSource.SelectedIndex);
		}
	}
}