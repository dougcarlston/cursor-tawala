// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Tawala.Common;
using Tawala.DesignerUI.Properties;

namespace Tawala.DesignerUI
{
    /// <summary>
    /// Summary description for DeployedProject.
    /// </summary>
    public partial class DeployedProjectForm : Form
    {
        public DeployedProjectForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            textBoxURL.Text = "";
            depLink.Text = "";
        }

        public Label LabelSuccess { get { return labelSuccess; } set { labelSuccess = value; } }

        public Label LabelMessage { get { return labelMessage; } set { labelMessage = value; } }

        public DeploymentResponse.StartingPointList StartingPoingList
        {
            set
            {
                comboBoxForms.DataSource = value;
                comboBoxForms.DisplayMember = "Name";

                // set the initial link to the first form
                setUrlLink(value[0].URL);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void depLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
#if false
    // this approach does not open a new browser window if one's already open
			ProcessStartInfo sInfo = new ProcessStartInfo(e.Link.LinkData.ToString());
			Process.Start(sInfo);
#endif
            // this approach opens a new browser window for the displayed Form, regardless of designers' browser settings;
            // thanks to Ryan Farley: http://ryanfarley.com/blog/archive/2004/05/16/649.aspx
            var p = new Process();
            p.StartInfo.FileName = RegistryHelper.GetDefaultBrowser();
            p.StartInfo.Arguments = e.Link.LinkData.ToString();
            p.Start();
        }

        private void comboBoxForms_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedStartingPoint = (DeploymentResponse.StartingPointInfo)comboBoxForms.SelectedItem;
            setUrlLink(selectedStartingPoint.URL);
        }

        private void setUrlLink(string urlString)
        {
            depLink.Links.Remove(depLink.Links[0]);
            depLink.Links.Add(0, urlString.Length, urlString);

            textBoxURL.Text = urlString;

            depLink.Text = Resources.DeploymentLinkText;
        }
    }
}