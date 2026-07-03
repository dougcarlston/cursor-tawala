// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Windows.Forms;
using Tawala.Common;

namespace TawalaDesigner.Dialogs
{
	/// <summary>
	/// Summary description for About.
	/// </summary>
	public partial class HelpAboutView : System.Windows.Forms.Form
    {
		public HelpAboutView()
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
