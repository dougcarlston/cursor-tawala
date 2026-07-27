// $Workfile: ViewInfoBar.cs $
// $Revision: 10 $	$Date: 11/25/05 4:35p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;

namespace Tawala.Processes
{
	public partial class ViewInfoBar : UserControl
	{
		IProcess curProcess = null;

		public ViewInfoBar(IProcess process)
		{
			curProcess = process;

			InitializeComponent();

			DoubleBuffered = true;

			/// On application idle make sure the info bar message is current.
			/// We don't hook any project events but instead 
			Application.Idle += new EventHandler(delegate(Object o, EventArgs e) { updateMessage(); });
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.DrawLine(SystemPens.ControlDark, 0, Height - 2, Width, Height - 2);
			e.Graphics.DrawLine(SystemPens.ControlDarkDark, 0, Height - 1, Width, Height - 1);
		}

		/// <summary>
		/// Clicking on the infobar displays a context menu.  Doesn't have to be a right click.
		/// </summary>
		private void label_Click(object sender, EventArgs e)
		{
			contextMenuStrip.Show(Cursor.Position);
		}

		/// <summary>
		/// Update the infobar's message text.  
		/// </summary>
		/// <remarks>
		/// Maybe called when nothing has changed so the function
		/// should do some simple checks to avoid unecessarilly regenerating and reseting the text.
		/// </remarks>
		private void updateMessage()
		{
			if (FindForm() != null && Project.Current != null && curProcess != null)
			{
				string newText = label.Text;

				if (Project.Current.FormList.Count == 0)
				{
					newText = Properties.Resources.ViewInfoBarNoForms;
				}
				else
				{
					int formsConnected = Project.Current.GetFormConnectionCount(curProcess);

					if (formsConnected == 0)
					{
						newText = Properties.Resources.ViewInfoBarNoConnections;
					}
					else if (formsConnected == 1)
					{
						string name = "?";

						foreach (IForm form in Project.Current.FormList)
						{
							if (form.ConnectedProcess == curProcess)
							{
								name = form.Name;
								break;
							}
						}

						newText = string.Format(Properties.Resources.ViewInfoBarOneConnection, name);
					}
					else
					{
						newText = string.Format(Properties.Resources.ViewInfoBarManyConnections, formsConnected);
					}
				}

				if (label.Text.CompareTo(newText) != 0)
				{
					label.Text = newText;
				}
			}
		}

		/// <summary>
		/// Context menu that displays forms for connecting/disconnecting.
		/// Built on the fly.  Connected forms are checked.
		/// </summary>
		private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
		{
			contextMenuStrip.Items.Clear();

			// cancel the context menu if no forms in the project
			e.Cancel = Project.Current.FormList.Count == 0;

			foreach (IForm form in Project.Current.FormList)
			{
				IProcess cp = form.ConnectedProcess;
				ToolStripMenuItem menuItem = contextMenuStrip.Items.Add(form.Name) as ToolStripMenuItem;
				menuItem.Checked = cp == curProcess;
				menuItem.Enabled = cp == curProcess || cp == null;
				menuItem.CheckOnClick = true;
				menuItem.Click += new EventHandler(menuItem_Click);
			}
		}

		/// <summary>
		/// Form context menu item click, connect or disconnect form.
		/// </summary>
		/// <remarks>
		/// There currently isn't a project event triggered by this so the Project Pane won't update!!
		/// </remarks>
		void menuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem menuItem = sender as ToolStripMenuItem;

			if (menuItem.Checked)
			{
				Project.Current.ConnectProcessToForm(curProcess, menuItem.Text);
			}
			else
			{
				Project.Current.DisconnectProcessFromForm(menuItem.Text);
			}
		}
	}
}
