// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Windows.Forms;
using System.ComponentModel;
using Tawala.Projects;
using Tawala.Projects.Deployment;
using Tawala.Common;

namespace TawalaDesigner.Dialogs
{
	public partial class DeployingProjectView : System.Windows.Forms.Form
	{
		private IDeploymentResponse deploymentResponse;
		private ProjectDeployer projectDeployer;

		public DeployingProjectView()
		{
			InitializeComponent();

			initializeForDeployment();
		}

		public IDeploymentResponse DeploymentResponse
		{
			get { return deploymentResponse; }
		}

		private void DeployingProjectView_Load(object sender, EventArgs e)
		{
			deployProject();
		}

		private void initializeForDeployment()
		{
			this.Text = string.Format("Deploying Project: {0}", Project.Current.Name);
			string projectXml = Project.Current.ToXmlForUpload(GlobalSettings.CredentialsElement());
			projectDeployer = new ProjectDeployer(projectXml);
		}

		private void deployProject()
		{
			backgroundWorkerDeploy.RunWorkerAsync();
		}

		private void backgroundWorkerDeploy_DoWork(object sender, DoWorkEventArgs e)
		{
			deploymentResponse = projectDeployer.Deploy();
		}

		private void backgroundWorkerDeploy_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (deploymentErrorOccurred())
			{
				if (credentialsAuthorizationFailed())
				{
					DialogResult promptForCredentialsResult = GlobalSettings.PromptForCredentials(Application.OpenForms[0]);

					if (promptForCredentialsResult == DialogResult.OK)
					{
						initializeForDeployment();
						deployProject();
					}
					else if (promptForCredentialsResult == DialogResult.Cancel)
					{
						this.Hide();
					}
				}
			}
			else
			{
				this.Close();
				new ProjectDeployedView(deploymentResponse).ShowDialog(Application.OpenForms[0]);
			}
		}

		private bool credentialsAuthorizationFailed()
		{
			return deploymentResponse.Error.Id == "auth.failed";
		}

		private bool deploymentErrorOccurred()
		{
			return deploymentResponse.Error != null;
		}
	}
}
