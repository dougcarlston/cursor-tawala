// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Tawala.Interfaces;
using Tawala.Projects.Processes;

namespace Tawala.ProcessDesigner
{
	public partial class ProcessViewInfoBar : UserControl
	{
		private IProcess process;
		private IProjectExplorerPresenter projectExplorerPresenter;

		public ProcessViewInfoBar()
		{
			InitializeComponent();
		}

		public ProcessViewInfoBar(IProcess process, IProjectExplorerPresenter projectExplorerPresenter)
			: this()
		{
			this.process = process;
			this.projectExplorerPresenter = projectExplorerPresenter;

			Application.Idle += new EventHandler(delegate(Object o, EventArgs e) { updateMessage(); });
		}

		private void updateMessage()
		{
			string messageText = labelMessage.Text;

			if (projectExplorerPresenter.GetFormNames().Count == 0)
			{
				messageText = Properties.Resources.ViewInfoBarNoForms;
			}
			else
			{
				int formsConnected = projectExplorerPresenter.GetConnectedFormCount(process);

				if (formsConnected == 0)
				{
					messageText = Properties.Resources.ViewInfoBarNoConnections;
				}
				else if (formsConnected == 1)
				{
					foreach (string formName in projectExplorerPresenter.GetFormNames())
					{
						IFormView formView = projectExplorerPresenter.GetFormView(formName);

						if (formView.Presenter.Form.ConnectedProcess == process)
						{
							messageText = string.Format(Properties.Resources.ViewInfoBarOneConnection, formName);
						}
					}
				}
				else
				{
					messageText = string.Format(Properties.Resources.ViewInfoBarManyConnections, formsConnected);
				}
			}

			if (messageText != labelMessage.Text)
			{
				labelMessage.Text = messageText;
			}
		}

		private void labelMessage_Click(object sender, EventArgs e)
		{
			populateContextMenuWithFormNames();
			contextMenuStrip.Show(Cursor.Position);
		}

		private void populateContextMenuWithFormNames()
		{
			contextMenuStrip.Items.Clear();

			foreach (string formName in projectExplorerPresenter.GetFormNames())
			{
				ToolStripMenuItem menuItem = contextMenuStrip.Items.Add(formName) as ToolStripMenuItem;

				menuItem.Enabled = true;

				IProcess connectedProcess = projectExplorerPresenter.GetFormView(formName).Presenter.Form.ConnectedProcess;
				menuItem.Enabled = (connectedProcess == null || connectedProcess == process);
				menuItem.Checked = (process == connectedProcess);
				menuItem.CheckOnClick = true;
				menuItem.Click += new EventHandler(menuItem_Click);
			}
		}

		void menuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem menuItem = sender as ToolStripMenuItem;

			IFormView formView = projectExplorerPresenter.GetFormView(menuItem.Text);

			if (menuItem.Checked)
			{
				projectExplorerPresenter.PostProcessConnectionRequested(formView, process.Name);
			}
			else
			{
				projectExplorerPresenter.PostProcessDisconnectionRequested(formView);
			}
		}
	}
}
