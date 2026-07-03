// $Workfile: MainForm.cs $
// $Revision: 22 $	$Date: 9/27/06 9:46p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using Tawala.Common;

namespace Tawala.Invite
{
	public partial class MainForm : Form
	{
		private InvitationView invitationView;

		private LinkLabel formViewLinkLabel;

		// tree image indices
		private const int tiClosed = 0;
		private const int tiOpen = 1;
		private const int tiForm = 2;
		private const int tiInvitation = 3;

		public MainForm()
		{
			InitializeComponent();

			invitationView = new InvitationView();
			invitationView.Dock = DockStyle.Fill;

			tree.ImageList = new ImageList();
			tree.ImageList.ColorDepth = ColorDepth.Depth24Bit;
			tree.ImageList.TransparentColor = Color.Magenta;
			tree.ImageList.Images.Add(Properties.Resources.FolderClosed);
			tree.ImageList.Images.Add(Properties.Resources.FolderOpen);
			tree.ImageList.Images.Add(Properties.Resources.Form);
			tree.ImageList.Images.Add(Properties.Resources.Invitation);

			formViewLinkLabel = new LinkLabel();
			formViewLinkLabel.VisitedLinkColor = formViewLinkLabel.ActiveLinkColor = formViewLinkLabel.LinkColor;
			formViewLinkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			formViewLinkLabel.Image = Properties.Resources.NewInvitation;
			formViewLinkLabel.ImageAlign = ContentAlignment.MiddleLeft;
			formViewLinkLabel.TextAlign = ContentAlignment.MiddleRight;
			formViewLinkLabel.Text = Properties.Resources.FormLinkLabelText;
			formViewLinkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(formViewLinkLabel_LinkClicked);

			// Put the build # in the title if iteration build
			// If no config.xml in app directory build # will be 0.
			if (Config.RuntimeEnvironment == Config.RuntimeType.Dev)
			{
				Text += " #" + Config.Build + " (DEV)";
			}
			else if (Config.RuntimeEnvironment == Config.RuntimeType.Build)
			{
				Text += " #" + Config.Build + " (BUILD)";
			}

			Application.Idle += new EventHandler(application_Idle);
		}

		/// <summary>
		/// Request list of deployments using the BackgroundWorker thread.
		/// </summary>
		private void requestDeploymentInfo()
		{
			// if thread is busy, its still handling a request for list of deployments
			if (!backgroundWorker.IsBusy)
			{
				tree.Nodes.Clear();
				invitationView.SaveChanges();
				splitContainer.Panel2.Controls.Clear();
				toolStripStatusLabel.Text = Properties.Resources.MsgQueryDeployments;
				backgroundWorker.RunWorkerAsync();
			}
		}

		private void application_Idle(object sender, EventArgs e)
		{
			bool bDeleteEnable = false;

			if (tree.ContainsFocus && tree.SelectedNode != null)
			{
				if (tree.SelectedNode.Level == 2 && !tree.SelectedNode.IsEditing)
				{
					bDeleteEnable = true;
				}
			}

			deleteToolStripButton.Enabled = bDeleteEnable;
			deleteToolStripMenuItem.Enabled = bDeleteEnable;
		}

		#region Form Events

		/// <summary>
		/// When the form is initially loaded on application launch do necessary setup.
		/// </summary>
		private void mainForm_Load(object sender, EventArgs e)
		{
			// load all invitations into master invitation list

			Invitations.Load(Config.InvitationsFile);

			// request deployments list

			requestDeploymentInfo();

			// Attach to master invitation list events

			Invitations.Inserted += new InvitationEventHandler(invitations_Inserted);
			Invitations.Removed += new InvitationEventHandler(invitations_Removed);
		}

		/// <summary>
		/// On form closing, flush any changes in the current invitation view to its
		/// invitation object then write out the master invitation list as one file.
		/// </summary>
		private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			invitationView.SaveChanges();
			Invitations.Save(Config.InvitationsFile);
		}

