// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Deployment;

namespace TawalaDesigner.Dialogs
{
	public partial class ProjectDeployedView : System.Windows.Forms.Form, IProjectDeployedView
	{
		private IProjectDeployedPresenter presenter;
		private IDeploymentResponse deploymentResponse;

		public ProjectDeployedView(IDeploymentResponse deploymentResponse)
		{
			InitializeComponent();

			this.presenter = new ProjectDeployedPresenter(this);
			this.deploymentResponse = deploymentResponse;

			if (deploymentResponse != null && deploymentResponse.Status == "success")
			{
				labelSuccess.Text = Properties.Resources.DeploySuccess;
				labelMessage.Text = Properties.Resources.DeploySuccessMessage;

				comboBoxForms.DataSource = deploymentResponse.GetStartPoints(Project.Current.Name);
				comboBoxForms.DisplayMember = "FormName";
			}
			else
			{
				labelSuccess.Text = Properties.Resources.DeployFailure;
				labelMessage.Text = "";
			}
		}

		private void linkLabelViewSelectedForm_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			presenter.FormViewRequested(e.Link.LinkData.ToString());
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			presenter.CloseRequested();
		}

		private void comboBoxForms_SelectedIndexChanged(object sender, EventArgs e)
		{
			StartPoint startPoint = deploymentResponse.GetStartPoints(Project.Current.Name)[comboBoxForms.SelectedIndex];

			linkLabelViewSelectedForm.Links.RemoveAt(0);
			linkLabelViewSelectedForm.Links.Add(0, 32, startPoint.Url);

			textBoxFormUrl.Text = startPoint.Url;
		}
	}
}
