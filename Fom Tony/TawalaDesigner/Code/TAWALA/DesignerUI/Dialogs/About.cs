// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Common;

namespace Tawala.DesignerUI
{
    /// <summary>
    /// Summary description for About.
    /// </summary>
    public partial class About : Form
    {
        public About()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            labelOS.Text = AboutHelper.OsNameAndServicePack;
            labelMemory.Text += AboutHelper.FreePhysicalMemory;
            labelNetVer.Text += AboutHelper.FrameworkVersion;
            labelBuild.Text = Config.BuildName;

            for (int i = 0; i < AboutHelper.Assemblies.Length; ++i)
            {
                listBoxVersions.Items.Add(AboutHelper.Assemblies[i]);
            }
        }
    }
}