		void formViewLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			newToolStripMenuItem.PerformClick();
		}

		#endregion

		#region SplitContainer Events

		private void splitContainer_Panel2_Layout(object sender, LayoutEventArgs e)
		{
			if (splitContainer.Panel2.Controls.Contains(formViewLinkLabel))
			{
				int desiredWidth = formViewLinkLabel.PreferredWidth;
				if (formViewLinkLabel.Image != null)
				{
					desiredWidth += formViewLinkLabel.Image.Width + 2;
				}
				formViewLinkLabel.Width = desiredWidth;
				formViewLinkLabel.Left = Math.Max(0, (splitContainer.Panel2.Width - formViewLinkLabel.Width) / 2);
				formViewLinkLabel.Top = Math.Max(0, (splitContainer.Panel2.Height - formViewLinkLabel.Height) / 2);
			}
		}

		#endregion


		#region Invitation Events

		/// <summary>
		/// Invitation removed from master list, find it in the tree and remove it.
		/// </summary>
		void invitations_Removed(object sender, InvitationEventArgs e)
		{
			TreeNode proj = tree.Nodes[e.Inv.Project];
			TreeNode form = proj.Nodes[e.Inv.Form];
			form.Nodes.RemoveByKey(e.Inv.Name);
		}

		/// <summary>
		/// A new invitation has been inserted into the invitation list.
		/// </summary>
		/// <remarks>
		/// Normally this event will only occur when the appropriate form node
		/// is selected in the tree.  Still, I don't assume and search for it to
		/// add the node to the tree.
		/// </remarks>
		void invitations_Inserted(object sender, InvitationEventArgs e)
		{
			TreeNode proj = tree.Nodes[e.Inv.Project];
			TreeNode form = proj.Nodes[e.Inv.Form];
			TreeNode inv = new TreeNode(e.Inv.Name, tiInvitation, tiInvitation);
			inv.Name = e.Inv.Name; // used as key on form Nodes collection
			inv.Tag = e.Inv;
			// the url of an invitation always comes from its parent form.
			e.Inv.Url = form.Tag as string;
			form.Nodes.Add(inv);

			if (e.Inv.Body.Length == 0)
			{
				// REVISIT: should appear as a placeholder rather than raw url
				// (1) Because they url format could change in the retrieved deployment info.
				// (2) Might want something friendlier than a raw url.
				e.Inv.Body = "\r\n\r\n" + e.Inv.Url;
//				e.Inv.Body = String.Format("\r\n\r\nLink to {0}:{1}", e.Inv.Project, e.Inv.Form);
			}

			tree.SelectedNode = inv;
		}

		#endregion

		#region Main Menu Events

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (tree.SelectedNode.Level == 1) // form
			{
				TreeNode form = tree.SelectedNode;
				Invitations.List.Add(new Invitation(null, form.Parent.Text, form.Text));
			}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// delete menu item, toolbar but or DEL key short cut --
			// Only delete invitation if one is selected and the tree contains focus
			// If we don't check this then pressing DEL in the Subject box can delete
			// the currently selected invitation.
			if (tree.ContainsFocus)
			{
				if (tree.SelectedNode.Level == 2)
				{
					Invitations.List.Remove((Invitation)tree.SelectedNode.Tag);
				}
			}
		}

		private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
		{
			requestDeploymentInfo();
		}

		/// <summary>
		/// When View menu is dropping down set state of menu items as necessary.
		/// </summary>
		private void viewToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
		{
			// if backgroundWorker is busy it is trying to get deployment info so disable
			// Refresh Deployed Projects menu item in this case.
			refreshToolStripMenuItem.Enabled = !backgroundWorker.IsBusy;
		}

		private void toolbarToolStripMenuItem_Click(object sender, EventArgs e)
		{
			toolStrip.Visible = !toolbarToolStripMenuItem.Checked;
			toolbarToolStripMenuItem.Checked = toolStrip.Visible;
		}

		private void statusbarToolStripMenuItem_Click(object sender, EventArgs e)
		{
			statusStrip.Visible = !statusbarToolStripMenuItem.Checked;
			statusbarToolStripMenuItem.Checked = statusStrip.Visible;
		}

		#endregion

		#region Tree Events

		private void tree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			// if a project (folder node) make it appear open
			if (e.Node.Parent == null)
			{
				e.Node.ImageIndex = e.Node.SelectedImageIndex = tiOpen;
			}
		}

		private void tree_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
		{
			// if a project (folder node) make it appear closed
			if (e.Node.Parent == null)
			{
				e.Node.ImageIndex = e.Node.SelectedImageIndex = tiClosed;
			}
		}

		/// <summary>
		/// Enabled New Invitation actions when a form is selected in the tree.
		/// </summary>
		private void tree_AfterSelect(object sender, TreeViewEventArgs e)
		{
			newToolStripMenuItem.Enabled = e.Node.Level == 1; // form is level 1
			newToolStripButton.Enabled = e.Node.Level == 1;

			deleteToolStripMenuItem.Enabled = e.Node.Level == 2;
			deleteToolStripButton.Enabled = e.Node.Level == 2;

			Control newView = null;

			if (e.Node.Level == 1) // form level
			{
				newView = formViewLinkLabel;
			}
			else if (e.Node.Level == 2) // invitation level
			{
				newView = invitationView;
				invitationView.Invitation = (Invitation)e.Node.Tag;
			}

			if (newView == null || !splitContainer.Panel2.Contains(newView))
			{
				splitContainer.Panel2.Controls.Clear();
				if (newView != null)
				{
					splitContainer.Panel2.Controls.Add(newView);
				}
			}
		}

		/// <summary>
		/// A node is about to enter label edit mode.  Cancel the edit
		/// before it begins if the node affected is not an invitation node.
		/// </summary>
		private void tree_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			// only allow editing the name of invitation nodes

			if (e.Node.Level != 2)
			{
				e.CancelEdit = true;
			}
		}

		/// <summary>
		/// After the user edited an invitation node's label make sure the new label/name is not
		/// already used.  If it is, cancel the change without warning.
		/// </summary>
		/// <remarks>
		/// The BeforeLabelEdit handler will have assured that if AfterLabelEdit is triggered
		/// that it only applies to an invitation node.
		/// </remarks>
		private void tree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			// null when ESC is pressed or edit cancelled so just ignore
			if (e.Label == null || e.Node == null)
			{
				return;
			}

			// Enforce uniqueness of invitation names, regardless of which form an
			// invitation belongs to.  The master list is built this requirement.
			if (Invitations.List.Contains(e.Label))
			{
				e.CancelEdit = true;
				return;
			}

			// Do the rename through the list so that the collection can update its key
			// which is used for quickly finding invitations by name and must be kept in sync.
			Invitations.List.Rename(((Invitation)e.Node.Tag), e.Label);
			e.Node.Name = e.Label; // Name is used as Key for ContainsKey method on a TreeNodeCollection
			// e.Node.Text will get set by virtue of the fact that the edit wasn't canceled.
		}

		#endregion

		/// <summary>
		/// !!!NOTE Regarding Exceptions:  The backgroundWorker object handles them.  But if you
		/// set the debugger to break on thrown exceptions then you will still end up breaking in the debugger
		/// if for instance the website doesn't respond.  Just continue.
		/// </summary>
		private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			// This method will run on a thread other than the UI thread.
			// Be sure not to manipulate any Windows Forms controls created
			// on the UI thread from this method.
			e.Result = DeploymentInfo.Error.General;
			e.Result = DeploymentInfo.QueryServer(GlobalSettings.CredentialsElement());
		}

		/// <summary>
		/// The backgroundWorker has completed the deployments list request, with or without error.
		/// </summary>
		private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			DeploymentInfo.Error error = (DeploymentInfo.Error)e.Result;

			switch (error)
			{
				case DeploymentInfo.Error.Authentication:
					{
						// prompt the user to enter them
						if (GlobalSettings.PromptForCredentials(this) == DialogResult.OK)
						{
							// if he submitted new credential info, retry the request
							requestDeploymentInfo();
							return;
						}
						toolStripStatusLabel.Text = Properties.Resources.ErrQueryDeployments;
						return;
					}

				case DeploymentInfo.Error.General:
					{
						toolStripStatusLabel.Text = Properties.Resources.ErrQueryDeployments;
						return;
					}

				case DeploymentInfo.Error.None:
					{
						foreach (string projName in DeploymentInfo.Projects.Keys)
						{
							StartingPoints sp = DeploymentInfo.Projects[projName];
							TreeNode proj = new TreeNode(projName, tiClosed, tiClosed);
							proj.Name = proj.Text; // key for node collection
							tree.Nodes.Add(proj);

							foreach (string formName in sp.Keys)
							{
								string url = sp[formName];
								TreeNode form = new TreeNode(formName, tiForm, tiForm);
								form.Name = form.Text; // key for node collection
								form.Tag = form.ToolTipText = url;
								proj.Nodes.Add(form);
							}
						}

						// reconnect saved invitations with nodes in tree
						// REVISIT:  If a form or project is renamed, old invitations will not appear
						// but they will still be saved and loaded.

						foreach (Invitation inv in Invitations.List)
						{
							if (tree.Nodes.ContainsKey(inv.Project))
							{
								TreeNode proj = tree.Nodes[inv.Project];

								if (proj.Nodes.ContainsKey(inv.Form))
								{
									TreeNode form = proj.Nodes[inv.Form];
									TreeNode child = new TreeNode(inv.Name, tiInvitation, tiInvitation);
									child.Name = inv.Name; // used as key on form Nodes collection
									child.Tag = inv;
									inv.Url = form.Tag as string;  // always regenerate the url
									form.Nodes.Add(child);
								}
							}
						}

						toolStripStatusLabel.Text = string.Empty;

						tree.ExpandAll();
						toolStripStatusLabel.Text = "";
						break;
					}
			}
		}
	}
}